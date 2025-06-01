using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class SetEnabledCommand : IRequest<Result<string>>
{
    public SetEnabledCommand(SetEnabledRequest setEnabledRequest)
    {
        SetEnabledRequest = setEnabledRequest;
    }

    public SetEnabledRequest SetEnabledRequest { get; set; }
}

public class SetEnabledCommandHandler : IRequestHandler<SetEnabledCommand, Result<string>>
{
    private readonly IIdentityService _identityService;

    public SetEnabledCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<string>> Handle(SetEnabledCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.SetEnabled(request.SetEnabledRequest);
    }
}