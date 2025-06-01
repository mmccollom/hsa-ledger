using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Identity;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class RefreshCommand : IRequest<Result<AuthResponse>>
{
    public RefreshCommand(RefreshRequest refreshRequest)
    {
        RefreshRequest = refreshRequest;
    }

    public RefreshRequest RefreshRequest { get; set; }
}

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;

    public RefreshCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.Refresh(request.RefreshRequest, cancellationToken);
    }
}