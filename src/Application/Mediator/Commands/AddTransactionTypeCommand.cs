using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class AddTransactionTypeCommand : IRequest<Result<int>>
{
    public AddTransactionTypeCommand(AddTransactionTypeRequest transactionTypeRequest)
    {
        TransactionTypeRequest = transactionTypeRequest;
    }

    public AddTransactionTypeRequest TransactionTypeRequest { get; }
}

public class AddTransactionTypeCommandHandler : IRequestHandler<AddTransactionTypeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddTransactionTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddTransactionTypeCommand request, CancellationToken cancellationToken)
    {
        var providers = from p in _context.Providers
            join tp in request.TransactionTypeRequest.ProviderIds on p.ProviderId equals tp
            select p;
        var transactionType = new Domain.Entities.TransactionType
        {
            Code = request.TransactionTypeRequest.Code,
            Description = request.TransactionTypeRequest.Description,
            Providers = providers.ToList()
        };

        var entity = await _context.TransactionTypes.AddAsync(transactionType, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(entity.Entity.TransactionTypeId);
    }
}