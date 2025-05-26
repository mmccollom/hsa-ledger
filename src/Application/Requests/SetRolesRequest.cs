namespace HsaLedger.Application.Requests;

public class SetRolesRequest
{
    public required string Username { get; set; }
    public List<string> Roles { get; set; } = [];
}