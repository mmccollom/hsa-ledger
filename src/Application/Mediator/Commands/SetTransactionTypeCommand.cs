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
        var transactionType = await _context.TransactionTypes.FirstAsync(x => x.TransactionTypeId == request.TransactionTypeRequest.TransactionTypeId, cancellationToken);

        transactionType.Code = request.TransactionTypeRequest.Code;
        transactionType.Description = request.TransactionTypeRequest.Description;
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Transaction Type Updated");
    }
}