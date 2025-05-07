using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface ITransactionTypeManager : IManager
{
    Task<IResult<IEnumerable<TransactionTypeResponse>>> Get();
    Task<IResult<int?>> Put(AddTransactionTypeRequest transactionTypeRequest);
    Task<IResult<int?>> Post(SetTransactionTypeRequest transactionTypeRequest);
    Task<IResult<int?>> Delete(int transactionTypeId);
}