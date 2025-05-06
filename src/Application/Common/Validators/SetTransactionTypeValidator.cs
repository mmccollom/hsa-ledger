using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class SetTransactionTypeValidator : AbstractValidator<SetTransactionTypeCommand>
{
    public SetTransactionTypeValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TransactionTypeRequest.Code)
            .NotEmpty()
            .Length(1, 50);
        RuleFor(x => x.TransactionTypeRequest.Description)
            .NotEmpty()
            .Length(1, 200);
        RuleFor(x => x.TransactionTypeRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await context.TransactionTypes.AnyAsync(x => x.TransactionTypeId == v.TransactionTypeId, cancellationToken);
            })
            .WithMessage("Unable to locate TransactionType");
        RuleFor(x => x.TransactionTypeRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return !await context.TransactionTypes.AnyAsync(x => x.Code == v.Code, cancellationToken);
            })
            .WithMessage("Transaction Type already exists");
    }
}