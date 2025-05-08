using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Server.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HsaLedger.Server.Identity;

public class JwtTokenGenerator
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _config;

    public JwtTokenGenerator(UserManager<User> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<AuthResponse> GenerateTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["Jwt:ExpiresInMinutes"]));

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );
        
        // Generate a secure refresh token
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        // Store the refresh token
        await _userManager.SetAuthenticationTokenAsync(user, "HsaLedger", "refresh_token", refreshToken);

        return new AuthResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresIn = (int)(expires - DateTime.UtcNow).TotalSeconds,
            RefreshToken = refreshToken
        };
    }
    
    public async Task<string?> GetStoredRefreshTokenAsync(User user)
    {
        return await _userManager.GetAuthenticationTokenAsync(user, "HsaLedger", "refresh_token");
    }
}
