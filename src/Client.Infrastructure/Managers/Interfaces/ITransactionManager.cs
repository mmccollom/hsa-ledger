using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface ITransactionManager : IManager
{
    Task<IResult<IEnumerable<TransactionResponse>>> Get();
    Task<IResult<IEnumerable<DocumentResponse>>> GetDocuments(int transactionId);
    Task<IResult<int?>> Put(AddTransactionRequest transactionRequest);
    Task<IResult<int?>> Post(SetTransactionRequest transactionRequest);
    Task<IResult<int?>> Delete(int transactionId);
}