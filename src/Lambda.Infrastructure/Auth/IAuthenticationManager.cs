using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Lambda.Infrastructure.Auth;

public interface IAuthenticationManager
{
    Task<IResult> Login(LoginRequest loginRequest);

    Task<IResult> RefreshToken();
}