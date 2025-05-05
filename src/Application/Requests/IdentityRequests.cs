namespace HsaLedger.Application.Requests;

public abstract class IdentityRequests
{
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

    public record SetRolesRequest(string UserId, List<string> Roles);
    public record RefreshRequest(string Email,  string RefreshToken);

    public class AuthResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
    }

}