using System.Security.Claims;
using HsaLedger.Application.Requests.Identity;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface IAuthenticationManager : IManager
{

    Task<IResult> ChangePassword(ChangePasswordRequest changePasswordRequest);
    
    Task<IResult> Login(LoginRequest loginRequest);
    
    Task<IResult> Logout();

    Task<string> RefreshToken();

    Task<string> TryRefreshToken();

    Task<string> TryForceRefreshToken();

    Task<ClaimsPrincipal> CurrentUser();
}