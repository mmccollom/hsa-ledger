using System.Security.Claims;
using HsaLedger.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HsaLedger.Server.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? UserName => User?.Identity?.Name;

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;
}