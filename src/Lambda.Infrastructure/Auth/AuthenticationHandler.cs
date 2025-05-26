using System.Net.Http.Headers;
using HsaLedger.Application.Requests;
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
            _tokenHolder.Token = secrets.AccessToken;
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
    
    private async Task<ServiceJwtSecrets> GetServiceSecrets()
    {
        // get api credentials from AWS secrets
        var secrets = await AwsSecretService.AwsConfigurationFromSecret<ServiceJwtSecrets>(_info.Name, _info.Region);

        if (secrets?.AccessToken == null)
        {
            throw new NullReferenceException("Null secret values");
        }
        
        return secrets;
    }
}