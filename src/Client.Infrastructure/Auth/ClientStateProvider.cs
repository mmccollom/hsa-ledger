using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.LocalStorage;
using HsaLedger.Application.Services;
using HsaLedger.Shared.Common.Constants.Storage;
using Microsoft.AspNetCore.Components.Authorization;

namespace HsaLedger.Client.Infrastructure.Auth;

public class ClientStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ClientStateProvider(
            HttpClient httpClient,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task StateChangedAsync()
        {
            var authState = Task.FromResult(await GetAuthenticationStateAsync());

            NotifyAuthenticationStateChanged(authState);

        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            NotifyAuthenticationStateChanged(authState);
        }

        public async Task<ClaimsPrincipal> AuthenticationStateProviderUserAsync()
        {
            var state = await GetAuthenticationStateAsync();
            var authenticationStateProviderUser = state.User;
            return authenticationStateProviderUser;
        }

        private ClaimsPrincipal? AuthenticationStateUser { get; set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
            var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtTokenService.ClaimsFromJwt(savedToken), "jwt")));
            AuthenticationStateUser = state.User;
            return state;
        }
    }