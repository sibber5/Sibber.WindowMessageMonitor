using System.Runtime.InteropServices;
using sibber.Common.Native.Windows;

namespace sibber.WindowMessageMonitor.Native.Windowing;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal readonly struct WNDCLASSW(
    uint style,
    WndProc wndProc,
    int clsExtra,
    int wndExtra,
    Handle instance,
    Handle icon,
    Handle cursor,
    Handle brBackground,
    string? menuName,
    string? className
)
{
    public readonly uint Style = style;

    public readonly WndProc WndProc = wndProc;

    public readonly int ClsExtra = clsExtra;

    public readonly int WndExtra = wndExtra;

    public readonly Handle Instance = instance;

    public readonly Handle Icon = icon;

    public readonly Handle Cursor = cursor;

    public readonly Handle BrBackground = brBackground;

    [MarshalAs(UnmanagedType.LPWStr)]
    public readonly string? MenuName = menuName;

    [MarshalAs(UnmanagedType.LPWStr)]
    public readonly string? ClassName = className;
}
