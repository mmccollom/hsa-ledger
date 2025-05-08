using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class AddProviderCommand : IRequest<Result<int>>
{
    public AddProviderCommand(AddProviderRequest providerRequest)
    {
        ProviderRequest = providerRequest;
    }

    public AddProviderRequest ProviderRequest { get; }
}

public class AddProviderCommandHandler : IRequestHandler<AddProviderCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddProviderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddProviderCommand request, CancellationToken cancellationToken)
    {
        var transactionTypes = from tt in _context.TransactionTypes
                join p in request.ProviderRequest.TransactionTypeIds on tt.TransactionTypeId equals p
                select tt;
        
        var provider = new Domain.Entities.Provider
        {
            Name = request.ProviderRequest.Name,
            TransactionTypes = transactionTypes.ToList(), 
        };

        var entity = await _context.Providers.AddAsync(provider, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(entity.Entity.ProviderId);
    }
}