using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class SetTransactionTypeCommand : IRequest<Result<int>>
{
    public SetTransactionTypeCommand(SetTransactionTypeRequest transactionTypeRequest)
    {
        TransactionTypeRequest = transactionTypeRequest;
    }

    public SetTransactionTypeRequest TransactionTypeRequest { get; }
}

public class SetTransactionTypeCommandHandler : IRequestHandler<SetTransactionTypeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public SetTransactionTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(SetTransactionTypeCommand request, CancellationToken cancellationToken)
    {
        var transactionType = await _context.TransactionTypes
            .Include(x => x.Providers)
            .FirstAsync(x => x.TransactionTypeId == request.TransactionTypeRequest.TransactionTypeId, cancellationToken);
        var providers = from p in _context.Providers
            join tp in request.TransactionTypeRequest.ProviderIds on p.ProviderId equals tp
            select p;

        transactionType.Code = request.TransactionTypeRequest.Code;
        transactionType.Description = request.TransactionTypeRequest.Description;

        transactionType.Providers.Clear();

        foreach (var provider in providers)
        {
            transactionType.Providers.Add(provider);
        }
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Transaction Type Updated");
    }
}