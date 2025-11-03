using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public static class JoinCodeUtility
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static bool TryGetLocalIPv4(out string address)
    {
        try
        {
            foreach (var entry in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (entry.AddressFamily != AddressFamily.InterNetwork)
                {
                    continue;
                }

                if (IPAddress.IsLoopback(entry))
                {
                    continue;
                }

                address = entry.ToString();
                return true;
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogWarning($"Failed to determine local IPv4 address: {ex.Message}");
        }

        address = string.Empty;
        return false;
    }

    public static string GenerateJoinCode(string address, ushort port)
    {
        if (!IPAddress.TryParse(address, out var ipAddress))
        {
            throw new ArgumentException("Invalid IPv4 address provided", nameof(address));
        }

        if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
        {
            throw new ArgumentException("Only IPv4 addresses are supported", nameof(address));
        }

        Span<byte> payload = stackalloc byte[6];
        var addressBytes = ipAddress.GetAddressBytes();
        addressBytes.AsSpan().CopyTo(payload);
        payload[4] = (byte)(port >> 8);
        payload[5] = (byte)(port & 0xFF);

        return Encode(payload);
    }

    public static bool TryParseJoinCode(string joinCode, out string address, out ushort port)
    {
        address = string.Empty;
        port = 0;

        if (!TryDecode(joinCode, out var bytes) || bytes.Length != 6)
        {
            return false;
        }

        address = string.Join(".", bytes[0], bytes[1], bytes[2], bytes[3]);
        port = (ushort)((bytes[4] << 8) | bytes[5]);
        return true;
    }

    public static bool IsValidCharacter(char value)
    {
        return Alphabet.IndexOf(char.ToUpperInvariant(value)) >= 0;
    }

    private static string Encode(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
        {
            return string.Empty;
        }

        int outputLength = (data.Length * 8 + 4) / 5;
        var result = new StringBuilder(outputLength);

        int buffer = data[0];
        int next = 1;
        int bitsLeft = 8;

        while (result.Length < outputLength)
        {
            if (bitsLeft < 5)
            {
                if (next < data.Length)
                {
                    buffer <<= 8;
                    buffer |= data[next++] & 0xFF;
                    bitsLeft += 8;
                }
                else
                {
                    int padding = 5 - bitsLeft;
                    buffer <<= padding;
                    bitsLeft += padding;
                }
            }

            int index = (buffer >> (bitsLeft - 5)) & 0x1F;
            bitsLeft -= 5;
            result.Append(Alphabet[index]);
        }

        return result.ToString();
    }

    private static bool TryDecode(string encoded, out byte[] data)
    {
        data = Array.Empty<byte>();

        if (string.IsNullOrWhiteSpace(encoded))
        {
            return false;
        }

        string sanitized = encoded.ToUpperInvariant();
        var builder = new StringBuilder(sanitized.Length);
        foreach (char value in sanitized)
        {
            if (!char.IsWhiteSpace(value) && value != '-')
            {
                builder.Append(value);
            }
        }

        string cleaned = builder.ToString();
        int byteCount = cleaned.Length * 5 / 8;
        if (byteCount == 0)
        {
            return false;
        }

        var result = new byte[byteCount];
        int buffer = 0;
        int bitsLeft = 0;
        int index = 0;

        foreach (char character in cleaned)
        {
            int value = Alphabet.IndexOf(character);
            if (value < 0)
            {
                return false;
            }

            buffer = (buffer << 5) | value;
            bitsLeft += 5;

            if (bitsLeft >= 8)
            {
                bitsLeft -= 8;
                if (index >= result.Length)
                {
                    Array.Resize(ref result, result.Length + 1);
                }

                result[index++] = (byte)((buffer >> bitsLeft) & 0xFF);
            }
        }

        if (index != result.Length)
        {
            Array.Resize(ref result, index);
        }

        data = result;
        return true;
    }
}

