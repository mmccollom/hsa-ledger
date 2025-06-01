using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetAppUsersQuery  : IRequest<Result<IEnumerable<UserResponse>>>
{
}

public class GetAppUsersQueryHandler : IRequestHandler<GetAppUsersQuery, Result<IEnumerable<UserResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetAppUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<UserResponse>>> Handle(GetAppUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.GetUsers();
        var data = users.AsQueryable()
            .Select(UserResponse.Projection)
            .ToList();
        
        return await Result<IEnumerable<UserResponse>>.SuccessAsync(data);
    }
}