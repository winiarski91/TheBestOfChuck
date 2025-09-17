namespace TheBestOfChuck.Infrastructure.Utils;

using System.Security.Cryptography;
using System.Text;
using Domain.Interfaces;

public class HashService : IHashService
{
    public string GenerateHash(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentNullException(nameof(input));

        var normalizedText = input.ToLowerInvariant().Trim();
        var inputBytes = Encoding.UTF8.GetBytes(normalizedText);
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
