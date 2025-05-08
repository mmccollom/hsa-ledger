using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class SetPersonValidator : AbstractValidator<SetPersonCommand>
{
    public SetPersonValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.PersonRequest.Name)
            .NotEmpty()
            .Length(1, 200);
        RuleFor(x => x.PersonRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await context.Persons.AnyAsync(x => x.PersonId == v.PersonId, cancellationToken);
            })
            .WithMessage("Unable to locate Person");
        RuleFor(x => x.PersonRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return !await context.Persons.AnyAsync(x => x.Name == v.Name && x.PersonId != v.PersonId, cancellationToken);
            })
            .WithMessage("Person already exists");
    }
}