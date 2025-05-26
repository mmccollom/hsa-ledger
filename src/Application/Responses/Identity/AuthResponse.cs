namespace HsaLedger.Application.Responses.Identity;

public class AuthResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}