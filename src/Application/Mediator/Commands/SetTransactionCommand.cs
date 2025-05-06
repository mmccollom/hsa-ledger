using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class SetTransactionCommand : IRequest<Result<int>>
{
    public SetTransactionCommand(SetTransactionRequest transactionRequest)
    {
        TransactionRequest = transactionRequest;
    }

    public SetTransactionRequest TransactionRequest { get; }
}

public class SetTransactionCommandHandler : IRequestHandler<SetTransactionCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public SetTransactionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(SetTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions.FirstAsync(x => x.TransactionId == request.TransactionRequest.TransactionId, cancellationToken);

        transaction.TransactionTypeId = request.TransactionRequest.TransactionTypeId;
        transaction.ProviderId = request.TransactionRequest.ProviderId;
        transaction.PersonId = request.TransactionRequest.PersonId;
        transaction.Date = request.TransactionRequest.Date;
        transaction.Amount = Convert.ToInt32(request.TransactionRequest.Amount * 100); // Store currency without decimal
        transaction.IsPaid = request.TransactionRequest.IsPaid;
        transaction.IsHsaWithdrawn = request.TransactionRequest.IsHsaWithdrawn;
        transaction.IsAudited = request.TransactionRequest.IsAudited;
        
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Transaction Updated");
    }
}