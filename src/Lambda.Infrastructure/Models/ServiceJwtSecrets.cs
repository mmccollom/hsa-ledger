namespace HsaLedger.Lambda.Infrastructure.Models;

public class ServiceJwtSecrets
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}