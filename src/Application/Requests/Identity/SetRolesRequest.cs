namespace HsaLedger.Application.Requests.Identity;

public class SetRolesRequest
{
    public required string Username { get; set; }
    public List<string> Roles { get; set; } = [];
}