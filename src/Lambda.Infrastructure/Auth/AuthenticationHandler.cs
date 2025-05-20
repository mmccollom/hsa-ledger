using System.Net.Http.Headers;
using HsaLedger.Application.Requests.Identity;
using HsaLedger.Lambda.Infrastructure.Models;
using HsaLedger.Lambda.Infrastructure.Services;

namespace HsaLedger.Lambda.Infrastructure.Auth;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly TokenHolder _tokenHolder;
    private readonly IAuthenticationManager _authenticationManager;
    private readonly SecretInfo _info;

    public AuthenticationHandler(TokenHolder tokenHolder, IAuthenticationManager authenticationManager, SecretInfo info)
    {
        _tokenHolder = tokenHolder;
        _authenticationManager = authenticationManager;
        _info = info;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var absPath = request.RequestUri?.AbsolutePath;

        if (absPath == null)
        {
            throw new ArgumentNullException(nameof(absPath));
        }

        if (string.IsNullOrEmpty(_tokenHolder.Token))
        {
            var secrets = await GetServiceSecrets();
            var result = await _authenticationManager.Login(new LoginRequest { Username = secrets.Username!, Password = secrets.Password!});
            if (!result.Succeeded)
            {
                throw new Exception("Failed to login: " + result.Messages?.FirstOrDefault());
            }
        }

        // Check if token needs ot be refreshed, if so refresh it
        if (_tokenHolder.TokenExpiration != null && _tokenHolder.TokenExpiration < DateTime.UtcNow)
        {
            var result = await _authenticationManager.RefreshToken();
            if (!result.Succeeded)
            {
                throw new Exception("Failed to refresh token: " + result.Messages?.FirstOrDefault());
            }
        }
        
        if (request.Headers.Authorization?.Scheme != "Bearer")
        {
            var savedToken = _tokenHolder.Token;
     
            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
            }
        }
     
        return await base.SendAsync(request, cancellationToken);
    }
    
    private async Task<ServiceSecrets> GetServiceSecrets()
    {
        // get api credentials from AWS secrets
        var secrets = await AwsSecretService.AwsConfigurationFromSecret<ServiceSecrets>(_info.Name, _info.Region);

        if (secrets?.Username == null || secrets.Password == null)
        {
            throw new NullReferenceException("Null secret values");
        }
        
        return secrets;
    }
}