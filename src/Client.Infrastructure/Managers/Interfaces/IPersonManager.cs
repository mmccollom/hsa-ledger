using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface IPersonManager : IManager
{
    Task<IResult<IEnumerable<PersonResponse>>> Get();
    Task<IResult<int?>> Put(AddPersonRequest personRequest);
    Task<IResult<int?>> Post(SetPersonRequest personRequest);
    Task<IResult<int?>> Delete(int personId);
}