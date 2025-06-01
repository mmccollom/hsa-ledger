using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class RegisterCommand : IRequest<Result<string>>
{
    public RegisterCommand(RegisterRequest registerRequest)
    {
        RegisterRequest = registerRequest;
    }

    public RegisterRequest RegisterRequest { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.Register(request.RegisterRequest);
    }
}