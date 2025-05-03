using sibber.Common.Native.Windows.Windowing;

namespace sibber.WindowMessageMonitor.Native;

internal static class Macros
{
    public static ushort LOWORD(uint l) => unchecked((ushort)(l & 0xFFFF));

    public static readonly HWnd HWND_MESSAGE = unchecked((HWnd)(nint)(-3));

    public const int E_FAIL = unchecked((int)0x80004005);
}
