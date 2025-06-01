using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class LoginCommand : IRequest<Result<AuthResponse>>
{
    public LoginCommand(LoginRequest loginRequest)
    {
        LoginRequest = loginRequest;
    }

    public LoginRequest LoginRequest { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.Login(request.LoginRequest, cancellationToken);
    }
}