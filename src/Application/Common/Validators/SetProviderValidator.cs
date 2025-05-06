using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class SetProviderValidator : AbstractValidator<SetProviderCommand>
{
    public SetProviderValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.ProviderRequest.Name)
            .NotEmpty()
            .Length(1, 200);
        RuleFor(x => x.ProviderRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await context.Providers.AnyAsync(x => x.ProviderId == v.ProviderId, cancellationToken);
            })
            .WithMessage("Unable to locate Provider");
        RuleFor(x => x.ProviderRequest)
            .MustAsync(async (v, cancellationToken) =>
            {
                return !await context.Providers.AnyAsync(x => x.Name == v.Name, cancellationToken);
            })
            .WithMessage("Provider already exists");
    }
}