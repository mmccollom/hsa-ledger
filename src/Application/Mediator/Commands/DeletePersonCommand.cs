using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Commands;

public class DeletePersonCommand : IRequest<Result<int>>
{
    public DeletePersonCommand(int personId)
    {
        PersonId = personId;
    }

    public int PersonId { get; }
}

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public DeletePersonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.Persons.FirstOrDefaultAsync(x => x.PersonId == request.PersonId, cancellationToken);

        if (person == null)
        {
            return await Result<int>.FailAsync("Not found");
        }
        
        _context.Persons.Remove(person);
        
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(changes, "Person Deleted");
    }
}