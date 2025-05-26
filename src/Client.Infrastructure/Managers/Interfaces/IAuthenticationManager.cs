using System.Security.Claims;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface IAuthenticationManager : IManager
{

    Task<IResult> ChangePassword(ChangePasswordRequest changePasswordRequest);
    
    Task<IResult> Login(LoginRequest loginRequest);
    
    Task<IResult> Logout();

    Task<string> RefreshToken();
    
    Task<string?> GetValidToken();

    Task<ClaimsPrincipal> CurrentUser();
    Task<IResult<IEnumerable<UserResponse>>> GetUsers();
    Task<IResult<IEnumerable<RoleResponse>>> GetRoles();
    Task<IResult> Register(RegisterRequest registerRequest);
    Task<IResult> SetRoles(SetRolesRequest setRolesRequest);
    Task<IResult> SetEnabled(SetEnabledRequest setEnabledRequest);
}