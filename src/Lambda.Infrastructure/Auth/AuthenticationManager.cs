using System.Net.Http.Json;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Application.Services;
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

    public async Task<Result<AuthResponse?>> Login(LoginRequest loginRequest)
    {
        var response = await _httpClient.PostAsJsonAsync(LoginEndpoint, loginRequest);
        var result = await response.ToResult<AuthResponse>();
        if (result.Succeeded)
        {
            var token = result.Data?.AccessToken;
            var refreshToken = result.Data?.RefreshToken;
            var expirationUtc = JwtTokenService.GetTokenExpiry(result.Data?.AccessToken ?? throw new Exception("Token is null"));

            _tokenHolder.Token = token;
            _tokenHolder.RefreshToken = refreshToken;
            _tokenHolder.TokenExpiration = expirationUtc;

            return await Result<AuthResponse?>.SuccessAsync(result.Data);
        }

        if (result.Messages == null)
        {
            return await Result<AuthResponse?>.FailAsync();
        }
        
        return await Result<AuthResponse?>.FailAsync(result.Messages);
    }
    
    public async Task<Result<AuthResponse?>> RefreshToken(string? refreshToken = null)
    {
        refreshToken ??= _tokenHolder.RefreshToken;

        var response = await _httpClient.PostAsJsonAsync(RefreshEndpoint,
            new RefreshRequest { RefreshToken = refreshToken! });

        var result = await response.ToResult<AuthResponse>();

        if (result.Succeeded)
        {
            var token = result.Data?.AccessToken;
            refreshToken = result.Data?.RefreshToken;
            var expirationUtc = JwtTokenService.GetTokenExpiry(result.Data?.AccessToken ?? throw new Exception("Token is null"));
        
            _tokenHolder.Token = token;
            _tokenHolder.RefreshToken = refreshToken;
            _tokenHolder.TokenExpiration = expirationUtc;

            return await Result<AuthResponse?>.SuccessAsync(result.Data);
        }
        
        if (result.Messages == null)
        {
            return await Result<AuthResponse?>.FailAsync();
        }
        
        return await Result<AuthResponse?>.FailAsync(result.Messages);
    }
}