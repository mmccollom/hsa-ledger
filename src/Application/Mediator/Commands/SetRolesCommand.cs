using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class SetRolesCommand : IRequest<Result<string>>
{
    public SetRolesCommand(SetRolesRequest setRolesRequest)
    {
        SetRolesRequest = setRolesRequest;
    }

    public SetRolesRequest SetRolesRequest { get; set; }
}

public class SetRolesCommandHandler : IRequestHandler<SetRolesCommand, Result<string>>
{
    private readonly IIdentityService _identityService;

    public SetRolesCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<string>> Handle(SetRolesCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.SetRoles(request.SetRolesRequest);
    }
}