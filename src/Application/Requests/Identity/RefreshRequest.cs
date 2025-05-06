namespace HsaLedger.Application.Requests.Identity;

public class RefreshRequest
{
    public required string Username { get; set; }
    public required string RefreshToken { get; set; }
}