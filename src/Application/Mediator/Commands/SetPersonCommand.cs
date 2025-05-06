using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class SetPersonCommand : IRequest<Result<int>>
{
    public SetPersonCommand(SetPersonRequest personRequest)
    {
        PersonRequest = personRequest;
    }

    public SetPersonRequest PersonRequest { get; }
}

public class SetPersonCommandHandler : IRequestHandler<SetPersonCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public SetPersonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(SetPersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.Persons.FirstAsync(x => x.PersonId == request.PersonRequest.PersonId, cancellationToken);

        person.Name = request.PersonRequest.Name;
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Person Updated");
    }
}