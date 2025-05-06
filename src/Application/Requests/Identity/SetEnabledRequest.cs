namespace HsaLedger.Application.Requests.Identity;

public class SetEnabledRequest
{
    public required string Username { get; set; }
    public required bool IsEnabled { get; set; }
}