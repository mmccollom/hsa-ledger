using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetTransactionQuery : IRequest<Result<IEnumerable<TransactionResponse>>>
{
    
}

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, Result<IEnumerable<TransactionResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<TransactionResponse>>> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Transactions
            .AsNoTracking()
            .Select(TransactionResponse.Projection)
            .ToListAsync(cancellationToken);

        return await Result<IEnumerable<TransactionResponse>>.SuccessAsync(data);
    }
}