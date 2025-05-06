using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetTransactionTypeQuery : IRequest<Result<IEnumerable<TransactionTypeResponse>>>
{
    
}

public class GetTransactionTypeQueryHandler : IRequestHandler<GetTransactionTypeQuery, Result<IEnumerable<TransactionTypeResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionTypeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<TransactionTypeResponse>>> Handle(GetTransactionTypeQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.TransactionTypes
            .AsNoTracking()
            .Select(TransactionTypeResponse.Projection)
            .ToListAsync(cancellationToken);

        return await Result<IEnumerable<TransactionTypeResponse>>.SuccessAsync(data);
    }
}