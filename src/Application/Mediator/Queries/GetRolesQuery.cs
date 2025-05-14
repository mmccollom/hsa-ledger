using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Queries;

public class GetRolesQuery : IRequest<Result<IEnumerable<RoleResponse>>>
{
    
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<IEnumerable<RoleResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetRolesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<RoleResponse>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.GetRoles();
        var data = users.AsQueryable()
            .Select(RoleResponse.Projection)
            .ToList();

        return await Result<IEnumerable<RoleResponse>>.SuccessAsync(data);
    }
}