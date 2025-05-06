using FluentValidation;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Common.Validators;

public class DeleteProviderValidator : AbstractValidator<DeleteProviderCommand>
{
    public DeleteProviderValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.ProviderId)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await context.Providers.AnyAsync(x => x.ProviderId == v,
                    cancellationToken);
            })
            .WithMessage("Unable to locate Provider");
    }
}