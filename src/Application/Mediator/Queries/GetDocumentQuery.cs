using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetDocumentQuery : IRequest<Result<IEnumerable<DocumentResponse>>>
{
    
}

public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, Result<IEnumerable<DocumentResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetDocumentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<DocumentResponse>>> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Documents
            .AsNoTracking()
            .Select(DocumentResponse.Projection)
            .ToListAsync(cancellationToken);

        return await Result<IEnumerable<DocumentResponse>>.SuccessAsync(data);
    }
}