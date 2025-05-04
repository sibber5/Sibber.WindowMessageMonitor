using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Sibber.Common.Native.Windows.Windowing;
using Sibber.WindowMessageMonitor.Native;

namespace Sibber.WindowMessageMonitor;

// From https://github.com/dotMorten/WinUIEx/blob/c363a6d25b586701a7996dfa8622b42a3c3b5740/src/WinUIEx/Messaging/WindowMessageMonitor.cs
// From https://github.com/dotMorten/WinUIEx by dotMorten (Morten Nielsen)
// 
// MIT License
// 
// Copyright (c) 2021 Morten Nielsen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

/// <summary>
/// Monitors window messages sent to the specified window and notifies subsribers when messages are recieved.
/// </summary>
/// <remarks>
/// If the instance was created with <see cref="CreateWithMessageOnlyWindow"/> then it must be disposed on the same thread it was created on and in the same executing assembly.<br/>
/// Disposing is not thread safe.<br/>
/// Subscribing and unsubscribing to the events is thread safe.
/// </remarks>
public sealed partial class WindowMessageMonitor : IWindowMessageMonitor, IDisposable
{
    public HWnd HWnd { get; }
    
    private GCHandle? _monitorGCHandle;
    private readonly object _lockObject = new();
#if !NET7_0_OR_GREATER
    private PInvoke.Windowing.SUBCLASSPROC? _callback;
#endif

    private static nuint _classIdCounter = 101;
    private readonly nuint _classId;

    private bool _disposed;

    /// <summary>
    /// Initialize a new instance of the <see cref="WindowMessageMonitor"/> class.
    /// </summary>
    /// <param name="hWnd">The window handle to listen to messages for.</param>
    public WindowMessageMonitor(HWnd hWnd)
    {
        HWnd = hWnd;
        _classId = _classIdCounter++;
    }

    ~WindowMessageMonitor()
    {
        Dispose(false);
    }

    /// <summary>
    /// If the instance was created with <see cref="CreateWithMessageOnlyWindow"/> then it must be disposed on the same thread it was created on and in the same executing assembly.<br/>
    /// Disposing is not thread safe.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (_windowMessageReceived is not null)
        {
            if (_windowClassName is null) RemoveWindowSubclass(true);
            _windowMessageReceived = null;
        }

        if (_windowClassName is not null)
        {
            if (!PInvoke.Windowing.DestroyWindow(HWnd)) throw new Win32Exception();
            if (!PInvoke.Windowing.UnregisterClass(_windowClassName, _instance)) throw new Win32Exception();
        }

        _disposed = true;
    }

    /// <inheritdoc cref="IWindowMessageMonitor.WindowMessageReceived"/>
    /// <exception cref="Win32Exception"></exception>
    public event RefEventHandler<WindowMessageEventArgs> WindowMessageReceived
    {
        add
        {
            ThrowHelper.ThrowIfDisposed(_disposed, this);

            if (_windowClassName is null && _windowMessageReceived is null) SetWindowSubclass();
            _windowMessageReceived += value;
        }
        remove
        {
            ThrowHelper.ThrowIfDisposed(_disposed, this);

            _windowMessageReceived -= value;
            if (_windowClassName is null && _windowMessageReceived is null) RemoveWindowSubclass();
        }
    }
    private event RefEventHandler<WindowMessageEventArgs>? _windowMessageReceived;


#if NET7_0_OR_GREATER
    [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
    private static nint SubclassWindowProc(HWnd hWnd, uint uMsg, nuint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData)
#else
    private nint SubclassWindowProc(HWnd hWnd, uint uMsg, nuint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData)
#endif
    {
        var handle = GCHandle.FromIntPtr((nint)dwRefData);
        if (handle is { IsAllocated: true, Target: WindowMessageMonitor monitor })
        {
            var eventHandler = monitor._windowMessageReceived;
            if (eventHandler != null)
            {
                var args = new WindowMessageEventArgs(hWnd, uMsg, wParam, lParam);
                eventHandler.Invoke(monitor, ref args);
                if (args.Handled) return args.Result;
            }
        }

        return PInvoke.Windowing.DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    private unsafe void SetWindowSubclass()
    {
        if (_monitorGCHandle.HasValue) return;

        lock (_lockObject)
        {
            ThrowHelper.ThrowIfDisposed(_disposed, this);
            Debug.Assert(_windowClassName is null);
            if (_monitorGCHandle.HasValue) return;
            
            _monitorGCHandle = GCHandle.Alloc(this);
#if NET7_0_OR_GREATER
            bool ok = PInvoke.Windowing.SetWindowSubclass(HWnd, &SubclassWindowProc, _classId, (nuint)GCHandle.ToIntPtr(_monitorGCHandle.Value).ToPointer());
#else
            _callback = new(SubclassWindowProc);
            bool ok = PInvoke.Windowing.SetWindowSubclass(HWnd, _callback, _classId, (nuint)GCHandle.ToIntPtr(_monitorGCHandle.Value).ToPointer());
#endif
            if (!ok) throw new Win32Exception(Macros.E_FAIL, "Error setting window subclass.");
        }
    }

    private unsafe void RemoveWindowSubclass(bool disposing = false)
    {
        if (!_monitorGCHandle.HasValue) return;

        lock (_lockObject)
        {
            if (!disposing) ThrowHelper.ThrowIfDisposed(_disposed, this);
            Debug.Assert(_windowClassName is null);
            if (!_monitorGCHandle.HasValue) return;

#if NET7_0_OR_GREATER
            bool ok = PInvoke.Windowing.RemoveWindowSubclass(HWnd, &SubclassWindowProc, _classId);
#else
            bool ok = PInvoke.Windowing.RemoveWindowSubclass(HWnd, _callback!, _classId);
#endif
            if (!disposing && !ok) throw new Win32Exception(Macros.E_FAIL, "Error removing window subclass.");

            _monitorGCHandle?.Free();
            _monitorGCHandle = null;
        }
    }
}
