using Amazon.Lambda.Core;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HsaLedger.Lambda.EmailReader;

internal static class ConfigureServices
{
    internal static async Task<IServiceProvider> InitializeContainer(this ILambdaContext context)
    {
        var services = new ServiceCollection();
        services.AddSingleton(context.Logger);
        services.AddManagers();

        // add infrastructure services
        return await Infrastructure.ConfigureServices.InitializeContainer(services);
    }
    
    private static void AddManagers(this IServiceCollection services)
    {
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
    }
}