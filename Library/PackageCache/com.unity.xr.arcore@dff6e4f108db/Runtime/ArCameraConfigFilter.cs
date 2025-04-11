using System;
#if UNITY_ANDROID && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Used to filter the set of camera configurations returned by
    /// [XRCameraSubsystem.GetConfigurations](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystem.GetConfigurations(Unity.Collections.Allocator)).
    /// </summary>
    /// <seealso cref="ARCoreCameraSubsystem.beforeGetCameraConfiguration"/>
    /// <seealso cref="ARCoreBeforeGetCameraConfigurationEventArgs"/>
    public struct ArCameraConfigFilter : IDisposable, IEquatable<ArCameraConfigFilter>
    {
        IntPtr m_Self;

        ArCameraConfigFilter(IntPtr value) => m_Self = value;

        /// <summary>
        /// Creates an instance from a native pointer. The native pointer must point to an existing native `ArCameraConfigFilter`.
        /// </summary>
        /// <param name="value">A pointer to an existing native `ArCameraConfigFilter`.</param>
        /// <returns>An instance whose native pointer is <paramref name="value"/>.</returns>
        public static ArCameraConfigFilter FromIntPtr(IntPtr value) => new(value);

        /// <summary>
        /// Creates a new, default-constructed instance.
        /// </summary>
        /// <param name="session">A non-null `ARSession`.</param>
        /// <remarks>
        /// When you no longer need the `ArCameraConfigFilter`, you must destroy it by calling <see cref="Dispose"/>.
        /// If you do not dispose it, ARCore will leak memory.
        /// </remarks>
        public ArCameraConfigFilter(ArSession session) => Create(session, out this);

        /// <summary>
        /// Gets the native pointer for this instance.
        /// </summary>
        /// <returns>The native pointer.</returns>
        public IntPtr AsIntPtr() => m_Self;

        /// <summary>
        /// Gets the desired depth sensor usages to allow.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>The `ArCameraConfigDepthSensorUsage` values allowed by this filter.</returns>
        public ArCameraConfigDepthSensorUsage GetDepthSensorUsage(ArSession session)
        {
            GetDepthSensorUsage(session, this, out var usage);
            return usage;
        }

        /// <summary>
        /// Sets the desired depth sensor usages to allow.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <param name="depthSensorUsage">The `ArCameraConfigDepthSensorUsage` values allowed by this filter.</param>
        public void SetDepthSensorUsage(ArSession session, ArCameraConfigDepthSensorUsage depthSensorUsage) =>
            SetDepthSensorUsage(session, this, depthSensorUsage);

        /// <summary>
        /// Gets the desired frame rates to allow.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>The frame rates allowed by this filter.</returns>
        public ArCameraConfigTargetFps GetTargetFps(ArSession session)
        {
            GetTargetFps(session, this, out var value);
            return value;
        }

        /// <summary>
        /// Sets the desired frame rates to allow.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <param name="targetFps">The frame rates allowed by this filter.</param>
        public void SetTargetFps(ArSession session, ArCameraConfigTargetFps targetFps) =>
            SetTargetFps(session, this, targetFps);

        /// <summary>
        /// Destroys this instance and sets its native pointer to <see langword="null"/>.
        /// </summary>
        /// <remarks>
        /// You should only dispose an `ArCameraConfigFilter` if you explicitly created it, e.g., by calling
        /// <see cref="ArCameraConfigFilter(ArSession)"/>. If you convert an existing config from an
        /// <see cref="IntPtr"/> (e.g., by calling <see cref="FromIntPtr"/>), then you should not dispose it.
        /// </remarks>
        public void Dispose()
        {
            if (m_Self != IntPtr.Zero)
            {
                Destroy(this);
            }

            m_Self = IntPtr.Zero;
        }

        /// <summary>
        /// Casts an instance to its underlying native pointer.
        /// </summary>
        /// <param name="filter">The instance to cast.</param>
        /// <returns>Returns the underlying native pointer for <paramref name="filter"/></returns>
        public static explicit operator IntPtr(ArCameraConfigFilter filter) => filter.AsIntPtr();

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// Two instances are considered equal if their native pointers are equal.
        /// </remarks>
        /// <param name="other">The instance to compare against.</param>
        /// <returns><see langword="true"/> if the native pointers are equal. Otherwise, returns <see langword="false"/>.</returns>
        public bool Equals(ArCameraConfigFilter other) => m_Self == other.m_Self;

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">An object to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an `ArCameraConfigFilter` and
        /// compares equal to this instance using <see cref="Equals(UnityEngine.XR.ARCore.ArCameraConfigFilter)"/>.</returns>
        public override bool Equals(object obj) => obj is ArCameraConfigFilter other && Equals(other);

        /// <summary>
        /// Generates a hash code suitable for use with a `HashSet` or `Dictionary`
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode() => m_Self.GetHashCode();

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(UnityEngine.XR.ARCore.ArCameraConfigFilter)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(UnityEngine.XR.ARCore.ArCameraConfigFilter)"/>. Otherwise, returns <see langword="false"/>.</returns>
        public static bool operator ==(ArCameraConfigFilter lhs, ArCameraConfigFilter rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to the negation of <see cref="Equals(UnityEngine.XR.ARCore.ArCameraConfigFilter)"/>.
        /// </summary>
        /// <param name="lhs">The <see cref="ArCameraConfigFilter"/> to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The <see cref="ArCameraConfigFilter"/> to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(UnityEngine.XR.ARCore.ArCameraConfigFilter)"/>. Otherwise, returns <see langword="true"/>.</returns>
        public static bool operator !=(ArCameraConfigFilter lhs, ArCameraConfigFilter rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// This equality operator lets you compare an `ArCameraConfigFilter` with <see langword="null"/> to determine
        /// whether its native pointer is `null`.
        /// </remarks>
        /// <example>
        /// <code>
        /// bool TestForNull(ArCameraConfigFilter obj)
        /// {
        ///     if (obj == null)
        ///     {
        ///         // obj.AsIntPtr() is IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="lhs">The nullable `ArCameraConfigFilter` to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable `ArCameraConfigFilter` to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not `null` and their native pointers are equal.
        /// - <paramref name="lhs"/> is `null` and <paramref name="rhs"/>'s native pointer is `null`.
        /// - <paramref name="rhs"/> is `null` and <paramref name="lhs"/>'s native pointer is `null`.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are `null`.
        ///
        /// Otherwise, returns <see langword="false"/>.
        /// </returns>
        public static bool operator ==(ArCameraConfigFilter? lhs, ArCameraConfigFilter? rhs) => NativeObject.ArePointersEqual(lhs?.m_Self, rhs?.m_Self);

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <remarks>
        /// This inequality operator lets you compare an `ArCameraConfigFilter` with <see langword="null"/> to determine
        /// whether its native pointer is `null`.
        /// </remarks>
        /// <example>
        /// <code>
        /// bool TestForNull(ArCameraConfigFilter obj)
        /// {
        ///     if (obj != null)
        ///     {
        ///         // obj.AsIntPtr() is not IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="lhs">The nullable `ArCameraConfigFilter` to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable `ArCameraConfigFilter` to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not `null` and their native pointers are equal.
        /// - <paramref name="lhs"/> is `null` and <paramref name="rhs"/>'s native pointer is `null`.
        /// - <paramref name="rhs"/> is `null` and <paramref name="lhs"/>'s native pointer is `null`.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are `null`.
        ///
        /// Otherwise, returns <see langword="true"/>.
        /// </returns>
        public static bool operator !=(ArCameraConfigFilter? lhs, ArCameraConfigFilter? rhs) => !(lhs == rhs);

#if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfigFilter_create")]
        static extern void Create(ArSession session, out ArCameraConfigFilter filterOut);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfigFilter_destroy")]
        static extern void Destroy(ArCameraConfigFilter self);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfigFilter_getDepthSensorUsage")]
        static extern void GetDepthSensorUsage(ArSession session, ArCameraConfigFilter filter, out ArCameraConfigDepthSensorUsage valueOut);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfigFilter_setDepthSensorUsage")]
        static extern void SetDepthSensorUsage(ArSession session, ArCameraConfigFilter filter, ArCameraConfigDepthSensorUsage value);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfigFilter_getTargetFps")]
        static extern void GetTargetFps(ArSession session, ArCameraConfigFilter filter, out ArCameraConfigTargetFps valueOut);

        [DllImport("arcore_sdk_c", EntryPoint = "ArCameraConfigFilter_setTargetFps")]
        static extern void SetTargetFps(ArSession session, ArCameraConfigFilter filter, ArCameraConfigTargetFps value);
#else
        static void Create(ArSession session, out ArCameraConfigFilter filterOut) => filterOut = default;

        static void Destroy(ArCameraConfigFilter self) { }

        static void GetDepthSensorUsage(ArSession session, ArCameraConfigFilter filter, out ArCameraConfigDepthSensorUsage valueOut) =>
            valueOut = default;

        static void SetDepthSensorUsage(ArSession session, ArCameraConfigFilter filter, ArCameraConfigDepthSensorUsage value) { }

        static void GetTargetFps(ArSession session, ArCameraConfigFilter filter, out ArCameraConfigTargetFps valueOut) =>
            valueOut = default;

        static void SetTargetFps(ArSession session, ArCameraConfigFilter filter, ArCameraConfigTargetFps value) { }
#endif
    }
}
