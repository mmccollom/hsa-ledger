namespace HsaLedger.Lambda.Infrastructure.Models;

public class SecretToRotate
{
    public required string Region { get; set; }
    public required string Name { get; set; }
}