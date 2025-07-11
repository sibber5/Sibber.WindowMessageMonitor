﻿// SPDX-License-Identifier: MIT
// Copyright (c) 2025 sibber (GitHub: sibber5)
// Copyright (c) 2021 Morten Nielsen
// Modified version of https://github.com/dotMorten/WinUIEx/blob/c363a6d25b586701a7996dfa8622b42a3c3b5740/src/WinUIEx/Messaging/WindowMessageMonitor.cs
// From https://github.com/dotMorten/WinUIEx by dotMorten (Morten Nielsen)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Sibber.Common.Native.Windows.Windowing;
using Sibber.WindowMessageMonitor.Native;

namespace Sibber.WindowMessageMonitor;

#if DOCFX
/// <summary>
/// Monitors window messages sent to the specified window and notifies subscribers when messages are received.
/// </summary>
/// <remarks>
/// <![CDATA[
/// > [!CAUTION]
/// > You cannot subscribe to a monitor for an existing window across threads.
/// ]]>
/// <![CDATA[
/// > [!CAUTION]
/// > If the instance was created with [CreateWithMessageOnlyWindow()](xref:Sibber.WindowMessageMonitor.WindowMessageMonitor.CreateWithMessageOnlyWindow) then it must be disposed on the same thread it was created on and in the same executing assembly.
/// ]]>
/// <![CDATA[
/// > [!THREADUNSAFE]
/// > Disposing is not thread-safe.
/// ]]>
/// <![CDATA[
/// > [!THREADSAFE]
/// > Constructing instances and subscribing to and unsubscribing from events are thread-safe.
/// ]]>
/// </remarks>
#else
/// <summary>
/// Monitors window messages sent to the specified window and notifies subscribers when messages are received.
/// </summary>
/// <remarks>
/// <para><b>WARNING:</b> You cannot subscribe to a monitor for an existing window across threads.</para>
/// <para>If the instance was created with <see cref="CreateWithMessageOnlyWindow"/> then it must be disposed on the same thread it was created on and in the same executing assembly.</para>
/// <para>Disposing is not thread-safe.</para>
/// <para>Constructing instances and subscribing to and unsubscribing from events are thread-safe.</para>
/// </remarks>
#endif
public sealed partial class WindowMessageMonitor : IWindowMessageMonitor, IDisposable
{
    public HWnd HWnd { get; }

    private GCHandle? _monitorGCHandle;
    private readonly object _lockObject = new();
#if !NET7_0_OR_GREATER
    private PInvoke.Windowing.SUBCLASSPROC? _callback;
#endif

    private static long MaxSubclassId => Environment.Is64BitProcess ? long.MaxValue : uint.MaxValue;
    private static long _subclassIdCounter = 100;
    private readonly UIntPtr _subclassId;

    private bool _disposed;

    /// <summary>
    /// Initialize a new instance of the <see cref="WindowMessageMonitor"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor is thread-safe.
    /// </remarks>
    /// <param name="hWnd">The window handle to listen to messages for.</param>
    public WindowMessageMonitor(HWnd hWnd)
    {
        HWnd = hWnd;

        var id = Interlocked.Increment(ref _subclassIdCounter);
        if (id < 0 || id > MaxSubclassId)
        {
            Interlocked.Decrement(ref _subclassIdCounter);
            throw new InvalidOperationException("Ran out of IDs.");
        }

        _subclassId = new((ulong)id);
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

    /// <summary>
    /// <b>WARNING:</b> You must subscribe to this event on the same thread as the window that this instance monitors.
    /// </summary>
    /// <remarks>
    /// Subscribing to and unsubscribing from this event is thread-safe.
    /// </remarks>
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
    {
        var refData = (nint)dwRefData;
#else
    private unsafe IntPtr SubclassWindowProc(HWnd hWnd, uint uMsg, UIntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, UIntPtr dwRefData)
    {
        var refData = (IntPtr)dwRefData.ToPointer();
#endif
        var handle = GCHandle.FromIntPtr(refData);
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
            bool ok = PInvoke.Windowing.SetWindowSubclass(HWnd, &SubclassWindowProc, _subclassId, (nuint)GCHandle.ToIntPtr(_monitorGCHandle.Value).ToPointer());
#else
            _callback = new(SubclassWindowProc);
            bool ok = PInvoke.Windowing.SetWindowSubclass(HWnd, _callback, _subclassId, (UIntPtr)GCHandle.ToIntPtr(_monitorGCHandle.Value).ToPointer());
#endif
            // pass error code 0 because there is error code for generic failure.
            if (!ok) throw new Win32Exception(0, "Error setting window subclass.\nThe operation did not complete successfully despite the native error code being 0 (ERROR_SUCCESS).");
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
            bool ok = PInvoke.Windowing.RemoveWindowSubclass(HWnd, &SubclassWindowProc, _subclassId);
#else
            bool ok = PInvoke.Windowing.RemoveWindowSubclass(HWnd, _callback!, _subclassId);
#endif
            if (!disposing && !ok) throw new Win32Exception(0, "Error removing window subclass.\nThe operation did not complete successfully despite the native error code being 0 (ERROR_SUCCESS).");

            _monitorGCHandle?.Free();
            _monitorGCHandle = null;
        }
    }
}
