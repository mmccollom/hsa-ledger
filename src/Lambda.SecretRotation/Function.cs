using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using HsaLedger.Application.Requests;
using HsaLedger.Lambda.Infrastructure;
using HsaLedger.Lambda.Infrastructure.Auth;
using HsaLedger.Lambda.Infrastructure.Models;
using HsaLedger.Lambda.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HsaLedger.Lambda.SecretRotation;

public class Function
{
    private readonly IAmazonSecretsManager _secretsManager;
    private readonly IAuthenticationManager _authenticationManager;
    private readonly SecretToRotate _secretToRotate;
    private readonly SecretInfo _secretInfo;
    
    public Function()
    {
        var services = new ServiceCollection();
        
        // Set up Configuration
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Needed for reading appsettings.json
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        var configuration = configurationBuilder.Build();
        
        // Get API URL from configuration
        var apiUrl = configuration["ApiUrl"] ?? throw new InvalidOperationException();
        
        // Parse "SecretInfo" from configuration as SecretInfo class
        var secretInfo = configuration.GetSection("SecretInfo").Get<SecretInfo>() ?? throw new InvalidOperationException();
        
        // Parse "SecretToRotate" from configuration as SecretToRotate class
        var secretToRotate = configuration.GetSection("SecretToRotate").Get<SecretToRotate>() ?? throw new InvalidOperationException();
        
        // Register secret info
        services.AddSingleton(secretInfo);
        
        // Register secret to rotate
        services.AddSingleton(secretToRotate);
        
        // Register configuration
        services.AddSingleton(configuration);
        
        // Register services
        services.AddInfrastructureServices(apiUrl);
        services.AddLambdaServices();
        
        // Build DI container
        var serviceProvider = services.BuildServiceProvider();

        // Intentionally eagerly build Function and its dependencies.
        // This causes time spent in the Constructor to appear on INIT_REPORTs
        _secretsManager = serviceProvider.GetRequiredService<IAmazonSecretsManager>();
        _authenticationManager = serviceProvider.GetRequiredService<IAuthenticationManager>();
        _secretToRotate = serviceProvider.GetRequiredService<SecretToRotate>();
        _secretInfo = serviceProvider.GetRequiredService<SecretInfo>();
    }

    public async Task FunctionHandler(ILambdaContext context)
    {
        // Rotate secret
        var tokenPair = await FetchNewTokenPair(context.Logger);

        if (tokenPair.AccessToken == string.Empty || tokenPair.RefreshToken == string.Empty)
        {
            return;
        }
        
        // Serialize and store in AWS Secrets Manager
        var newSecretValue = JsonSerializer.Serialize(tokenPair);

        var putRequest = new PutSecretValueRequest
        {
            SecretId = _secretToRotate.Name,
            SecretString = newSecretValue
        };

        try
        {
            var result = await _secretsManager.PutSecretValueAsync(putRequest);
            context.Logger.LogInformation("Successfully rotated secret: {0}", result.ARN);
        }
        catch (Exception ex)
        {
            context.Logger.LogError("Failed to update secret: {0}", ex.Message);
            throw;
        }
    }

    private async Task<TokenPair> FetchNewTokenPair(ILambdaLogger logger)
    {
        string accessToken;
        string refreshToken;
        
        var jwtSecret = await AwsSecretService.AwsConfigurationFromSecret<ServiceJwtSecrets>(_secretToRotate.Name, _secretToRotate.Region);

        if(!string.IsNullOrEmpty(jwtSecret?.RefreshToken))
        {
            // refresh token
            var result = await _authenticationManager.RefreshToken(jwtSecret.RefreshToken);
            if (result is { Succeeded: true, Data: { } token })
            {
                accessToken = token.AccessToken;
                refreshToken = token.RefreshToken;
            }
            else
            {
                if (result.Messages != null)
                {
                    foreach(var message in result.Messages)
                    {
                        logger.LogError(message);
                    }
                }
                else
                {
                    logger.LogError("Failed to refresh token");
                }
                return new TokenPair { AccessToken = string.Empty, RefreshToken = string.Empty };
            }
        }
        else
        {
            // get api credentials from AWS secrets
            var secrets = await AwsSecretService.AwsConfigurationFromSecret<ServiceSecrets>(_secretInfo.Name, _secretInfo.Region);

            if (secrets?.Username == null || secrets.Password == null)
            {
                logger.LogError("Failed to get username or password from AWS secrets");
                return new TokenPair { AccessToken = string.Empty, RefreshToken = string.Empty };
            }

            var result = await _authenticationManager.Login(new LoginRequest
            {
                Username = secrets.Username,
                Password = secrets.Password
            });
            
            if (result is { Succeeded: true, Data: { } token })
            {
                accessToken = token.AccessToken;
                refreshToken = token.RefreshToken;
            }
            else
            {
                if (result.Messages != null)
                {
                    foreach(var message in result.Messages)
                    {
                        logger.LogError(message);
                    }
                }
                else
                {
                    logger.LogError("Failed to login");   
                }
                return new TokenPair { AccessToken = string.Empty, RefreshToken = string.Empty };
            }
        }
        
        return new TokenPair { AccessToken = accessToken, RefreshToken = refreshToken };
    }
    
    private class TokenPair
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}