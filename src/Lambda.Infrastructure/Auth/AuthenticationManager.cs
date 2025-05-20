using System.Net.Http.Json;
using HsaLedger.Application.Requests.Identity;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Lambda.Infrastructure.Models;
using HsaLedger.Shared.Common.Constants.HttpClient;
using HsaLedger.Shared.Common.Extensions;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Lambda.Infrastructure.Auth;

public class AuthenticationManager : IAuthenticationManager
{
    private readonly HttpClient _httpClient;
    private readonly TokenHolder _tokenHolder;

    private const string LoginEndpoint = "api/Identity/login";
    private const string RefreshEndpoint = "api/Identity/refresh";

    public AuthenticationManager(IHttpClientFactory factory, TokenHolder tokenHolder)
    {
        _httpClient = factory.CreateClient(HttpClientConstants.AuthHttpClientName);
        _tokenHolder = tokenHolder;
    }

    public async Task<IResult> Login(LoginRequest loginRequest)
    {
        var response = await _httpClient.PostAsJsonAsync(LoginEndpoint, loginRequest);
        var result = await response.ToResult<AuthResponse>();
        if (result.Succeeded)
        {
            var token = result.Data?.AccessToken;
            var refreshToken = result.Data?.RefreshToken;
            var expiresIn = result.Data?.ExpiresIn;
            var expirationUtc = DateTime.UtcNow.AddSeconds(expiresIn ?? 0);

            _tokenHolder.Token = token;
            _tokenHolder.RefreshToken = refreshToken;
            _tokenHolder.TokenExpiration = expirationUtc;

            return await Result.SuccessAsync();
        }

        if (result.Messages == null)
        {
            return await Result.FailAsync();
        }
        
        return await Result.FailAsync(result.Messages);
    }
    
    public async Task<IResult> RefreshToken()
    {
        var refreshToken = _tokenHolder.RefreshToken;
        var username = _tokenHolder.Username;

        var response = await _httpClient.PostAsJsonAsync(RefreshEndpoint,
            new RefreshRequest { Username = username!, RefreshToken = refreshToken! });

        var result = await response.ToResult<AuthResponse>();

        if (result.Succeeded)
        {
            var token = result.Data?.AccessToken;
            refreshToken = result.Data?.RefreshToken;
            var expiresIn = result.Data?.ExpiresIn;
            var expirationUtc = DateTime.UtcNow.AddSeconds(expiresIn ?? 0);
        
            _tokenHolder.Token = token;
            _tokenHolder.RefreshToken = refreshToken;
            _tokenHolder.TokenExpiration = expirationUtc;

            return await Result.SuccessAsync();
        }
        
        if (result.Messages == null)
        {
            return await Result.FailAsync();
        }
        
        return await Result.FailAsync(result.Messages);
    }
}