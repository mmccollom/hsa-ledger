using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class SetDocumentCommand : IRequest<Result<int>>
{
    public SetDocumentCommand(SetDocumentRequest documentRequest)
    {
        DocumentRequest = documentRequest;
    }

    public SetDocumentRequest DocumentRequest { get; }
}

public class SetDocumentCommandHandler : IRequestHandler<SetDocumentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public SetDocumentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(SetDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.Documents.FirstAsync(x => x.DocumentId == request.DocumentRequest.DocumentId, cancellationToken);

        document.TransactionId = request.DocumentRequest.TransactionId;
        document.Name = request.DocumentRequest.Name;
        document.Fullname = request.DocumentRequest.Fullname;
        document.Extension = request.DocumentRequest.Extension;
        document.Content = request.DocumentRequest.Content;
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Document Updated");
    }
}