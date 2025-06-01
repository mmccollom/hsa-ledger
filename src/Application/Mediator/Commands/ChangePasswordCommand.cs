using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class ChangePasswordCommand : IRequest<Result<string>>
{
    public ChangePasswordCommand(ChangePasswordRequest changePasswordRequest, string userId)
    {
        ChangePasswordRequest = changePasswordRequest;
        UserId = userId;
    }

    public ChangePasswordRequest ChangePasswordRequest { get; set; }
    public string UserId { get; set; }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<string>>
{
    private readonly IIdentityService _identityService;

    public ChangePasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ChangePassword(request.ChangePasswordRequest, request.UserId);
    }
}