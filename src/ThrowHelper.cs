using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace sibber.WindowMessageMonitor;

internal static class ThrowHelper
{
    /// <summary>Throws an <see cref="ObjectDisposedException"/> if the specified <paramref name="condition"/> is <see langword="true"/>.</summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="instance">The object whose type's full name should be included in any resulting <see cref="ObjectDisposedException"/>.</param>
    /// <exception cref="ObjectDisposedException">The <paramref name="condition"/> is <see langword="true"/>.</exception>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
    public static void ThrowIfDisposed([DoesNotReturnIf(true)] bool condition, object instance)
#else
    public static void ThrowIfDisposed(bool condition, object instance)
#endif
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(condition, instance);
#else
        if (condition)
        {
            throw new ObjectDisposedException(instance.GetType().FullName);
        }
#endif
    }

#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    public static void ThrowUnreachable()
    {
#if NETSTANDARD
        throw new Exception("Unreachable code reached.");
#else
        throw new UnreachableException();
#endif
    }
}
