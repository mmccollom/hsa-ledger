using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetDocumentByTransactionIdQuery : IRequest<Result<IEnumerable<DocumentResponse>>>
{
    public GetDocumentByTransactionIdQuery(int transactionId)
    {
        TransactionId = transactionId;
    }

    public int TransactionId { get; set; }
}

public class GetDocumentByTransactionIdQueryHandler : IRequestHandler<GetDocumentByTransactionIdQuery, Result<IEnumerable<DocumentResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetDocumentByTransactionIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<DocumentResponse>>> Handle(GetDocumentByTransactionIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Documents
            .Where(x => x.TransactionId == request.TransactionId)
            .AsNoTracking()
            .Select(DocumentResponse.Projection)
            .ToListAsync(cancellationToken);

        return await Result<IEnumerable<DocumentResponse>>.SuccessAsync(data);
    }
}