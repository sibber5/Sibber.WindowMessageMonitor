using sibber.Common.Native.Windows.Windowing;

namespace sibber.WindowMessageMonitor;

/// <summary>
/// Event arguments for the <see cref="WindowMessageMonitor.WindowMessageReceived"/> event.
/// </summary>
public readonly record struct WindowMessageEventArgs
{
    internal WindowMessageEventArgs(HWnd hWnd, uint messageId, nuint wParam, nint lParam)
    {
        Message = new(hWnd, messageId, wParam, lParam, default, default);
    }

    /// <summary>
    /// The result after processing the message. Use this to set the return result, after also setting <see cref="Handled"/> to <see langword="true"/>.
    /// </summary>
    /// <seealso cref="Handled"/>
    public nint Result { readonly get; init; }

    /// <summary>
    /// Indicates whether this message was handled and the <see cref="Result"/> value should be returned.
    /// </summary>
    /// <remarks><see langword="true"/> is the message was handled and the <see cref="Result"/> should be returned, otherwise <see langword="false"/> and continue processing this message by other subsclasses.</remarks>
    /// <seealso cref="Result"/>
    public bool Handled { readonly get; init; }

    /// <summary>
    /// The Windows WM Message
    /// </summary>
    public readonly MSG Message { get; }

    public readonly WindowMessage MessageType => (WindowMessage)Message.MessageId;
}
