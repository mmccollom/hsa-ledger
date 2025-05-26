using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Domain.Entities;
using HsaLedger.Server.Identity.Model;
using HsaLedger.Server.Infrastructure.Identity;
using HsaLedger.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HsaLedger.Server.Identity;

public class IdentityCommands
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtConfigurationModel _jwtConfiguration;
    private readonly IApplicationDbContext _context;


    public IdentityCommands(UserManager<User> userManager, SignInManager<User> signInManager, JwtConfigurationModel jwtConfiguration, IApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtConfiguration = jwtConfiguration;
        _context = context;
    }

    internal async Task<Result<string>> Register(RegisterRequest request)
    {
        var user = new User
        {
            UserName = request.Username,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return await Result<string>.FailAsync(result.Errors.Select(e => e.Description).ToList());
        }

        return await Result<string>.SuccessAsync("User registered.");
    }
    
    internal async Task<Result<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user is not { IsEnabled: true })
        {
            return await Result<AuthResponse>.FailAsync("Invalid login.");
        }

        var checkPasswordResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!checkPasswordResult.Succeeded)
        {
            return await Result<AuthResponse>.FailAsync("Invalid login.");
        }
        // Credentials are valid at this point
        
        // Generate JWT token
        var jwtToken = await GenerateJwtToken(user);
        // Generate refresh token
        var refreshToken = await GenerateRefreshToken(user.Id, null, null, cancellationToken); // TODO: Implement device and ip address
        
        var authResponse = new AuthResponse
        {
            AccessToken = jwtToken,
            RefreshToken = refreshToken
        };
        return await Result<AuthResponse>.SuccessAsync(authResponse);
    }
    
    internal async Task<Result<AuthResponse>> Refresh(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        // Validate and get refresh token data from database
        var refreshTokenData = await GetRefreshDataAsync(request.RefreshToken, cancellationToken);
        if (refreshTokenData == null)
        {
            return await Result<AuthResponse>.FailAsync("Invalid");
        }
        
        // Revoke existing refresh token
        await RevokeRefreshToken(refreshTokenData.UserRefreshTokenId, cancellationToken);
        
        // Lookup user using UserId
        var user = await _userManager.FindByIdAsync(refreshTokenData.UserId);
        
        // Generate JWT token
        var jwtToken = await GenerateJwtToken(user!);
        // Generate refresh token
        var refreshToken = await GenerateRefreshToken(user!.Id, null, null, cancellationToken); // TODO: Implement device and ip address
        
        var authResponse = new AuthResponse
        {
            AccessToken = jwtToken,
            RefreshToken = refreshToken
        };
        return await Result<AuthResponse>.SuccessAsync(authResponse);
    }
    
    internal async Task<Result<string>> ChangePassword(ChangePasswordRequest request, ClaimsPrincipal principalUser)
    {
        var user = await _userManager.GetUserAsync(principalUser);
        if (user == null) return await Result<string>.FailAsync("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        if (!result.Succeeded)
        {
            return await Result<string>.FailAsync(result.Errors.Select(e => e.Description).ToList());
        }

        return await Result<string>.SuccessAsync("Password changed.");
    }
    
    internal async Task<Result<string>> SetEnabled(SetEnabledRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) return await Result<string>.FailAsync("User not found.");

        var existingRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
        if (!removeResult.Succeeded)
        {
            return await Result<string>.FailAsync(removeResult.Errors.Select(e => e.Description).ToList());
        }

        user.IsEnabled = request.IsEnabled;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return await Result<string>.FailAsync(updateResult.Errors.Select(e => e.Description).ToList());
        }
        
        return await Result<string>.SuccessAsync("IsEnabled flag updated.");
    }
    
    internal async Task<Result<string>> SetRoles(SetRolesRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) return await Result<string>.FailAsync("User not found.");

        var existingRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
        if (!removeResult.Succeeded)
        {
            return await Result<string>.FailAsync(removeResult.Errors.Select(e => e.Description).ToList());
        }

        var addResult = await _userManager.AddToRolesAsync(user, request.Roles);
        if (!addResult.Succeeded)
        {
            return await Result<string>.FailAsync(addResult.Errors.Select(e => e.Description).ToList());
        }

        return await Result<string>.SuccessAsync("Roles updated.");
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Name, user.UserName ?? ""),
            new (JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(_jwtConfiguration.ExpiresInMinutes);

        var token = new JwtSecurityToken(_jwtConfiguration.Issuer,_jwtConfiguration.Audience,claims, expires: expires,
            signingCredentials: signingCredentials);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        return jwtToken;
    }
    
    private async Task<string> GenerateRefreshToken(string userId, string? device, string? ipAddress, CancellationToken cancellationToken = default)
    {
        // Generate a secure refresh token
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        // Hash refresh token
        var tokenHash = RefreshTokenService.Hash(refreshToken);
        
        // Persist refresh token
        var userRefreshToken = new UserRefreshToken
        {
            UserId = userId,
            Hash = tokenHash.hash,
            Salt = tokenHash.salt,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfiguration.RefreshExpiresInDays),
            Device = device,
            IpAddress = ipAddress
        };

        await _context.UserRefreshTokens.AddAsync(userRefreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return refreshToken;
    }
    
    private async Task<UserRefreshTokenResponse?> GetRefreshDataAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var userRefreshTokens = await _context
            .UserRefreshTokens
            .Where(x => x.IsRevoked == false)
            .Where(x => x.ExpiresAt > DateTime.UtcNow)
            .Select(UserRefreshTokenResponse.Projection)
            .ToListAsync(cancellationToken: cancellationToken);

        var userRefreshToken = userRefreshTokens.FirstOrDefault(x => RefreshTokenService.Verify(refreshToken, x.Hash, x.Salt));
        return userRefreshToken;
    }

    private async Task RevokeRefreshToken(int userRefreshTokenId, CancellationToken cancellationToken = default)
    {
        var userRefreshToken = await _context.UserRefreshTokens.FirstAsync(x => x.UserRefreshTokenId == userRefreshTokenId, cancellationToken);
        userRefreshToken.IsRevoked = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}