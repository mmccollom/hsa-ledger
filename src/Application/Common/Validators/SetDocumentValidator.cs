using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class SetDocumentValidator : AbstractValidator<SetDocumentCommand>
{
    public SetDocumentValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.DocumentRequest.Fullname)
            .NotEmpty()
            .Length(1, 250);
        RuleFor(x => x.DocumentRequest.Name)
            .NotEmpty()
            .Length(1, 200);
        RuleFor(x => x.DocumentRequest.Extension)
            .NotEmpty()
            .Length(1, 50);
        RuleFor(x => x.DocumentRequest.Content)
            .NotEmpty();
        RuleFor(x => x.DocumentRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return !await context.Documents.AnyAsync(x => x.DocumentId == v.DocumentId, cancellationToken);
            })
            .WithMessage("Unable to locate Document");
        RuleFor(x => x.DocumentRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return !await context.Transactions.AnyAsync(x => x.TransactionId == v.TransactionId, cancellationToken);
            })
            .WithMessage("Unable to locate Transaction");
    }
}