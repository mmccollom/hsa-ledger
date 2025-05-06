using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class DeleteTransactionCommand : IRequest<Result<int>>
{
    public DeleteTransactionCommand(int transactionId)
    {
        TransactionId = transactionId;
    }

    public int TransactionId { get; }
}

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public DeleteTransactionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.TransactionId == request.TransactionId, cancellationToken);

        if (transaction == null)
        {
            return await Result<int>.FailAsync("Not found");
        }
        
        _context.Transactions.Remove(transaction);
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Transaction Deleted");
    }
}