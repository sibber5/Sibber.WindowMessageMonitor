using System;
using System.ComponentModel;
using Sibber.Common.Native.Windows;
using Sibber.Common.Native.Windows.Windowing;
using Sibber.WindowMessageMonitor.Native;
using Sibber.WindowMessageMonitor.Native.Windowing;
using Helpers = Sibber.WindowMessageMonitor.Native.Helpers;

namespace Sibber.WindowMessageMonitor;

public sealed partial class WindowMessageMonitor
{
    private readonly WndProc? _windowProc;
    private readonly string? _windowClassName;
    private readonly Handle _instance;

    private WindowMessageMonitor()
    {
        _windowProc = new(WindowProc);

        string windowName = $"{nameof(Sibber)}.{nameof(WindowMessageMonitor)} Message Window {Guid.NewGuid().ToBase64()}";
        _windowClassName = $"{windowName} Class";
        _instance = Helpers.Windowing.GetCurrentHINSTANCE();
        WNDCLASSW windowClass = new(
            default,
            _windowProc,
            default,
            default,
            _instance,
            default,
            default,
            default,
            default,
            _windowClassName
        );
        if (PInvoke.Windowing.RegisterClass(ref windowClass) == 0) throw new Win32Exception();

        HWnd = Helpers.Windowing.CreateMessageOnlyWindow(_windowClassName, windowName, _instance);

        _classId = _classIdCounter++;

        if (HWnd == HWnd.Null) throw new Win32Exception();
    }

    /// <summary>
    /// <para>Creates a window message monitor along with a message-only window (which it monitors).</para>
    /// <para>The monitor owns the window and ensures its disposal when the monitor is disposed. <b>The monitor must be disposed on the same thread it was created on and in the same executing assembly.</b></para>
    /// </summary>
    public static WindowMessageMonitor CreateWithMessageOnlyWindow() => new();

    private nint WindowProc(HWnd hWnd, uint uMsg, nuint wParam, nint lParam)
    {
        var args = new WindowMessageEventArgs(hWnd, uMsg, wParam, lParam);
        _windowMessageReceived?.Invoke(this, ref args);

        return args.Handled ? args.Result : PInvoke.Windowing.DefWindowProc(hWnd, uMsg, wParam, lParam);
    }
}
