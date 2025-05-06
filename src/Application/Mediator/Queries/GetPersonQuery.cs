using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetPersonQuery : IRequest<Result<IEnumerable<PersonResponse>>>
{
    
}

public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, Result<IEnumerable<PersonResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetPersonQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<PersonResponse>>> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Persons
            .AsNoTracking()
            .Select(PersonResponse.Projection)
            .ToListAsync(cancellationToken);

        return await Result<IEnumerable<PersonResponse>>.SuccessAsync(data);
    }
}