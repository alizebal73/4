using System.Security.Cryptography;
using GameNet.Application.Foundation;

namespace GameNet.Infrastructure.Security;

public sealed class SecureTokenGenerator : ITokenGenerator
{
    public string Generate()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("/", "_", StringComparison.Ordinal)
            .TrimEnd('=');
    }

    public string Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var hash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
