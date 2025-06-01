using Amazon.SecretsManager;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HsaLedger.Lambda.SecretRotation;

internal static class ConfigureServices
{
    internal static void AddLambdaServices(this IServiceCollection services)
    {
        // Register AWS services
        services.AddAWSService<IAmazonSecretsManager>();
        
        // Add API Managers
        services.AddManagers();
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