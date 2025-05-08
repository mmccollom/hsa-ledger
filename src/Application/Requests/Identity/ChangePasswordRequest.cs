namespace HsaLedger.Application.Requests.Identity;

public class ChangePasswordRequest
{
    public required string Password { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmNewPassword { get; set; }
}