using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class DeleteTransactionValidator : AbstractValidator<DeleteTransactionCommand>
{
    public DeleteTransactionValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TransactionId)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await context.Transactions.AnyAsync(x => x.TransactionId == v,
                    cancellationToken);
            })
            .WithMessage("Unable to locate Transaction");
    }
}