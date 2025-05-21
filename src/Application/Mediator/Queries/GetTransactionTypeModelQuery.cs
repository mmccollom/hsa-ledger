using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Domain.Common.Model;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetTransactionTypeModelQuery : IRequest<Result<IEnumerable<TransactionTypeModel>>>
{
    
}

public class GetTransactionTypeModelQueryHandler : IRequestHandler<GetTransactionTypeModelQuery, Result<IEnumerable<TransactionTypeModel>>>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionTypeModelQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<TransactionTypeModel>>> Handle(GetTransactionTypeModelQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.TransactionTypes
            .AsNoTracking()
            .Select(TransactionTypeModel.Projection)
            .ToListAsync(cancellationToken);

        return await Result<IEnumerable<TransactionTypeModel>>.SuccessAsync(data);
    }
}