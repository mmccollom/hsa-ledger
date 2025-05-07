using System.Net.Http.Headers;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using Microsoft.AspNetCore.Components;
using Toolbelt.Blazor;

namespace HsaLedger.Client.Infrastructure.Managers;

public class HttpInterceptorManager : IHttpInterceptorManager
{
    private readonly HttpClientInterceptor _interceptor;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly NavigationManager _navigationManager;

        public HttpInterceptorManager(HttpClientInterceptor interceptor, IAuthenticationManager authenticationManager,
            NavigationManager navigationManager)
        {
            _interceptor = interceptor;
            _authenticationManager = authenticationManager;
            _navigationManager = navigationManager;
        }

        public void RegisterEvent() => _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;

        public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            var absPath = e.Request?.RequestUri?.AbsolutePath;

            if (absPath == null) return;
            
            if (!absPath.Contains("token") && !absPath.Contains("accounts"))
            {
                try
                {
                    var token = await _authenticationManager.TryRefreshToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        e.Request!.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await _authenticationManager.Logout();
                    _navigationManager.NavigateTo("/");
                }
            }
        }

        public void DisposeEvent() => _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
}