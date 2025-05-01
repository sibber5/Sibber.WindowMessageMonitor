using System;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace sibber.WindowMessageMonitor;

internal static class GuidExtensions
{
    public static string ToBase64(this Guid id)
    {
        Span<byte> idBytes = stackalloc byte[16];
        Span<byte> base64Bytes = stackalloc byte[24];

#if NETCOREAPP2_1_OR_GREATER
        MemoryMarshal.TryWrite(idBytes, in id);
#else
        MemoryMarshal.TryWrite(idBytes, ref id);
#endif
        var status = Base64.EncodeToUtf8(idBytes, base64Bytes, out _, out _);
        Debug.Assert(status == OperationStatus.Done);

        Span<char> chars = stackalloc char[22];

        for (int i = 0; i < 22; i++)
        {
            chars[i] = base64Bytes[i] switch
            {
                (byte)'+' => '-',
                (byte)'/' => '_',
                _ => (char)base64Bytes[i]
            };
        }

#if NETCOREAPP2_1_OR_GREATER
        return new string(chars);
#else
        return new string(chars.ToArray());
#endif
    }

    public static Guid FromBase64(ReadOnlySpan<char> base64Id)
    {
        if (base64Id.Length != 22 && (base64Id.Length != 24 || base64Id[22] != '=' || base64Id[23] != '=')) throw new ArgumentException("Invalid base64 string");

        Span<char> base64Chars = stackalloc char[24];

        for (int i = 0; i < 22; i++)
        {
            base64Chars[i] = base64Id[i] switch
            {
                '-' => '+',
                '_' => '/',
                _ => base64Id[i]
            };
        }

        base64Chars[22] = '=';
        base64Chars[23] = '=';

#if NETCOREAPP2_1_OR_GREATER
        Span<byte> idBytes = stackalloc byte[16];
        bool success = Convert.TryFromBase64Chars(base64Chars, idBytes, out _);
        Debug.Assert(success);
#else
        byte[] idBytes = Convert.FromBase64CharArray(base64Chars.ToArray(), 0, base64Chars.Length);
#endif

        return new Guid(idBytes);
    }
}
