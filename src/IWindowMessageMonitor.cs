// SPDX-License-Identifier: MIT
// Copyright (c) 2025 sibber (GitHub: sibber5)

using Sibber.Common.Native.Windows.Windowing;

namespace Sibber.WindowMessageMonitor;

/// <summary>
/// Implementations of this interface monitors window messages sent to a specified window and notify subsribers when messages are recieved.
/// </summary>
public interface IWindowMessageMonitor
{
    /// <summary>
    /// The handle of the window that is being monitored.
    /// </summary>
    public HWnd HWnd { get; }

    /// <summary>
    /// Raised when a windows message is received.
    /// </summary>
    public event RefEventHandler<WindowMessageEventArgs> WindowMessageReceived;
}
