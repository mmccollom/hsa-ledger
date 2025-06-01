using HsaLedger.Application.Mediator.Commands;
using HsaLedger.Application.Mediator.Queries;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HsaLedger.Server.Controllers;

public class IdentityController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<Result<string>> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request);
        var result = await Mediator.Send(command);
        return result;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public async Task<Result<AuthResponse>> Login(LoginRequest request)
    {
        var command = new LoginCommand(request);
        var result = await Mediator.Send(command);
        return result;
    }

    [HttpPost]
    [Route("refresh")]
    [AllowAnonymous]
    public async Task<Result<AuthResponse>> Refresh(RefreshRequest request)
    {
        var command = new RefreshCommand(request);
        var result = await Mediator.Send(command);
        return result;
    }

    [HttpPost]
    [Route("changePassword")]
    [Authorize]
    public async Task<Result<string>> ChangePassword(ChangePasswordRequest request)
    {
        // TODO get user ID
        var command = new ChangePasswordCommand(request, string.Empty);
        var result = await Mediator.Send(command);
        return result;
    }

    [HttpPost]
    [Route("setEnabled")]
    [Authorize(Roles = "Administrator")]
    public async Task<Result<string>> SetEnabled(SetEnabledRequest request)
    {
        var command = new SetEnabledCommand(request);
        var result = await Mediator.Send(command);
        return result;
    }

    [HttpPost]
    [Route("setRoles")]
    [Authorize(Roles = "Administrator")]
    public async Task<Result<string>> SetRoles(SetRolesRequest request)
    {
        var command = new SetRolesCommand(request);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpGet]
    [Route("users")]
    [Authorize(Roles = "Administrator")]
    public async Task<Result<IEnumerable<UserResponse>>> GetUsers()
    {
        var command = new GetAppUsersQuery();
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpGet]
    [Route("roles")]
    [Authorize(Roles = "Administrator")]
    public async Task<Result<IEnumerable<RoleResponse>>> GetRoles()
    {
        var command = new GetAppRolesQuery();
        var result = await Mediator.Send(command);
        return result;
    }

}