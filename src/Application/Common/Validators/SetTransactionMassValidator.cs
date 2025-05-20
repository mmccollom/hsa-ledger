using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class SetTransactionMassValidator : AbstractValidator<SetTransactionMassCommand>
{
    public SetTransactionMassValidator(IApplicationDbContext context)
    {
        RuleForEach(x => x.TransactionRequests).ChildRules(transaction =>
        {
            transaction.RuleFor(x => x.Date)
                .NotEmpty();
            transaction.RuleFor(x => x.Amount)
                .Must(x => Math.Round(x * 100) == x * 100)
                .WithMessage("Must not have more than 2 decimal places");
            transaction.RuleFor(x => x)
                .MustAsync(async (v, cancellationToken) =>
                {
                    return await context.Transactions.AnyAsync(x => x.TransactionId == v.TransactionId, cancellationToken);
                })
                .WithMessage("Can't locate Transaction");
            transaction.RuleFor(x => x)
                .MustAsync(async (v, cancellationToken) =>
                {
                    return await context.TransactionTypes.AnyAsync(x => x.TransactionTypeId == v.TransactionTypeId, cancellationToken);
                })
                .WithMessage("Can't locate Transaction Type");
            transaction.RuleFor(x => x)
                .MustAsync(async (v, cancellationToken) =>
                {
                    return await context.Providers.AnyAsync(x => x.ProviderId == v.ProviderId, cancellationToken);
                })
                .WithMessage("Can't locate Provider");
            transaction.RuleFor(x => x)
                .MustAsync(async (v, cancellationToken) =>
                {
                    return v.PersonId == null || await context.Persons.AnyAsync(x => x.PersonId == v.PersonId, cancellationToken);
                })
                .WithMessage("Can't locate Person");
        });
    }
}