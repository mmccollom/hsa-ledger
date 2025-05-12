using System.Globalization;
using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class AddTransactionValidator : AbstractValidator<AddTransactionCommand>
{
    public AddTransactionValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TransactionRequest.Date)
            .NotEmpty();
        RuleFor(x => x.TransactionRequest.Amount)
            .NotEmpty().Must(x => Math.Round(x * 100) == x * 100)
            .WithMessage("Must not have more than 2 decimal places");
        RuleFor(x => x.TransactionRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await context.TransactionTypes.AnyAsync(x => x.TransactionTypeId == v.TransactionTypeId, cancellationToken);
            })
            .WithMessage("Can't locate Transaction Type");
        RuleFor(x => x.TransactionRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await context.Providers.AnyAsync(x => x.ProviderId == v.ProviderId, cancellationToken);
            })
            .WithMessage("Can't locate Provider");
        RuleFor(x => x.TransactionRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return v.PersonId == null || await context.Persons.AnyAsync(x => x.PersonId == v.PersonId, cancellationToken);
            })
            .WithMessage("Can't locate Person");
    }
}