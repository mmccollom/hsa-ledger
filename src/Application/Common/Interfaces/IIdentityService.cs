using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<string>> Register(RegisterRequest request);
    Task<Result<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> Refresh(RefreshRequest request, CancellationToken cancellationToken = default);
    Task<Result<string>> ChangePassword(ChangePasswordRequest request, string userId);
    Task<Result<string>> SetEnabled(SetEnabledRequest request);
    Task<Result<string>> SetRoles(SetRolesRequest request);
}