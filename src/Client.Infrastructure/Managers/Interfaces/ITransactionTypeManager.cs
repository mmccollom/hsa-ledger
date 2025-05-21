using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Domain.Common.Model;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface ITransactionTypeManager : IManager
{
    Task<IResult<IEnumerable<TransactionTypeResponse>>> Get();
    Task<IResult<IEnumerable<TransactionTypeModel>>> GetUiModel();
    Task<IResult<int?>> Put(AddTransactionTypeRequest transactionTypeRequest);
    Task<IResult<int?>> Post(SetTransactionTypeRequest transactionTypeRequest);
    Task<IResult<int?>> Delete(int transactionTypeId);
}