using System.Security.Claims;

namespace HsaLedger.Client.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    private const string UsernameIdentifier = "username";
    private static string? FindFirstValue(this ClaimsPrincipal user, string claimType)
    {
        return user.FindFirst(claimType)?.Value;
    }
    
    public static string? UserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static string? Username(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(UsernameIdentifier);
    }

    public static string? UserEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email);
    }

    public static string? UserRole(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Role);
    }

    public static string? GivenName(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.GivenName);
    }

    public static string? Surname(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Surname);
    }

    public static string? PhoneNumber(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.MobilePhone);
    }
}
