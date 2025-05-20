using System.Globalization;
using HsaLedger.Application.Requests.Identity;
using HsaLedger.Lambda.Infrastructure.Auth;
using HsaLedger.Lambda.Infrastructure.Models;
using HsaLedger.Lambda.Infrastructure.Services;
using HsaLedger.Shared.Common.Constants.HttpClient;
using Microsoft.Extensions.DependencyInjection;

namespace HsaLedger.Lambda.Infrastructure;

public static class ConfigureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, string url)
    {
        services.AddHttpClient(HttpClientConstants.DefaultClientName, client =>
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture
                ?.TwoLetterISOLanguageName);
            client.BaseAddress = new Uri(url);
        }).AddHttpMessageHandler<AuthenticationHandler>();
        
        services.AddHttpClient(HttpClientConstants.AuthHttpClientName,client =>
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture
                ?.TwoLetterISOLanguageName);
            client.BaseAddress = new Uri(url);
        });
        
        // Provide the configured named client as the default HttpClient
        services.AddScoped(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            return factory.CreateClient(HttpClientConstants.DefaultClientName);
        });

        var tokenHolder = new TokenHolder();
        services.AddSingleton(tokenHolder);
        services.AddScoped<IAuthenticationManager, AuthenticationManager>();
        services.AddTransient<AuthenticationHandler>();
    }
}