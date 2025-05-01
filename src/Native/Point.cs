using System;
using System.Runtime.InteropServices;

namespace sibber.WindowMessageMonitor.Native;

/// <summary>The <a href="https://learn.microsoft.com/windows/win32/api/windef/ns-windef-point">POINT</a> structure defines the <i>x</i>- and <i>y</i>-coordinates of a point.</summary>
/// <remarks>
/// <para>The <a href="https://learn.microsoft.com/windows/win32/api/windef/ns-windef-point">POINT</a> structure is identical to the <a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-pointl">POINTL</a> structure.</para>
/// <para><see href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-point">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Point(int x, int y) : IEquatable<Point>
{
    /// <summary>Specifies the <i>x</i>-coordinate of the point.</summary>
    private readonly int X = x;

    /// <summary>Specifies the <i>y</i>-coordinate of the point.</summary>
    private readonly int Y = y;

    public static explicit operator System.Drawing.Point(Point p) => new(p.X, p.Y);

    public static explicit operator Point(System.Drawing.Point p) => new(p.X, p.Y);

    public static bool operator !=(Point left, Point right) => !(left == right);

    public static bool operator ==(Point left, Point right) => left.Equals(right);

    public readonly override int GetHashCode() => HashCode.Combine(X, Y);

    public readonly override bool Equals(object? obj) => obj is Point point && Equals(point);

    public readonly bool Equals(Point other) => X == other.X && Y == other.Y;

    public readonly void Deconstruct(out int X, out int Y)
    {
        X = this.X;
        Y = this.Y;
    }

    public readonly override string ToString() => $"Point {{ {nameof(X)} = {X}, {nameof(Y)} = {Y} }}";
}
