using System;
using Sibber.Common.Native.Windows.Windowing;

namespace Sibber.WindowMessageMonitor.Native;

internal static class Macros
{
    public static ushort LOWORD(uint l) => unchecked((ushort)(l & 0xFFFF));

    public static HWnd HWND_MESSAGE => unchecked((HWnd)(IntPtr)(-3));

    public const int E_FAIL = unchecked((int)0x80004005);
}
