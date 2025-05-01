using System.Runtime.InteropServices;

namespace sibber.WindowMessageMonitor.Native;

internal static unsafe partial class PInvoke
{
    public static unsafe partial class Misc
    {
#if NETSTANDARD2_0
        [DllImport("KERNEL32.dll", EntryPoint = "GetModuleHandleW", ExactSpelling = true, CharSet = CharSet.Unicode)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern Handle GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);
#endif

#if NETSTANDARD
        [DllImport("KERNEL32.dll", EntryPoint = "SetLastError", ExactSpelling = true)]
        public static extern void SetLastError(uint dwErrCode);
#endif
    }
}
