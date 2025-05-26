using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Server.Identity;

public class IdentityQueries
{
    private readonly IApplicationDbContext _context;

    public IdentityQueries(IApplicationDbContext context)
    {
        _context = context;
    }

    internal async Task<Result<IEnumerable<UserResponse>>> GetUsersAsync()
    {
        var users = await _context.GetUsers();
        var data = users.AsQueryable()
            .Select(UserResponse.Projection)
            .ToList();

        return await Result<IEnumerable<UserResponse>>.SuccessAsync(data);
    }

    internal async Task<Result<IEnumerable<RoleResponse>>> GetRolesAsync()
    {
        var users = await _context.GetRoles();
        var data = users.AsQueryable()
            .Select(RoleResponse.Projection)
            .ToList();

        return await Result<IEnumerable<RoleResponse>>.SuccessAsync(data);
    }
}