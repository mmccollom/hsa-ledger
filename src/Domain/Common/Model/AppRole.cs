namespace HsaLedger.Domain.Common.Model;

public class AppRole
{
    public required string RoleId { get; set; }
    public required string RoleName { get; set; }
    public required string NormalizedName { get; set; }
}