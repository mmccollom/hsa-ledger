using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class AddDocumentCommand : IRequest<Result<int>>
{
    public AddDocumentCommand(AddDocumentRequest documentRequest)
    {
        DocumentRequest = documentRequest;
    }

    public AddDocumentRequest DocumentRequest { get; }
}

public class AddDocumentCommandCommandHandler : IRequestHandler<AddDocumentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddDocumentCommandCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = new Domain.Entities.Document
        {
            TransactionId = request.DocumentRequest.TransactionId,
            Fullname = request.DocumentRequest.Fullname,
            Name = request.DocumentRequest.Name,
            Extension = request.DocumentRequest.Extension,
            Content = request.DocumentRequest.Content
        };

        var entity = await _context.Documents.AddAsync(document, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(entity.Entity.DocumentId);
    }
}