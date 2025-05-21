using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetDocumentQuery : IRequest<Result<DocumentResponse>>
{
    public GetDocumentQuery(int documentId)
    {
        DocumentId = documentId;
    }

    public int DocumentId { get; set; }
}

public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, Result<DocumentResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetDocumentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DocumentResponse>> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Documents
            .Where(x => x.DocumentId == request.DocumentId)
            .AsNoTracking()
            .Select(DocumentResponse.Projection)
            .FirstAsync(cancellationToken);

        return await Result<DocumentResponse>.SuccessAsync(data);
    }
}