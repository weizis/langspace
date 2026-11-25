using System.Security.Cryptography;
using System.Text;
using LangSpace.Models;
using Microsoft.AspNetCore.Identity;

namespace LangSpace.Service;

internal static class Verification
{
    private static readonly PasswordHasher<User> Hasher = new();

    public static string HashPassword(string password)
        => Hasher.HashPassword(new User(), password);

    public static PasswordVerificationResult VerifyPassword(User user, string password)
    {
        var hash = user.PasswordHash;
        if (string.IsNullOrWhiteSpace(hash))
            return PasswordVerificationResult.Failed;

        if (hash.Length == 128 && hash.All(Uri.IsHexDigit))
        {
            var actual = SHA512.HashData(Encoding.UTF8.GetBytes(password));
            return CryptographicOperations.FixedTimeEquals(actual, Convert.FromHexString(hash))
                ? PasswordVerificationResult.SuccessRehashNeeded
                : PasswordVerificationResult.Failed;
        }

        try
        {
            return Hasher.VerifyHashedPassword(user, hash, password);
        }
        catch (FormatException)
        {
            return PasswordVerificationResult.Failed;
        }
    }

    public static string GetSHA512Hash(string input) => HashPassword(input);

    public static bool VerifySHA512Hash(string input, string hash)
        => VerifyPassword(new User { PasswordHash = hash }, input) != PasswordVerificationResult.Failed;
}
