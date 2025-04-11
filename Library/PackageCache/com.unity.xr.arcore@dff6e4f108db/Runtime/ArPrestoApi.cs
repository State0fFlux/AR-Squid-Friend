using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARCore
{
    internal static class ArPrestoApi
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport(Constants.k_LibraryName)]
        public static extern void ArPresto_update();
#else
        public static void ArPresto_update() {}
#endif
    }
}
