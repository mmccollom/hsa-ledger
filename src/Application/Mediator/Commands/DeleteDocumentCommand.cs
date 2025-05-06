using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class DeleteDocumentCommand : IRequest<Result<int>>
{
    public DeleteDocumentCommand(int documentId)
    {
        DocumentId = documentId;
    }

    public int DocumentId { get; }
}

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public DeleteDocumentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(x => x.DocumentId == request.DocumentId, cancellationToken);

        if (document == null)
        {
            return await Result<int>.FailAsync("Not found");
        }
        
        _context.Documents.Remove(document);
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Document Deleted");
    }
}