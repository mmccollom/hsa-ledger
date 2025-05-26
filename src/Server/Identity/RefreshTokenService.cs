using System.Security.Cryptography;
using System.Text;

namespace HsaLedger.Server.Identity;

internal static class RefreshTokenService
{
    internal static (string hash, string salt) Hash(string token)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(32);
        var salt = Convert.ToBase64String(saltBytes);

        using var hmac = new HMACSHA256(saltBytes);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        var hash = Convert.ToBase64String(hashBytes);

        return (hash, salt);
    }

    internal static bool Verify(string token, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using var hmac = new HMACSHA256(saltBytes);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(computedHash) == hash;
    }
}