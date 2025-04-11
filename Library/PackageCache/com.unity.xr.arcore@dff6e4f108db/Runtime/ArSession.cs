using System;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Represents the context for an ARCore session.
    /// </summary>
    /// <remarks>
    /// This is an opaque object that represents a native
    /// [ArSession](https://developers.google.com/ar/reference/c/group/ar-session).
    /// </remarks>
    /// <seealso cref="ARCoreSessionSubsystem.beforeSetConfiguration"/>
    /// <seealso cref="ARCoreBeforeSetConfigurationEventArgs"/>
    /// <seealso cref="ARCoreCameraSubsystem.beforeGetCameraConfiguration"/>
    /// <seealso cref="ARCoreBeforeGetCameraConfigurationEventArgs"/>
    public struct ArSession : IEquatable<ArSession>
    {
        IntPtr m_Self;

        ArSession(IntPtr value) => m_Self = value;

        /// <summary>
        /// Creates an instance from a native pointer. The native pointer must point to an existing native `ArSession`.
        /// </summary>
        /// <param name="value">A pointer to an existing native `ArSession`.</param>
        /// <returns>An instance whose native pointer is <paramref name="value"/>.</returns>
        public static ArSession FromIntPtr(IntPtr value) => new ArSession(value);

        /// <summary>
        /// Gets the native pointer for this instance.
        /// </summary>
        /// <returns>The native pointer.</returns>
        public IntPtr AsIntPtr() => m_Self;

        /// <summary>
        /// Casts an instance to its native pointer.
        /// </summary>
        /// <param name="session">The instance to cast.</param>
        /// <returns>The native pointer.</returns>
        public static explicit operator IntPtr(ArSession session) => session.AsIntPtr();

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// Two instances are considered equal if their native pointers are equal.
        /// </remarks>
        /// <param name="other">The instance to compare against.</param>
        /// <returns><see langword="true"/> if the native pointers are equal. Otherwise, returns <see langword="false"/>.</returns>
        public bool Equals(ArSession other) => m_Self == other.m_Self;

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">An `object` to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an `ArSession` and it compares
        /// equal to this instance using <see cref="Equals(UnityEngine.XR.ARCore.ArSession)"/>.</returns>
        public override bool Equals(object obj) => obj is ArSession other && Equals(other);

        /// <summary>
        /// Generates a hash code suitable for use with a `HashSet` or `Dictionary`
        /// </summary>
        /// <returns>Returns a hash code for this instance.</returns>
        public override int GetHashCode() => m_Self.GetHashCode();

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(UnityEngine.XR.ARCore.ArSession)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(UnityEngine.XR.ARCore.ArSession)"/>. Otherwise, returns <see langword="false"/>.</returns>
        public static bool operator ==(ArSession lhs, ArSession rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to the negation of <see cref="Equals(UnityEngine.XR.ARCore.ArSession)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(UnityEngine.XR.ARCore.ArSession)"/>. Otherwise, returns <see langword="true"/>.</returns>
        public static bool operator !=(ArSession lhs, ArSession rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// This equality operator lets you compare an instance with <see langword="null"/> to determine whether its
        /// native pointer is `null`.
        /// </remarks>
        /// <example>
        /// <code>
        /// bool TestForNull(ArSession obj)
        /// {
        ///     if (obj == null)
        ///     {
        ///         // obj.AsIntPtr() is IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="lhs">The nullable `ArSession` to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable `ArSession` to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not `null` and their native pointers are equal.
        /// - <paramref name="lhs"/> is `null` and <paramref name="rhs"/>'s native pointer is `null`.
        /// - <paramref name="rhs"/> is `null` and <paramref name="lhs"/>'s native pointer is `null`.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are `null`.
        ///
        /// Otherwise, returns <see langword="false"/>.
        /// </returns>
        public static bool operator ==(ArSession? lhs, ArSession? rhs) => NativeObject.ArePointersEqual(lhs?.m_Self, rhs?.m_Self);

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <remarks>
        /// This inequality operator lets you compare an instance with <see langword="null"/> to determine whether its
        /// native pointer is `null`.
        /// </remarks>
        /// <example>
        /// <code>
        /// bool TestForNull(ArSession obj)
        /// {
        ///     if (obj != null)
        ///     {
        ///         // obj.AsIntPtr() is not IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="lhs">The nullable `ArSession` to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable `ArSession` to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not `null` and their native pointers are equal.
        /// - <paramref name="lhs"/> is `null` and <paramref name="rhs"/>'s native pointer is `null`.
        /// - <paramref name="rhs"/> is `null` and <paramref name="lhs"/>'s native pointer is `null`.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are `null`.
        ///
        /// Otherwise, returns <see langword="true"/>.
        /// </returns>
        public static bool operator !=(ArSession? lhs, ArSession? rhs) => !(lhs == rhs);

        /// <summary>
        /// Sets an MP4 dataset file to playback instead of live camera feed.
        /// </summary>
        /// <remarks>
        /// Restrictions:
        /// - Can only be called while the session is paused. Playback of the MP4
        /// dataset file starts once the session is resumed.
        /// - The MP4 dataset file must use the same camera facing direction as is
        /// configured in the session.
        ///
        /// When an MP4 dataset file is set:
        /// - All existing trackables (i.e., anchors and trackables) immediately enter tracking state [TrackingState.None](xref:UnityEngine.XR.ARSubsystems.TrackingState.None).
        /// - The desired focus mode is ignored, and does not affect the previously recorded camera images.
        /// - The current camera configuration is immediately set to the default for the device the MP4 dataset file was recorded on.
        /// - Calls to retrieve the supported camera configurations return camera configs supported by the device the MP4 dataset file was recorded on.
        /// - Setting a previously obtained camera config has no effect.
        /// </remarks>
        /// <param name="path">A file path to a MP4 dataset file or `null` to use the live camera feed.</param>
        /// <returns>
        /// - Returns <see cref="ArStatus.Success"/> if successful.
        /// - Returns <see cref="ArStatus.ErrorSessionNotPaused"/> if called when session is not paused.
        /// - Returns <see cref="ArStatus.ErrorSessionUnsupported"/> if playback is incompatible with selected features.
        /// - Returns <see cref="ArStatus.ErrorPlaybackFailed"/> if an error occurred with the MP4 dataset file such as not being able to open the file or the file is unable to be decoded.
        /// </returns>
        public ArStatus SetPlaybackDataset(string path)
        {
            unsafe
            {
                if (string.IsNullOrEmpty(path))
                {
                    return SetPlaybackDataset(this, null);
                }

                using (var bytes = path.ToBytes(Encoding.UTF8, Allocator.Temp))
                {
                    return SetPlaybackDataset(this, (byte*)bytes.GetUnsafePtr());
                }
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArSession_setPlaybackDataset")]
        static extern unsafe ArStatus SetPlaybackDataset(ArSession session, byte* mp4DatasetFilePath);

        /// <summary>
        /// Gets the playback status.
        /// </summary>
        /// <value>Indicates whether the session is playing back a recording or has stopped because of an error.</value>
        public ArPlaybackStatus playbackStatus
        {
            get
            {
                GetPlaybackStatus(this, out var value);
                return value;
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArSession_getPlaybackStatus")]
        static extern void GetPlaybackStatus(ArSession session, out ArPlaybackStatus outPlaybackStatus);

        /// <summary>
        /// Starts a new MP4 dataset file recording that is written to the specific filesystem path.
        /// </summary>
        /// <remarks>
        /// Existing files are overwritten.
        ///
        /// The MP4 video stream (VGA) bitrate is 5Mbps (40Mb per minute).
        ///
        /// Recording introduces additional overhead and may affect app performance.
        /// </remarks>
        /// <param name="recordingConfig">The configuration defined for recording.</param>
        /// <returns>Returns <see cref="ArStatus.Success"/> if successful. Returns one of the following values otherwise:
        /// - <see cref="ArStatus.ErrorIllegalState"/>
        /// - <see cref="ArStatus.ErrorInvalidArgument"/>
        /// - <see cref="ArStatus.ErrorRecordingFailed"/>
        /// </returns>
        public ArStatus StartRecording(ArRecordingConfig recordingConfig) => StartRecording(this, recordingConfig);

        [DllImport("arcore_sdk_c", EntryPoint = "ArSession_startRecording")]
        static extern ArStatus StartRecording(ArSession session, ArRecordingConfig recordingConfig);

        /// <summary>
        /// Stops recording and flushes unwritten data to disk. The MP4 dataset file is ready to read after this
        /// call.
        /// </summary>
        /// <remarks>
        /// Recording can be stopped automatically when the session is paused,
        /// if auto stop is enabled via <see cref="ArRecordingConfig.SetAutoStopOnPause"/>.
        /// Recording errors that would be thrown in <see cref="StopRecording"/> are silently
        /// ignored on session pause.
        /// </remarks>
        /// <returns>Returns <see cref="ArStatus.Success"/> if successful. Returns
        ///     <see cref="ArStatus.ErrorRecordingFailed"/> otherwise.</returns>
        public ArStatus StopRecording() => StopRecording(this);

        [DllImport("arcore_sdk_c", EntryPoint = "ArSession_stopRecording")]
        static extern ArStatus StopRecording(ArSession session);

        /// <summary>
        /// Gets the current recording status.
        /// </summary>
        /// <value>Indicates whether the session is recording or has stopped because of an error.</value>
        public ArRecordingStatus recordingStatus
        {
            get
            {
                GetRecordingStatus(this, out var value);
                return value;
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArSession_getRecordingStatus")]
        static extern void GetRecordingStatus(ArSession session, out ArRecordingStatus outRecordingStatus);
    }
}
