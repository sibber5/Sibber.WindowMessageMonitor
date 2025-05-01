using System;
using System.ComponentModel;
using sibber.WindowMessageMonitor.Native;
using sibber.WindowMessageMonitor.Native.Windowing;

namespace sibber.WindowMessageMonitor;

public sealed partial class WindowMessageMonitor
{
    private readonly WndProc? _windowProc;
    private readonly string? _windowClassName;
    private readonly Handle _instance;

    private WindowMessageMonitor()
    {
        _windowProc = new(WindowProc);

        string windowName = $"sibber.WindowMessageMonitor Message Window {Guid.NewGuid().ToBase64()}";
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

        HWnd = Helpers.Windowing.CreateMessageWindow(_windowClassName, windowName, _instance);
        //PInvoke.Windowing.ShowWindow(HWnd, 1);

        _classId = _classIdCounter++;

        if (HWnd == HWnd.Null) throw new Win32Exception();
    }

    public static WindowMessageMonitor CreateWithMessageWindow() => new();

    private nint WindowProc(HWnd hWnd, uint uMsg, nuint wParam, nint lParam)
    {
        var args = new WindowMessageEventArgs(hWnd, uMsg, wParam, lParam);
        _windowMessageReceived?.Invoke(this, ref args);

        return args.Handled ? args.Result : PInvoke.Windowing.DefWindowProc(hWnd, uMsg, wParam, lParam);
    }
}
