using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Queries;

public class GetUsersQuery : IRequest<Result<IEnumerable<UserResponse>>>
{
    
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<IEnumerable<UserResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<UserResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.GetUsers();
        var data = users.AsQueryable()
            .Select(UserResponse.Projection)
            .ToList();

        return await Result<IEnumerable<UserResponse>>.SuccessAsync(data);
    }
}