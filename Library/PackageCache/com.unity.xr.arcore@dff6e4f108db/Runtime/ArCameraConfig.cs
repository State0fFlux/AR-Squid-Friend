using System;
using UnityEngine.XR.ARSubsystems;
#if UNITY_ANDROID && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Represents an
    /// [ARCore camera configuration](https://developers.google.com/ar/reference/c/group/ar-camera-config), which
    /// describes the ARCore-related properties of a device camera.
    /// </summary>
    public struct ArCameraConfig : IDisposable, IEquatable<ArCameraConfig>
    {
        IntPtr m_Self;

        ArCameraConfig(IntPtr value) => m_Self = value;

        /// <summary>
        /// Creates a <see cref="ArCameraConfig"/> from a native pointer. The native pointer must point
        /// to an existing <see cref="ArCameraConfig"/>.
        /// </summary>
        /// <param name="value">A pointer to an existing native `ArCameraConfig`.</param>
        /// <returns>An instance whose native pointer is <paramref name="value"/>.</returns>
        public static ArCameraConfig FromIntPtr(IntPtr value) => new ArCameraConfig(value);

        /// <summary>
        /// Gets the native pointer for this instance.
        /// </summary>
        /// <returns>The native pointer.</returns>
        public IntPtr AsIntPtr() => m_Self;

        /// <summary>
        /// Creates a new instance.
        /// > [!IMPORTANT]
        /// > You must <see cref="Dispose"/> this instance when you no longer need it.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        public ArCameraConfig(ArSession session) => Create(session, out this);

        /// <summary>
        /// Destroys this instance and sets its native pointer to <see langword="null"/>.
        /// </summary>
        /// <remarks>
        /// You should only dispose an <see cref="ArCameraConfig"/> if you explicitly created it, e.g., by calling
        /// <see cref="ArCameraConfig(ArSession)"/>. If you convert an existing config from an <see cref="IntPtr"/>
        /// (e.g., by calling <see cref="FromIntPtr"/>), then you should not dispose it.
        /// </remarks>
        public void Dispose()
        {
            if (m_Self != IntPtr.Zero)
            {
                Destroy(this);
            }

            m_Self = default;
        }

        /// <summary>
        /// Gets the camera ID for this instance.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>The camera ID for this instance.</returns>
        public string GetCameraId(ArSession session)
        {
            GetCameraId(session, this, out var cameraId);
            using (cameraId)
            {
                return cameraId.ToString();
            }
        }

        /// <summary>
        /// Gets the depth sensor usage settings.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>The depth sensor usage settings.</returns>
        public ArCameraConfigDepthSensorUsage GetDepthSensorUsage(ArSession session)
        {
            GetDepthSensorUsage(session, this, out var value);
            return value;
        }

        /// <summary>
        /// Gets the facing direction of the camera selected by this config.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>The facing direction.</returns>
        public ArCameraConfigFacingDirection GetFacingDirection(ArSession session)
        {
            GetFacingDirection(session, this, out var value);
            return value;
        }

        /// <summary>
        /// Gets the minimum and maximum camera capture rate in frames per second (fps) for the current camera config.
        /// </summary>
        /// <remarks>
        /// Actual capture frame rate will vary within this range, depending on lighting conditions. Frame rates will
        /// generally be lower under poor lighting conditions to accommodate longer exposure times.
        /// </remarks>
        /// <param name="session">The ARCore session.</param>
        /// <returns>The minimum and maximum camera capture rate in frames per second supported by this camera config.</returns>
        public (int minFps, int maxFps) GetFpsRange(ArSession session)
        {
            GetFpsRange(session, this, out var min, out var max);
            return (min, max);
        }

        /// <summary>
        /// Gets the camera image dimensions for this camera config.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>Returns the camera image dimensions for this camera config.</returns>
        public (int width, int height) GetImageDimensions(ArSession session)
        {
            GetImageDimensions(session, this, out var width, out var height);
            return (width, height);
        }

        /// <summary>
        /// Gets the camera texture dimensions for this camera config.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>Returns the camera texture dimensions for this camera config.</returns>
        public (int width, int height) GetTextureDimensions(ArSession session)
        {
            GetTextureDimensions(session, this, out var width, out var height);
            return (width, height);
        }

        /// <summary>
        /// Casts an instance to its native pointer.
        /// </summary>
        /// <param name="cameraConfig">The instance to cast.</param>
        /// <returns>The native pointer.</returns>
        public static explicit operator IntPtr(ArCameraConfig cameraConfig) => cameraConfig.AsIntPtr();

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// Two instances are considered equal if their native pointers are equal.
        /// </remarks>
        /// <param name="other">The instance to compare against.</param>
        /// <returns><see langword="true"/> if the native pointers are equal. Otherwise, returns <see langword="false"/>.</returns>
        public bool Equals(ArCameraConfig other) => m_Self == other.m_Self;

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">An `object` to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an `ArCameraConfig` and it compares
        /// equal to this instance using <see cref="Equals(UnityEngine.XR.ARCore.ArCameraConfig)"/>.</returns>
        public override bool Equals(object obj) => obj is ArCameraConfig other && Equals(other);

        /// <summary>
        /// Generates a hash code suitable for use with a `HashSet` or `Dictionary`
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode() => m_Self.GetHashCode();

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(UnityEngine.XR.ARCore.ArCameraConfig)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(UnityEngine.XR.ARCore.ArCameraConfig)"/>. Otherwise, returns <see langword="false"/>.</returns>
        public static bool operator ==(ArCameraConfig lhs, ArCameraConfig rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to the negation of <see cref="Equals(UnityEngine.XR.ARCore.ArCameraConfig)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(UnityEngine.XR.ARCore.ArCameraConfig)"/>. Otherwise, returns <see langword="true"/>.</returns>
        public static bool operator !=(ArCameraConfig lhs, ArCameraConfig rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// This equality operator lets you compare an instance with <see langword="null"/> to
        /// determine whether its native pointer is `null`.
        /// </remarks>
        /// <example>
        /// <code>
        /// bool TestForNull(ArCameraConfig obj)
        /// {
        ///     if (obj == null)
        ///     {
        ///         // obj.AsIntPtr() is IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="lhs">The nullable `ArCameraConfig` to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable `ArCameraConfig` to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not `null` and their underlying pointers are equal.
        /// - <paramref name="lhs"/> is `null` and <paramref name="rhs"/>'s underlying pointer is `null`.
        /// - <paramref name="rhs"/> is `null` and <paramref name="lhs"/>'s underlying pointer is `null`.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are `null`.
        ///
        /// Otherwise, returns <see langword="false"/>.
        /// </returns>
        public static bool operator ==(ArCameraConfig? lhs, ArCameraConfig? rhs) => NativeObject.ArePointersEqual(lhs?.m_Self, rhs?.m_Self);

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <remarks>
        /// This inequality operator lets you compare an instance with <see langword="null"/> to
        /// determine whether its native pointer is `null`.
        /// </remarks>
        /// <example>
        /// <code>
        /// bool TestForNull(ArCameraConfig obj)
        /// {
        ///     if (obj != null)
        ///     {
        ///         // obj.AsIntPtr() is not IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="lhs">The nullable `ArCameraConfig` to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable `ArCameraConfig` to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not `null` and their native pointers are equal.
        /// - <paramref name="lhs"/> is `null` and <paramref name="rhs"/>'s native pointer is `null`.
        /// - <paramref name="rhs"/> is `null` and <paramref name="lhs"/>'s native pointer is `null`.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are `null`.
        ///
        /// Otherwise, returns <see langword="true"/>.
        /// </returns>
        public static bool operator !=(ArCameraConfig? lhs, ArCameraConfig? rhs) => !(lhs == rhs);

#if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfig_create")]
        static extern void Create(ArSession session, out ArCameraConfig value);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfig_destroy")]
        static extern void Destroy(ArCameraConfig self);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfig_getCameraId")]
        static extern void GetCameraId(ArSession session, ArCameraConfig self, out ArString valueOut);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfig_getDepthSensorUsage")]
        static extern void GetDepthSensorUsage(ArSession session, ArCameraConfig self, out ArCameraConfigDepthSensorUsage valueOut);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfig_getFacingDirection")]
        static extern void GetFacingDirection(ArSession session, ArCameraConfig self, out ArCameraConfigFacingDirection valueOut);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfig_getFpsRange")]
        static extern void GetFpsRange(ArSession session, ArCameraConfig self, out int minFps, out int maxFps);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfig_getImageDimensions")]
        static extern void GetImageDimensions(ArSession session, ArCameraConfig self, out int width, out int height);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfig_getTextureDimensions")]
        static extern void GetTextureDimensions(ArSession session, ArCameraConfig self, out int width, out int height);
#else
        static void Create(ArSession session, out ArCameraConfig value) => value = default;

        static void Destroy(ArCameraConfig self) { }

        static void GetCameraId(ArSession session, ArCameraConfig self, out ArString valueOut) => valueOut = default;

        static void GetDepthSensorUsage(ArSession session, ArCameraConfig self, out ArCameraConfigDepthSensorUsage valueOut) =>
            valueOut = default;

        static void GetFacingDirection(ArSession session, ArCameraConfig self, out ArCameraConfigFacingDirection valueOut) =>
            valueOut = default;

        static void GetFpsRange(ArSession session, ArCameraConfig self, out int minFps, out int maxFps)
        {
            minFps = default;
            maxFps = default;
        }

        static void GetImageDimensions(ArSession session, ArCameraConfig self, out int width, out int height)
        {
            width = default;
            height = default;
        }

        static void GetTextureDimensions(ArSession session, ArCameraConfig self, out int width, out int height)
        {
            width = default;
            height = default;
        }
#endif
    }

    /// <summary>
    /// Extensions to the [XRCameraConfiguration](xref:UnityEngine.XR.ARSubsystems.XRCameraConfiguration) object.
    /// </summary>
    public static class XRCameraConfigurationExtensions
    {
        /// <summary>
        /// Gets <paramref name="cameraConfiguration"/> as an ARCore <see cref="ArCameraConfig"/>.
        /// </summary>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > Do not <see cref="ArCameraConfig.Dispose"/> the returned <see cref="ArCameraConfig"/>.
        /// </remarks>
        /// <param name="cameraConfiguration">The
        /// [XRCameraConfiguration](xref:UnityEngine.XR.ARSubsystems.XRCameraConfiguration) being extended.</param>
        /// <returns>Returns the ARCore-specific representation of <paramref name="cameraConfiguration"/>.</returns>
        public static ArCameraConfig AsArCameraConfig(this XRCameraConfiguration cameraConfiguration) =>
            ArCameraConfig.FromIntPtr(cameraConfiguration.nativeConfigurationHandle);
    }
}
