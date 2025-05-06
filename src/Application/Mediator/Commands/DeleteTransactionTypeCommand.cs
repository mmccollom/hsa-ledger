using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class DeleteTransactionTypeCommand : IRequest<Result<int>>
{
    public DeleteTransactionTypeCommand(int transactionTypeId)
    {
        TransactionTypeId = transactionTypeId;
    }

    public int TransactionTypeId { get; }
}

public class DeleteTransactionTypeCommandHandler : IRequestHandler<DeleteTransactionTypeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public DeleteTransactionTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeleteTransactionTypeCommand request, CancellationToken cancellationToken)
    {
        var transactionType = await _context.TransactionTypes.FirstOrDefaultAsync(x => x.TransactionTypeId == request.TransactionTypeId, cancellationToken);

        if (transactionType == null)
        {
            return await Result<int>.FailAsync("Not found");
        }
        
        _context.TransactionTypes.Remove(transactionType);
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "TransactionType Deleted");
    }
}