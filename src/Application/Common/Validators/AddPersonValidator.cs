using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class AddPersonValidator : AbstractValidator<AddPersonCommand>
{
    public AddPersonValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.PersonRequest.Name)
            .NotEmpty()
            .Length(1, 200);
        RuleFor(x => x.PersonRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return !await context.Persons.AnyAsync(x => x.Name == v.Name, cancellationToken);
            })
            .WithMessage("Person already exists");
    }
}