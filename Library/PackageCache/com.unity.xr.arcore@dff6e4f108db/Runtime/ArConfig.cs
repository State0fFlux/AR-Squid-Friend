using System;
#if UNITY_ANDROID
using System.Runtime.InteropServices;
#endif

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Represents the configuration for an <see cref="ArSession"/>.
    /// </summary>
    /// <remarks>
    /// This is an opaque object that represents a native
    /// [ArConfig](https://developers.google.com/ar/reference/c/group/ar-config).
    /// </remarks>
    /// <seealso cref="ARCoreSessionSubsystem.beforeSetConfiguration"/>
    /// <seealso cref="ARCoreBeforeSetConfigurationEventArgs"/>
    public struct ArConfig : IEquatable<ArConfig>, IDisposable
    {
        IntPtr m_Self;

        ArConfig(IntPtr value) => m_Self = value;

        /// <summary>
        /// Creates a new session configuration and initializes it to a sensible default configuration.
        /// </summary>
        /// <remarks>
        /// Plane detection and Lighting Estimation are enabled, and blocking update is selected. This configuration is
        /// guaranteed to be supported on all devices that support ARCore.
        ///
        /// When you no longer need the `ArConfig`, you must destroy it by calling <see cref="Dispose"/>.
        /// If you do not dispose it, ARCore will leak memory.
        /// </remarks>
        /// <param name="session">A non-null <see cref="ArSession"/>.</param>
        public ArConfig(ArSession session) => Create(session, out this);

        /// <summary>
        /// Creates an instance from a native pointer. The native pointer must point to an existing `ArConfig`.
        /// </summary>
        /// <param name="value">A pointer to an existing native `ArConfig`.</param>
        /// <returns>An instance whose native pointer is <paramref name="value"/>.</returns>
        public static ArConfig FromIntPtr(IntPtr value) => new ArConfig(value);

        /// <summary>
        /// Gets the native pointer for this instance.
        /// </summary>
        /// <returns>The native pointer.</returns>
        public IntPtr AsIntPtr() => m_Self;

        /// <summary>
        /// Casts an instance to its native pointer.
        /// </summary>
        /// <param name="config">The instance to cast.</param>
        /// <returns>The native pointer for <paramref name="config"/></returns>
        public static explicit operator IntPtr(ArConfig config) => config.AsIntPtr();

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// Two instances are considered equal if their native pointers are equal.
        /// </remarks>
        /// <param name="other">The instance to compare against.</param>
        /// <returns><see langword="true"/> if the native pointers are equal. Otherwise, returns <see langword="false"/>.</returns>
        public bool Equals(ArConfig other) => m_Self == other.m_Self;

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">An `object` to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an `ArConfig` and it compares
        /// equal to this instance using <see cref="Equals(UnityEngine.XR.ARCore.ArConfig)"/>.</returns>
        public override bool Equals(object obj) => obj is ArConfig other && Equals(other);

        /// <summary>
        /// Generates a hash code suitable for use with a `HashSet` or `Dictionary`
        /// </summary>
        /// <returns>Returns a hash code for this instance.</returns>
        public override int GetHashCode() => m_Self.GetHashCode();

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(UnityEngine.XR.ARCore.ArConfig)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(UnityEngine.XR.ARCore.ArConfig)"/>. Otherwise, returns <see langword="false"/>.</returns>
        public static bool operator ==(ArConfig lhs, ArConfig rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to the negation of <see cref="Equals(UnityEngine.XR.ARCore.ArConfig)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(UnityEngine.XR.ARCore.ArConfig)"/>. Otherwise, returns <see langword="true"/>.</returns>
        public static bool operator !=(ArConfig lhs, ArConfig rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Destroys this instance and sets its native pointer to <see langword="null"/>.
        /// </summary>
        /// <remarks>
        /// You should only dispose an `ArConfig` if you explicitly created it, e.g., by calling
        /// <see cref="ArConfig(ArSession)"/>. If you convert an existing config from an
        /// <see cref="IntPtr"/> (e.g., by calling <see cref="FromIntPtr"/>), then you should not dispose it.
        /// </remarks>
        public void Dispose()
        {
            if (m_Self != IntPtr.Zero)
            {
                Destroy(this);
            }

            m_Self = IntPtr.Zero;;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// This equality operator lets you compare an <see cref="ArConfig"/> with `null` to determine whether its
        /// native pointer is null.
        /// </remarks>
        /// <example>
        /// <code>
        /// bool TestForNull(ArConfig obj)
        /// {
        ///     if (obj == null)
        ///     {
        ///         // obj.AsIntPtr() is IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="lhs">The nullable `ArConfig` to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable `ArConfig` to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not `null` and their native pointers are equal.
        /// - <paramref name="lhs"/> is `null` and <paramref name="rhs"/>'s native pointer is `null`.
        /// - <paramref name="rhs"/> is `null` and <paramref name="lhs"/>'s native pointer is `null`.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are `null`.
        ///
        /// Otherwise, returns <see langword="false"/>.
        /// </returns>
        public static bool operator ==(ArConfig? lhs, ArConfig? rhs) => NativeObject.ArePointersEqual(lhs?.m_Self, rhs?.m_Self);

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <remarks>
        /// This inequality operator lets you compare an <see cref="ArConfig"/> with `null` to determine whether its
        /// native pointer is null.
        /// </remarks>
        /// <example>
        /// <code>
        /// bool TestForNull(ArConfig obj)
        /// {
        ///     if (obj != null)
        ///     {
        ///         // obj.AsIntPtr() is not IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="lhs">The nullable `ArConfig` to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable `ArConfig` to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not `null` and their native pointers are equal.
        /// - <paramref name="lhs"/> is `null` and <paramref name="rhs"/>'s native pointer is `null`.
        /// - <paramref name="rhs"/> is `null` and <paramref name="lhs"/>'s native pointer is `null`.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are `null`.
        ///
        /// Otherwise, returns <see langword="true"/>.
        /// </returns>
        public static bool operator !=(ArConfig? lhs, ArConfig? rhs) => !(lhs == rhs);

#if UNITY_EDITOR
        static void Create(ArSession session, out ArConfig configOut) => configOut = default;

        static void Destroy(ArConfig config) { }
#elif UNITY_ANDROID
        [DllImport("arcore_sdk_c", EntryPoint = "ArConfig_create")]
        static extern void Create(ArSession session, out ArConfig configOut);

        [DllImport("arcore_sdk_c", EntryPoint = "ArConfig_destroy")]
        static extern void Destroy(ArConfig config);
#endif
    }
}
