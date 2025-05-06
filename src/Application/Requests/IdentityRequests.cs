namespace HsaLedger.Application.Requests;

public abstract class IdentityRequests
{
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

    public record SetRolesRequest(string Email, List<string> Roles);
    public record RefreshRequest(string Email,  string RefreshToken);
    public record SetEnabledRequest(string Email, bool IsEnabled);

    public class AuthResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
    }

}