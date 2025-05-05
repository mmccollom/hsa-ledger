using HsaLedger.Application.Requests;
using HsaLedger.Server.Identity;
using HsaLedger.Server.Infrastructure.Identity;
using HsaLedger.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace HsaLedger.Server.Controllers;

public class IdentityController : ApiControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly LinkGenerator _linkGenerator;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public IdentityController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        LinkGenerator linkGenerator, JwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _linkGenerator = linkGenerator;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<Result<string>> Register(RegisterRequest request)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return await Result<string>.FailAsync(result.Errors.Select(e => e.Description).ToList());

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmUrl = _linkGenerator.GetUriByAction(
            HttpContext,
            action: nameof(ConfirmEmail),
            controller: "Identity",
            values: new { userId = user.Id, token });

        Console.WriteLine(confirmUrl);
        // Send `confirmUrl` via email here...

        return await Result<string>.SuccessAsync("User registered. Confirmation email sent.");
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("confirm")]
    public async Task<Result<string>> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return await Result<string>.FailAsync("User not found.");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return await Result<string>.FailAsync(result.Errors.Select(e => e.Description).ToList());

        return await Result<string>.SuccessAsync("Email confirmed.");
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public async Task<Result<IdentityRequests.AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Email);
        if (user == null)
            return await Result<IdentityRequests.AuthResponse>.FailAsync("Invalid login.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return await Result<IdentityRequests.AuthResponse>.FailAsync("Invalid login.");

        var tokenResult = await _jwtTokenGenerator.GenerateTokenAsync(user);
        return await Result<IdentityRequests.AuthResponse>.SuccessAsync(tokenResult);
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<Result<IdentityRequests.AuthResponse>> Refresh(IdentityRequests.RefreshRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return await Result<IdentityRequests.AuthResponse>.FailAsync("User not found");

        var storedToken = await _jwtTokenGenerator.GetStoredRefreshTokenAsync(user);
        if (storedToken != request.RefreshToken)
            return await Result<IdentityRequests.AuthResponse>.FailAsync("Invalid refresh token");

        var tokenResponse = await _jwtTokenGenerator.GenerateTokenAsync(user);
        return await Result<IdentityRequests.AuthResponse>.SuccessAsync(tokenResponse);
    }

    [HttpPost]
    [Route("changePassword")]
    public async Task<Result<string>> ChangePassword(IdentityRequests.ChangePasswordRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return await Result<string>.FailAsync("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
            return await Result<string>.FailAsync(result.Errors.Select(e => e.Description).ToList());

        return await Result<string>.SuccessAsync("Password changed.");
    }

    [HttpPost]
    [Route("setRoles")]
    [Authorize(Roles = "Administrator")]
    public async Task<Result<string>> SetRoles(IdentityRequests.SetRolesRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
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