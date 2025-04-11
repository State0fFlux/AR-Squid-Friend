using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Event arguments for the <see cref="ARCoreSessionSubsystem.beforeSetConfiguration"/> event.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ARCoreBeforeSetConfigurationEventArgs : IEquatable<ARCoreBeforeSetConfigurationEventArgs>
    {
        ArSession m_Session;
        ArConfig m_Config;

        /// <summary>
        /// Gets the native ARCore session whose corresponding configuration object will be set.
        /// </summary>
        /// <value>The current session object.</value>
        public ArSession arSession => m_Session;

        /// <summary>
        /// Gets the native ARCore configuration that will be set.
        /// </summary>
        /// <remarks>
        /// Refer to Google's [ArConfig](https://developers.google.com/ar/reference/c/group/config#arconfig) documentation
        /// for more information.
        /// </remarks>
        /// <value>The native ARCore configuration</value>
        public ArConfig arConfig => m_Config;

        /// <summary>
        /// Constructs an <see cref="ARCoreBeforeSetConfigurationEventArgs"/>.
        /// </summary>
        /// <param name="session">The ARCore session whose corresponding configuration object will be set.</param>
        /// <param name="config">The ARCore session configuration which will be set.</param>
        public ARCoreBeforeSetConfigurationEventArgs(ArSession session, ArConfig config)
        {
            m_Session = session;
            m_Config = config;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other instance to compare against.</param>
        /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this
        /// instance. Otherwise, returns <see langword="false"/>.</returns>
        public bool Equals(ARCoreBeforeSetConfigurationEventArgs other) =>
            m_Session.Equals(other.m_Session) &&
            m_Config.Equals(other.m_Config);

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode() => HashCodeUtil.Combine(m_Session.GetHashCode(), m_Config.GetHashCode());

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is of type `ARCoreBeforeSetConfigurationEventArgs` and
        /// <see cref="Equals(ARCoreBeforeSetConfigurationEventArgs)"/> also returns <see langword="true"/>.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is ARCoreBeforeSetConfigurationEventArgs other && Equals(other);

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(ARCoreBeforeSetConfigurationEventArgs)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public static bool operator ==(ARCoreBeforeSetConfigurationEventArgs lhs, ARCoreBeforeSetConfigurationEventArgs rhs) => lhs.Equals(rhs);

         /// <summary>
         /// Tests for inequality. Equivalent to `!`<see cref="Equals(ARCoreBeforeSetConfigurationEventArgs)"/>.
         /// </summary>
         /// <param name="lhs">The left-hand side of the comparison.</param>
         /// <param name="rhs">The right-hand side of the comparison.</param>
         /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>.
         /// Otherwise, returns <see langword="false"/>.</returns>
        public static bool operator !=(ARCoreBeforeSetConfigurationEventArgs lhs, ARCoreBeforeSetConfigurationEventArgs rhs) => !lhs.Equals(rhs);
    }
}
