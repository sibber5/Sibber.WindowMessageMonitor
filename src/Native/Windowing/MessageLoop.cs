using System.ComponentModel;

namespace sibber.WindowMessageMonitor.Native.Windowing;

/// <summary>
/// Use to run a message loop if needed.
/// </summary>
public static class MessageLoop
{
    public static unsafe int Run()
    {
        MSG msg = new();
        int ret;
        while (true)
        {
            ret = PInvoke.Windowing.GetMessage(&msg, HWnd.Null, 0, 0);
            if (ret == 0) break;

            if (ret == -1) throw new Win32Exception();

            PInvoke.Windowing.TranslateMessage(&msg);
            PInvoke.Windowing.DispatchMessage(&msg);
        }

        return ret;
    }
}
