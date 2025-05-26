using System.Security.Claims;
using System.Text;
using System.Text.Json;
using HsaLedger.Shared.Common.Constants.Permission;

namespace HsaLedger.Application.Services;

public static class JwtTokenService
{
    public static IEnumerable<Claim> ClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

            if (roles != null)
            {
                if (roles.ToString()!.Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);

                    if (parsedRoles != null)
                    {
                        claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out var permissions);
            if (permissions != null)
            {
                if (permissions.ToString()!.Trim().StartsWith("["))
                {
                    var parsedPermissions = JsonSerializer.Deserialize<string[]>(permissions.ToString()!);
                    if (parsedPermissions != null)
                    {
                        claims.AddRange(parsedPermissions.Select(permission =>
                            new Claim(ApplicationClaimTypes.Permission, permission)));
                    }
                }
                else
                {
                    claims.Add(new Claim(ApplicationClaimTypes.Permission, permissions.ToString()!));
                }

                keyValuePairs.Remove(ApplicationClaimTypes.Permission);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
        }

        return claims;
    }
    
    public static bool IsTokenExpired(string token)
    {
        var expiry = GetTokenExpiry(token);
        return !expiry.HasValue || expiry <= DateTime.UtcNow;
    }

    public static DateTime? GetTokenExpiry(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
                return null;

            var payload = parts[1];
            var payloadBytes = ParseBase64WithoutPadding(payload);
            var json = Encoding.UTF8.GetString(payloadBytes);
            var document = JsonDocument.Parse(json);

            if (!document.RootElement.TryGetProperty("exp", out var expElement))
                return null;

            var expSeconds = expElement.GetInt64();
            var expiration = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;

            return expiration;
        }
        catch
        {
            return null;
        }
    }

    private static byte[] ParseBase64WithoutPadding(string payload)
    {
        payload = payload.Trim().Replace('-', '+').Replace('_', '/');
        var base64 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }
}