using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetProviderQuery : IRequest<Result<IEnumerable<ProviderResponse>>>
{
    
}

public class GetProviderQueryHandler : IRequestHandler<GetProviderQuery, Result<IEnumerable<ProviderResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetProviderQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<ProviderResponse>>> Handle(GetProviderQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Providers
            .AsNoTracking()
            .Select(ProviderResponse.Projection)
            .ToListAsync(cancellationToken);

        return await Result<IEnumerable<ProviderResponse>>.SuccessAsync(data);
    }
}