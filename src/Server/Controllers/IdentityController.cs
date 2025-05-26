using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Server.Identity;
using HsaLedger.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HsaLedger.Server.Controllers;

public class IdentityController : ApiControllerBase
{
    private readonly IdentityQueries _identityQueries;
    private readonly IdentityCommands _identityCommands;

    public IdentityController(IdentityQueries identityQueries, IdentityCommands identityCommands)
    {
        _identityQueries = identityQueries;
        _identityCommands = identityCommands;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<Result<string>> Register(RegisterRequest request)
    {
        return await _identityCommands.Register(request);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public async Task<Result<AuthResponse>> Login(LoginRequest request)
    {
        return await _identityCommands.Login(request);
    }

    [HttpPost]
    [Route("refresh")]
    [AllowAnonymous]
    public async Task<Result<AuthResponse>> Refresh(RefreshRequest request)
    {
        return await _identityCommands.Refresh(request);
    }

    [HttpPost]
    [Route("changePassword")]
    [Authorize]
    public async Task<Result<string>> ChangePassword(ChangePasswordRequest request)
    {
        return await _identityCommands.ChangePassword(request, User);
    }

    [HttpPost]
    [Route("setEnabled")]
    [Authorize(Roles = "Administrator")]
    public async Task<Result<string>> SetEnabled(SetEnabledRequest request)
    {
        return await _identityCommands.SetEnabled(request);
    }

    [HttpPost]
    [Route("setRoles")]
    [Authorize(Roles = "Administrator")]
    public async Task<Result<string>> SetRoles(SetRolesRequest request)
    {
        return await _identityCommands.SetRoles(request);
    }
    
    [HttpGet]
    [Route("users")]
    [Authorize(Roles = "Administrator")]
    public async Task<Result<IEnumerable<UserResponse>>> GetUsers()
    {
        return await _identityQueries.GetUsersAsync();
    }
    
    [HttpGet]
    [Route("roles")]
    [Authorize(Roles = "Administrator")]
    public async Task<Result<IEnumerable<RoleResponse>>> GetRoles()
    {
        return await _identityQueries.GetRolesAsync();
    }

}