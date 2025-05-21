using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Domain.Common.Model;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetProviderModelQuery : IRequest<Result<IEnumerable<ProviderModel>>>
{
    
}

public class GetProviderModelQueryHandler : IRequestHandler<GetProviderModelQuery, Result<IEnumerable<ProviderModel>>>
{
    private readonly IApplicationDbContext _context;

    public GetProviderModelQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<ProviderModel>>> Handle(GetProviderModelQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Providers
            .AsNoTracking()
            .Select(ProviderModel.Projection)
            .ToListAsync(cancellationToken);

        return await Result<IEnumerable<ProviderModel>>.SuccessAsync(data);
    }
}