using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class AddTransactionCommand : IRequest<Result<int>>
{
    public AddTransactionCommand(AddTransactionRequest transactionRequest)
    {
        TransactionRequest = transactionRequest;
    }

    public AddTransactionRequest TransactionRequest { get; }
}

public class AddTransactionCommandHandler : IRequestHandler<AddTransactionCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddTransactionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = new Domain.Entities.Transaction
        {
            TransactionTypeId = request.TransactionRequest.TransactionTypeId,
            ProviderId = request.TransactionRequest.ProviderId,
            PersonId = request.TransactionRequest.PersonId,
            Date =  request.TransactionRequest.Date,
            Amount = Convert.ToInt32(request.TransactionRequest.Amount * 100), // Store currency without decimal
            IsPaid = request.TransactionRequest.IsPaid,
            IsHsaWithdrawn = request.TransactionRequest.IsHsaWithdrawn,
            IsAudited = request.TransactionRequest.IsAudited,
            Documents = request.TransactionRequest.Documents?.Select(x => new HsaLedger.Domain.Entities.Document
            {
                Fullname = x.Fullname,
                Name = x.Name,
                Extension = x.Extension,
                Content = x.Content
            }).ToList() ?? []
        };

        var entity = await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return await Result<int>.SuccessAsync(entity.Entity.TransactionId);
    }
}