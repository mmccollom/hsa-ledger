using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Lambda.Infrastructure.Auth;

public interface IAuthenticationManager
{
    Task<Result<AuthResponse?>> Login(LoginRequest loginRequest);

    Task<Result<AuthResponse?>> RefreshToken(string? refreshToken = null);
}