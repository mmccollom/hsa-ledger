using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

namespace HsaLedger.Lambda.Infrastructure.Services;

public static class AwsSecretService
{
    public static async Task<T?> AwsConfigurationFromSecret<T>(string secretName, string region)
    {
        var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
        };

        var response = await client.GetSecretValueAsync(request);
        return JsonConvert.DeserializeObject<T>(response.SecretString);
    }
}