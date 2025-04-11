using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARCore
{
    static class ArCoreImg
    {
        public abstract class ReferenceImageException : Exception
        {
            public XRReferenceImage referenceImage { get; }

            public ReferenceImageException(XRReferenceImage referenceImage) => this.referenceImage = referenceImage;
        }

        public class MissingTextureException : ReferenceImageException
        {
            public MissingTextureException(XRReferenceImage referenceImage) : base(referenceImage) { }
        }

        public class EncodeToPNGFailedException : ReferenceImageException
        {
            public EncodeToPNGFailedException(XRReferenceImage referenceImage) : base(referenceImage) { }
        }

        public class BuildDatabaseFailedException : Exception
        {
            public int exitCode { get; }

            public string stdErr { get; }

            public BuildDatabaseFailedException(int exitCode, string stdErr)
                : base($"arcoreimg failed with exit code {exitCode} and stderr:\n{stdErr}")
                => (this.exitCode, this.stdErr) = (exitCode, stdErr);
        }

        public class EmptyDatabaseException : Exception
        {
            public string stdOut { get; }
            public string stdErr { get; }

            public EmptyDatabaseException(string stdOut, string stdErr) => (this.stdOut, this.stdErr) = (stdOut, stdErr);
        }

        readonly struct ImageDatabaseEntry
        {
            internal readonly XRReferenceImage source;
            internal readonly string guidStr;
            internal readonly string destinationPath;
            internal readonly string outputText;

            internal ImageDatabaseEntry(in XRReferenceImage referenceImage, string destinationDirectory)
            {
                source = referenceImage;
                destinationPath = source.CopyTo(destinationDirectory);
                guidStr = source.guid.ToString("N");
                outputText = source.specifySize
                    ? string.Join(
                        "|",
                        guidStr,
                        destinationPath,
                        source.width.ToString("G", CultureInfo.InvariantCulture))
                    : string.Join("|", guidStr, destinationPath);
            }
        }

        readonly struct LowScoringImageData
        {
            internal readonly ImageDatabaseEntry entry;
            internal readonly int score;

            internal LowScoringImageData(ImageDatabaseEntry entry, int score)
            {
                this.entry = entry;
                this.score = score;
            }
        }

        static string packagePath => Path.GetFullPath("Packages/com.unity.xr.arcore");

#if UNITY_EDITOR_WIN
        static string extension => ".exe";
#else
        static string extension => "";
#endif

#if UNITY_EDITOR_WIN
        static string platformName => "Windows";
#elif UNITY_EDITOR_OSX
        static string platformName => "MacOS";
#elif UNITY_EDITOR_LINUX
        static string platformName => "Linux";
#else
        static string platformName => throw new NotSupportedException();
#endif

        const int k_RecommendedMinimumArCoreImageScore = 75;
        static readonly string k_BelowRecommendationLine = $"Google recommends a score of at least {k_RecommendedMinimumArCoreImageScore:D} for good image tracking.";

        static readonly string[] k_SupportedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png"
        };

        static string GetOrPrepareCliPath()
        {
            var cliPath = Path.Combine(packagePath, "Tools~", platformName, $"arcoreimg{extension}");

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            Cli.Execute("/bin/chmod", $"+x \"{cliPath}\"");
#endif

            return cliPath;
        }

        internal static (string stdOut, string stdErr, int exitCode) BuildDb(string inputImageListPath, string outputDbPath)
        {
            var cliPath = GetOrPrepareCliPath();

            if (File.Exists(outputDbPath))
            {
                File.Delete(outputDbPath);
            }

            return Cli.Execute(cliPath, new[]
            {
                "build-db",
                $"--input_image_list_path=\"{inputImageListPath}\"",
                $"--output_db_path=\"{outputDbPath}\"",
            });
        }

        static string GetTempFileNameWithoutExtension() => Guid.NewGuid().ToString("N");

        public static byte[] BuildDb(XRReferenceImageLibrary library)
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), GetTempFileNameWithoutExtension());

            try
            {
                Directory.CreateDirectory(tempDirectory);
                var imageListPath = library.ToInputImageListPath(tempDirectory);
                var dbPath = Path.Combine(tempDirectory, $"{GetTempFileNameWithoutExtension()}.imgdb");
                var (stdOut, stdErr, exitCode) = BuildDb(imageListPath, dbPath);

                if (exitCode != 0)
                    throw new BuildDatabaseFailedException(exitCode, stdErr);

                if (!File.Exists(dbPath))
                    throw new EmptyDatabaseException(stdOut, stdErr);

                return File.ReadAllBytes(dbPath);
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }
        }

        static int EvaluateImageScore(in ImageDatabaseEntry entry)
        {
            if (!File.Exists(entry.destinationPath))
                return -1;

            var cliPath = GetOrPrepareCliPath();
            var commandAndArguments = new[] { "eval-img", $"--input_image_path={entry.destinationPath}" };

            var result = Cli.Execute(cliPath, commandAndArguments);

            if (result.exitCode == 0 && int.TryParse(result.stdout, out var score))
            {
                return score;
            }

            Debug.LogError($"Unable to score image: {entry.source.name} ({entry.guidStr})");

            return -1;
        }

        static string CopyTo(this XRReferenceImage referenceImage, string destinationDirectory)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(referenceImage.textureGuid.ToString("N"));
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
                throw new MissingTextureException(referenceImage);

            var fileExtension = Path.GetExtension(assetPath);
            if (k_SupportedExtensions.Any(supportedExtension => fileExtension.Equals(supportedExtension, StringComparison.OrdinalIgnoreCase)))
            {
                var pathWithLowercaseExtension = Path.Combine(destinationDirectory, $"{GetTempFileNameWithoutExtension()}{fileExtension.ToLower()}");
                File.Copy(assetPath, pathWithLowercaseExtension);
                return pathWithLowercaseExtension;
            }
            else
            {
                var path = Path.Combine(destinationDirectory, $"{GetTempFileNameWithoutExtension()}.png");
                var bytes = texture.EncodeToPNG();
                if (bytes == null)
                    throw new EncodeToPNGFailedException(referenceImage);

                File.WriteAllBytes(path, bytes);
                return path;
            }
        }

        static string ToInputImageListPath(this XRReferenceImageLibrary library, string destinationDirectory)
        {
            var lowScoringEntries = new List<LowScoringImageData>();
            var entries = new List<string>();
            foreach (var referenceImage in library)
            {
                var dbEntry = new ImageDatabaseEntry(referenceImage, destinationDirectory);
                var score = EvaluateImageScore(dbEntry);

                if (score < k_RecommendedMinimumArCoreImageScore && score >= 0)
                {
                    lowScoringEntries.Add(new LowScoringImageData(dbEntry, score));
                }

                entries.Add(dbEntry.outputText);
            }

            var path = Path.Combine(destinationDirectory, $"{GetTempFileNameWithoutExtension()}.txt");
            File.WriteAllText(path, string.Join("\n", entries));

            if (lowScoringEntries.Count == 0)
                return path;

            var libraryPath = AssetDatabase.GetAssetPath(library);
            ReportLowScoringEntries(lowScoringEntries, library.name, libraryPath);

            return path;
        }

        static void ReportLowScoringEntries(
            List<LowScoringImageData> lowScoringImages,
            string libraryName,
            string libraryPath)
        {
            var builder = new StringBuilder();

            const string barLine = "------";

            builder.Append("Low scoring image report from 'arcoreimg eval-img' for library ")
               .Append(libraryName)
               .AppendLine()
               .AppendLine(libraryPath)
               .AppendLine()
               .AppendLine(k_BelowRecommendationLine)
               .AppendLine()
               .AppendLine(barLine)
               .AppendLine();

            foreach (var lqe in lowScoringImages)
            {
                builder.Append("- ")
                   .Append(lqe.entry.source.name)
                   .Append(" (")
                   .Append(lqe.entry.guidStr)
                   .Append(") | score: ")
                   .Append(lqe.score.ToString("D"))
                   .Append("/100")
                   .AppendLine();
            }

            builder.AppendLine().AppendLine(barLine);

            Debug.LogWarning(builder.ToString());
        }
    }
}
