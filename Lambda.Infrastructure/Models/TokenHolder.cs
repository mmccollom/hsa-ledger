namespace HsaLedger.Lambda.Infrastructure.Models;

public class TokenHolder
{
    public string? Username { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpiration { get; set; }
}