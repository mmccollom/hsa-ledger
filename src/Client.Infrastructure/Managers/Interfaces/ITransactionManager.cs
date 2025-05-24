using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Pagination;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Application.Responses.SimpleDto;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface ITransactionManager : IManager
{
    Task<IResult<IEnumerable<TransactionResponse>>> Get();
    Task<IResult<GridQueryResponse<TransactionResponse>>> GetPaged(GridQueryRequest gridQueryRequest);
    Task<IResult<DashboardResponse>> GetDashboardInfo();
    Task<IResult<IEnumerable<DocumentResponse>>> GetDocuments(int transactionId);
    Task<IResult<int?>> Put(AddTransactionRequest transactionRequest);
    Task<IResult<int?>> Post(SetTransactionRequest transactionRequest);
    Task<IResult<int?>> Post(List<SetTransactionRequest> transactionRequests);
    Task<IResult<int?>> Delete(int transactionId);
}