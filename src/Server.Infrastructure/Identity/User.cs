using HsaLedger.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HsaLedger.Server.Infrastructure.Identity;

public class User : IdentityUser
{
    public bool IsEnabled { get; set; }
    
    public virtual ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
}