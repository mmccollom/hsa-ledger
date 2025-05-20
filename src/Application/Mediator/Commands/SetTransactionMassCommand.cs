using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class SetTransactionMassCommand : IRequest<Result<int>>
{
    public SetTransactionMassCommand(List<SetTransactionRequest> transactionRequests)
    {
        TransactionRequests = transactionRequests;
    }

    public List<SetTransactionRequest> TransactionRequests { get; }
}

public class SetTransactionMassCommandHandler : IRequestHandler<SetTransactionMassCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public SetTransactionMassCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(SetTransactionMassCommand request, CancellationToken cancellationToken)
    {
        foreach (var transactionRequest in request.TransactionRequests)
        {
            var transaction = await _context.Transactions.FirstAsync(x => x.TransactionId == transactionRequest.TransactionId, cancellationToken);

            transaction.TransactionTypeId = transactionRequest.TransactionTypeId;
            transaction.ProviderId = transactionRequest.ProviderId;
            transaction.PersonId = transactionRequest.PersonId;
            transaction.Date = transactionRequest.Date;
            transaction.Amount = Convert.ToInt32(transactionRequest.Amount * 100); // Store currency without decimal
            transaction.IsPaid = transactionRequest.IsPaid;
            transaction.IsHsaWithdrawn = transactionRequest.IsHsaWithdrawn;
            transaction.IsAudited = transactionRequest.IsAudited;
        }
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Transactions Updated");
    }
}