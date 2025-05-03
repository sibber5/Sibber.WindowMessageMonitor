using sibber.Common.Native.Windows.Windowing;

namespace sibber.WindowMessageMonitor;

/// <summary>
/// Implementations of this interface monitors window messages sent to a specified window and notify subsribers when messages are recieved.
/// </summary>
public interface IWindowsMessageMonitor
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
