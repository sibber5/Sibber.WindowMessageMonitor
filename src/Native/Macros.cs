using sibber.WindowMessageMonitor.Native.Windowing;

namespace sibber.WindowMessageMonitor.Native;

internal static class Macros
{
    public static ushort LOWORD(uint l) => unchecked((ushort)(l & 0xFFFF));

    public static readonly HWnd HWND_MESSAGE = unchecked((HWnd)(nint)(-3));
}
