using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetDocumentContentQuery : IRequest<Result<byte[]>>
{
    public GetDocumentContentQuery(int documentId)
    {
        DocumentId = documentId;
    }

    public int DocumentId { get; set; }
}

public class GetDocumentContentQueryHandler : IRequestHandler<GetDocumentContentQuery, Result<byte[]>>
{
    private readonly IApplicationDbContext _context;

    public GetDocumentContentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<byte[]>> Handle(GetDocumentContentQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Documents
            .Where(x => x.DocumentId == request.DocumentId)
            .AsNoTracking()
            .Select(x => x.Content)
            .FirstAsync(cancellationToken);

        return await Result<byte[]>.SuccessAsync(data);
    }
}