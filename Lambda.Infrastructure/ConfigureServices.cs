using System.Globalization;
using HsaLedger.Application.Requests.Identity;
using HsaLedger.Lambda.Infrastructure.Auth;
using HsaLedger.Lambda.Infrastructure.Models;
using HsaLedger.Lambda.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HsaLedger.Lambda.Infrastructure;

public static class ConfigureServices
{
    public static async Task<IServiceProvider> InitializeContainer(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationManager, AuthenticationManager>();
        services.AddTransient<AuthenticationHandler>();
        
        var secrets = await GetServiceSecrets();
        
        const string clientName = "HSA Ledger";
        services.AddHttpClient(clientName, client =>
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture
                ?.TwoLetterISOLanguageName);
            client.BaseAddress = new Uri(secrets.ApiUrl!);
        }).AddHttpMessageHandler<AuthenticationHandler>();
        
        var provider = services.BuildServiceProvider();
        var authManager = provider.GetRequiredService<IAuthenticationManager>();
        await authManager.Login(new LoginRequest { Username = secrets.Username!, Password = secrets.Password!});

        return provider;
    }
    
    private static async Task<ServiceSecrets> GetServiceSecrets()
    {
        // get api credentials from AWS secrets
        const string secretName = "service-secrets";
        const string region = "us-east-2";
        var secrets = await AwsSecretService.AwsConfigurationFromSecret<ServiceSecrets>(secretName, region);

        if (secrets?.Username == null || secrets.Password == null || secrets.ApiUrl == null)
        {
            throw new NullReferenceException("Null secret values");
        }
        
        return secrets;
    }
}