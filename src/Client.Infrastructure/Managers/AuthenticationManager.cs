using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Application.Services;
using HsaLedger.Client.Infrastructure.Auth;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using HsaLedger.Client.Infrastructure.Managers.Routes;
using HsaLedger.Shared.Common.Constants.Permission;
using HsaLedger.Shared.Common.Constants.Storage;
using HsaLedger.Shared.Common.Extensions;
using HsaLedger.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Authorization;

namespace HsaLedger.Client.Infrastructure.Managers;

public class AuthenticationManager : IAuthenticationManager
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthenticationManager(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<ClaimsPrincipal> CurrentUser()
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User;
    }
    
    public async Task<IResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
    {
        var response = await _httpClient.PostAsJsonAsync(IdentityEndpoints.ChangePassword, changePasswordRequest);
        var result = await response.ToResult<string>();
        return result;
    }

    public async Task<IResult> Login(LoginRequest loginRequest)
    {
        var response = await _httpClient.PostAsJsonAsync(IdentityEndpoints.Login, loginRequest);
        var result = await response.ToResult<AuthResponse>();
        if (result.Succeeded)
        {
            var token = result.Data?.AccessToken;
            var refreshToken = result.Data?.RefreshToken;
            var expirationUtc = JwtTokenService.GetTokenExpiry(result.Data?.AccessToken ?? throw new Exception("Token is null"));
            
            // validate role
            var claims = JwtTokenService.ClaimsFromJwt(token!);
            var role = claims.Where(x => x.Type == ApplicationClaimTypes.Role).Select(x=> x.Value).FirstOrDefault();
            if (role == null || (!role.Contains(ApplicationRoles.Administrator) && !role.Contains(ApplicationRoles.Operations)))
            {
                return await Result.FailAsync("You are not authorized to use this application.");
            }
            
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthTokenExpiration, expirationUtc);
            await _localStorage.RemoveItemAsync(StorageConstants.Local.ChangeTemporaryPasswordGuid);


            await ((ClientStateProvider)_authenticationStateProvider).StateChangedAsync();

            return await Result.SuccessAsync();
        }

        if (result.Messages == null)
        {
            return await Result.FailAsync();
        }
        if (!result.Messages[0].Equals("Update your temporary password"))
        {
            return await Result.FailAsync(result.Messages);
        }
        
        var guid = Guid.NewGuid().ToString();
        await _localStorage.SetItemAsync(StorageConstants.Local.ChangeTemporaryPasswordGuid, guid);
        result.Messages.Add(guid);
        return await Result.FailAsync(result.Messages);
    }

    public async Task<IResult> Logout()
    {
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthTokenExpiration);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.ChangeTemporaryPasswordGuid);
        ((ClientStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        return await Result.SuccessAsync();
    }
    
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);
    public async Task<string> RefreshToken()
    {
        await RefreshLock.WaitAsync();
        try
        {
            var currentToken = await GetValidToken();
            if (currentToken != null)
            {
                return currentToken;
            }
            
            var refreshToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
            var response = await _httpClient.PostAsJsonAsync(IdentityEndpoints.Refresh,
                new RefreshRequest { RefreshToken = refreshToken! });

            var result = await response.ToResult<AuthResponse>();

            if (!result.Succeeded || string.IsNullOrEmpty(result.Data?.AccessToken))
            {
                if (result.Messages != null)
                {
                    var joinedMessages = string.Join(Environment.NewLine, result.Messages);
                    throw new ApplicationException(joinedMessages);
                }
                else
                {
                    throw new ApplicationException("Something went wrong during the refresh token action");    
                }
            }

            var token = result.Data?.AccessToken;
            refreshToken = result.Data?.RefreshToken;
            var expirationUtc = JwtTokenService.GetTokenExpiry(result.Data?.AccessToken ?? throw new Exception("Token is null"));
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthTokenExpiration, expirationUtc);
        
            return token!;
        }
        finally
        {
            RefreshLock.Release();
        }
    }

    public async Task<string?> GetValidToken()
    {
        //check if token exists
        var availableToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
        if (string.IsNullOrEmpty(availableToken))
        {
            return null;
        }
        
        var expiry = await _localStorage.GetItemAsync<DateTime>(StorageConstants.Local.AuthTokenExpiration);
        var utcNow = DateTime.UtcNow;
        var diff = expiry - utcNow;
        if (diff.TotalMinutes <= 1)
        {
            return null;
        }
        
        var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
        return token ?? null;
    }
    
    public async Task<IResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var response = await _httpClient.GetAsync(IdentityEndpoints.GetUsers);
        var result = await response.ToResult<IEnumerable<UserResponse>>();
        return result;
    }

    public async Task<IResult<IEnumerable<RoleResponse>>> GetRoles()
    {
        var response = await _httpClient.GetAsync(IdentityEndpoints.GetRoles);
        var result = await response.ToResult<IEnumerable<RoleResponse>>();
        return result;
    }
    
    public async Task<IResult> Register(RegisterRequest registerRequest)
    {
        var response = await _httpClient.PostAsJsonAsync(IdentityEndpoints.Register, registerRequest);
        var result = await response.ToResult<string>();
        return result;
    }
    
    public async Task<IResult> SetRoles(SetRolesRequest setRolesRequest)
    {
        var response = await _httpClient.PostAsJsonAsync(IdentityEndpoints.SetRoles, setRolesRequest);
        var result = await response.ToResult<string>();
        return result;
    }
    
    public async Task<IResult> SetEnabled(SetEnabledRequest setEnabledRequest)
    {
        var response = await _httpClient.PostAsJsonAsync(IdentityEndpoints.SetEnabled, setEnabledRequest);
        var result = await response.ToResult<string>();
        return result;
    }
}