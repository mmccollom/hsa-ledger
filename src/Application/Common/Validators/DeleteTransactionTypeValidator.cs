using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class DeleteTransactionTypeValidator : AbstractValidator<DeleteTransactionTypeCommand>
{
    public DeleteTransactionTypeValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TransactionTypeId)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await context.TransactionTypes.AnyAsync(x => x.TransactionTypeId == v,
                    cancellationToken);
            })
            .WithMessage("Unable to locate TransactionType");
    }
}