using HsaLedger.Application.Requests.Identity;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Server.Identity;
using HsaLedger.Server.Infrastructure.Identity;
using HsaLedger.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HsaLedger.Server.Controllers;

public class IdentityController : ApiControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public IdentityController(UserManager<User> userManager, SignInManager<User> signInManager,JwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<Result<string>> Register(RegisterRequest request)
    {
        var user = new User
        {
            UserName = request.Username,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return await Result<string>.FailAsync(result.Errors.Select(e => e.Description).ToList());
        
        /*
        // TODO: Send `confirmUrl` via email here...

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmUrl = _linkGenerator.GetUriByAction(
            HttpContext,
            action: nameof(ConfirmEmail),
            controller: "Identity",
            values: new { userId = user.Id, token });

        Console.WriteLine(confirmUrl);
        */

        return await Result<string>.SuccessAsync("User registered.");
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public async Task<Result<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user is not { IsEnabled: true })
            return await Result<AuthResponse>.FailAsync("Invalid login.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return await Result<AuthResponse>.FailAsync("Invalid login.");

        var tokenResult = await _jwtTokenGenerator.GenerateTokenAsync(user);
        return await Result<AuthResponse>.SuccessAsync(tokenResult);
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<Result<AuthResponse>> Refresh(RefreshRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Username);
        if (user == null)
            return await Result<AuthResponse>.FailAsync("User not found");

        var storedToken = await _jwtTokenGenerator.GetStoredRefreshTokenAsync(user);
        if (storedToken != request.RefreshToken)
            return await Result<AuthResponse>.FailAsync("Invalid refresh token");

        var tokenResponse = await _jwtTokenGenerator.GenerateTokenAsync(user);
        return await Result<AuthResponse>.SuccessAsync(tokenResponse);
    }

    [HttpPost]
    [Route("changePassword")]
    public async Task<Result<string>> ChangePassword(ChangePasswordRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return await Result<string>.FailAsync("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        if (!result.Succeeded)
            return await Result<string>.FailAsync(result.Errors.Select(e => e.Description).ToList());

        return await Result<string>.SuccessAsync("Password changed.");
    }

    [HttpPost]
    [Route("setEnabled")]
    //[Authorize(Roles = "Administrator")]
    [Authorize]
    public async Task<Result<string>> SetRoles(SetEnabledRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) return await Result<string>.FailAsync("User not found.");

        var existingRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
        if (!removeResult.Succeeded)
            return await Result<string>.FailAsync(removeResult.Errors.Select(e => e.Description).ToList());

        user.IsEnabled = request.IsEnabled;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return await Result<string>.FailAsync(updateResult.Errors.Select(e => e.Description).ToList());
        
        return await Result<string>.SuccessAsync("IsEnabled flag updated.");
    }

    [HttpPost]
    [Route("setRoles")]
    [Authorize(Roles = "Administrator")]
    public async Task<Result<string>> SetRoles(SetRolesRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) return await Result<string>.FailAsync("User not found.");

        var existingRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
        if (!removeResult.Succeeded)
            return await Result<string>.FailAsync(removeResult.Errors.Select(e => e.Description).ToList());

        var addResult = await _userManager.AddToRolesAsync(user, request.Roles);
        if (!addResult.Succeeded)
            return await Result<string>.FailAsync(addResult.Errors.Select(e => e.Description).ToList());

        return await Result<string>.SuccessAsync("Roles updated.");
    }
}