using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;

namespace HsaLedger.Application.Mediator.Commands;

public class AddPersonCommand : IRequest<Result<int>>
{
    public AddPersonCommand(AddPersonRequest personRequest)
    {
        PersonRequest = personRequest;
    }

    public AddPersonRequest PersonRequest { get; }
}

public class AddPersonCommandHandler : IRequestHandler<AddPersonCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddPersonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddPersonCommand request, CancellationToken cancellationToken)
    {
        var person = new Domain.Entities.Person
        {
            Name = request.PersonRequest.Name,
        };

        var entity = await _context.Persons.AddAsync(person, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(entity.Entity.PersonId);
    }
}