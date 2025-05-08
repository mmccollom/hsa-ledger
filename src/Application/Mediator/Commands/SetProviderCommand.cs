using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class SetProviderCommand : IRequest<Result<int>>
{
    public SetProviderCommand(SetProviderRequest providerRequest)
    {
        ProviderRequest = providerRequest;
    }

    public SetProviderRequest ProviderRequest { get; }
}

public class SetProviderCommandHandler : IRequestHandler<SetProviderCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public SetProviderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(SetProviderCommand request, CancellationToken cancellationToken)
    {
        var provider = await _context.Providers.FirstAsync(x => x.ProviderId == request.ProviderRequest.ProviderId, cancellationToken);

        var transactionTypes = from tt in _context.TransactionTypes
            join p in request.ProviderRequest.TransactionTypeIds on tt.TransactionTypeId equals p
            select tt;
        
        provider.Name = request.ProviderRequest.Name;
        provider.TransactionTypes = transactionTypes.ToList();
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Provider Updated");
    }
}