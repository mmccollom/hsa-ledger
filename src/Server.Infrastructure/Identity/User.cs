using Microsoft.AspNetCore.Identity;

namespace HsaLedger.Server.Infrastructure.Identity;

public class User : IdentityUser
{
    public bool IsEnabled { get; set; }
}