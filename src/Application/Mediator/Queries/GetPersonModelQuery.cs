using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Domain.Common.Model;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetPersonModelQuery : IRequest<Result<IEnumerable<PersonModel>>>
{
    
}

public class GetPersonModelQueryHandler : IRequestHandler<GetPersonModelQuery, Result<IEnumerable<PersonModel>>>
{
    private readonly IApplicationDbContext _context;

    public GetPersonModelQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<PersonModel>>> Handle(GetPersonModelQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Persons
            .AsNoTracking()
            .Select(PersonModel.Projection)
            .ToListAsync(cancellationToken);

        return await Result<IEnumerable<PersonModel>>.SuccessAsync(data);
    }
}