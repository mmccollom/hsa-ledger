using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class DeleteProviderCommand : IRequest<Result<int>>
{
    public DeleteProviderCommand(int providerId)
    {
        ProviderId = providerId;
    }

    public int ProviderId { get; }
}

public class DeleteProviderCommandHandler : IRequestHandler<DeleteProviderCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public DeleteProviderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeleteProviderCommand request, CancellationToken cancellationToken)
    {
        var provider = await _context.Providers.FirstOrDefaultAsync(x => x.ProviderId == request.ProviderId, cancellationToken);

        if (provider == null)
        {
            return await Result<int>.FailAsync("Not found");
        }
        
        _context.Providers.Remove(provider);
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Provider Deleted");
    }
}