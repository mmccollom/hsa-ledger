namespace HsaLedger.Server.Identity.Model;

public class JwtConfigurationModel
{
    public required string Key { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpiresInMinutes { get; set; }
    public int RefreshExpiresInDays { get; set; }
}