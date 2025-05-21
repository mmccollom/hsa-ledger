using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Pagination;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Domain.Common.Model;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface ITransactionManager : IManager
{
    Task<IResult<IEnumerable<TransactionResponse>>> Get();
    Task<IResult<GridQueryResponse<TransactionModel>>> GetPaged(GridQueryRequest gridQueryRequest);
    Task<IResult<IEnumerable<DocumentModel>>> GetDocuments(int transactionId);
    Task<IResult<int?>> Put(AddTransactionRequest transactionRequest);
    Task<IResult<int?>> Post(SetTransactionRequest transactionRequest);
    Task<IResult<int?>> Post(List<SetTransactionRequest> transactionRequests);
    Task<IResult<int?>> Delete(int transactionId);
}