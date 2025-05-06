using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class DeletePersonValidator : AbstractValidator<DeletePersonCommand>
{
    public DeletePersonValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.PersonId)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await context.Persons.AnyAsync(x => x.PersonId == v,
                    cancellationToken);
            })
            .WithMessage("Unable to locate Person");
    }
}