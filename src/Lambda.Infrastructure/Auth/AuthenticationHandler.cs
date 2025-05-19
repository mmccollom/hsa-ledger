using System.Net.Http.Headers;
using HsaLedger.Lambda.Infrastructure.Models;

namespace HsaLedger.Lambda.Infrastructure.Auth;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly TokenHolder _tokenHolder;
    private readonly IAuthenticationManager _authenticationManager;

    public AuthenticationHandler(TokenHolder tokenHolder, IAuthenticationManager authenticationManager)
    {
        _tokenHolder = tokenHolder;
        _authenticationManager = authenticationManager;
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

        // Check if token needs ot be refreshed, if so refresh it
        if (_tokenHolder.TokenExpiration != null && _tokenHolder.TokenExpiration < DateTime.UtcNow)
        {
            await _authenticationManager.RefreshToken();
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
}