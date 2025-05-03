using System.Runtime.InteropServices;
using sibber.WindowMessageMonitor.Native.Windowing;

namespace sibber.WindowMessageMonitor.Native;

public static partial class Helpers
{
    internal static class Windowing
    {
        public static unsafe HWnd CreateMessageWindow(string className, string? windowName, Handle instance)
        {
            return PInvoke.Windowing.CreateWindowEx(0, className, windowName, 0, 0, 0, 0, 0, Macros.HWND_MESSAGE, Handle.Null, instance);
        }

        public static Handle GetCurrentHINSTANCE()
        {
#if NETSTANDARD2_0
            return PInvoke.Misc.GetModuleHandle(typeof(Handle).Assembly.Location);
#else
            return (Handle)Marshal.GetHINSTANCE(typeof(Handle).Module);
#endif
        }
    }
}
