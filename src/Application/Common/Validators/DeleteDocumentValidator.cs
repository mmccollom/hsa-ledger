using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class DeleteDocumentValidator : AbstractValidator<DeleteDocumentCommand>
{
    public DeleteDocumentValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.DocumentId)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await context.Documents.AnyAsync(x => x.DocumentId == v,
                    cancellationToken);
            })
            .WithMessage("Unable to locate Document");
    }
}