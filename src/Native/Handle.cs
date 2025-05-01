using System;
using System.Diagnostics;
using System.Globalization;

namespace sibber.WindowMessageMonitor.Native;

[DebuggerDisplay("{Value}")]
public readonly partial struct Handle(nint value) : IEquatable<Handle>, IFormattable
{
    public readonly nint Value = value;

    public static Handle Null => default;

    public static explicit operator Handle(nint value) => new(value);

    public static implicit operator nint(Handle value) => value.Value;

    public static bool operator ==(Handle left, Handle right) => left.Value == right.Value;
    public static bool operator !=(Handle left, Handle right) => !(left == right);
    public readonly bool Equals(Handle other) => Value == other.Value;
    public readonly override bool Equals(object? obj) => obj is Handle other && Equals(other);
    public readonly override int GetHashCode() => Value.GetHashCode();
    
    public readonly override string ToString() => $"0x{Value:X}";

    public readonly string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format)) return ToString();
        formatProvider ??= CultureInfo.CurrentCulture;

#if NET5_0_OR_GREATER
        return Value.ToString(format, formatProvider);
#else
        return Value.ToString(format);
#endif
    }
}
