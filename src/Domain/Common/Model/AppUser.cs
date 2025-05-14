namespace HsaLedger.Domain.Common.Model;

public class AppUser
{
    public required string UserId { get; set; }
    public required string Username { get; set; }
    public required bool IsEnabled { get; set; }
    public required List<AppRole> Roles { get; set; }
}