using System.Globalization;
using Blazored.LocalStorage;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Client.Infrastructure.Auth;
using HsaLedger.Client.Infrastructure.Managers;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using HsaLedger.Client.Infrastructure.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;

namespace HsaLedger.Client;

internal static class ConfigureServices
{
    internal static IServiceCollection AddClientServices(this IServiceCollection services, string url)
    {
        const string clientName = "HSA Ledger";
        
        services.AddAuthorizationCore(options =>
        {
            //RegisterPermissionClaims(options);
        });
        services.AddBlazoredLocalStorage();

        services.AddScoped<ClientStateProvider>();
        services.AddScoped<AuthenticationStateProvider, ClientStateProvider>();
        services.AddManagers();
        services.AddTransient<AuthenticationHeaderHandler>();
        services.AddScoped(sp => sp
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient(clientName).EnableIntercept(sp));
        
        services.AddHttpClient(clientName, client =>
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture
                ?.TwoLetterISOLanguageName);
            client.BaseAddress = new Uri(url);
        }).AddHttpMessageHandler<AuthenticationHeaderHandler>();
        
        services.AddHttpClientInterceptor();

        // Add client services
        services.AddScoped<IClientExcelService, ClientExcelService>();
        
        return services;
    }

    private static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped<IClientPreferenceManager, ClientPreferenceManager>();
        var managers = typeof(IManager);

        var types = managers
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Service = t.GetInterface($"I{t.Name}"),
                Implementation = t
            })
            .Where(t => t.Service != null);

        foreach (var type in types)
        {
            if (managers.IsAssignableFrom(type.Service))
            {
                services.AddTransient(type.Service, type.Implementation);
            }
        }

        return services;
    }
}