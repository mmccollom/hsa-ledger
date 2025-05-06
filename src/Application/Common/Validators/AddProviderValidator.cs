using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class AddProviderValidator : AbstractValidator<AddProviderCommand>
{
    public AddProviderValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.ProviderRequest.Name)
            .NotEmpty()
            .Length(1, 200);
        RuleFor(x => x.ProviderRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return !await context.Providers.AnyAsync(x => x.Name == v.Name, cancellationToken);
            })
            .WithMessage("Provider already exists");
    }
}