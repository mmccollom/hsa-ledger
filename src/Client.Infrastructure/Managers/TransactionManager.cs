using System.Net.Http.Json;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Pagination;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Application.Responses.SimpleDto;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using HsaLedger.Client.Infrastructure.Managers.Routes;
using HsaLedger.Shared.Common.Extensions;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers;

public class TransactionManager : ITransactionManager
{
    private readonly HttpClient _httpClient;

    public TransactionManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IResult<IEnumerable<TransactionResponse>>> Get()
    {
        var response = await _httpClient.GetAsync(TransactionEndpoints.Get);
        var result = await response.ToResult<IEnumerable<TransactionResponse>>();
        return result;
    }
    
    public async Task<IResult<DashboardResponse>> GetDashboardInfo()
    {
        var response = await _httpClient.GetAsync(TransactionEndpoints.GetDashboard);
        var result = await response.ToResult<DashboardResponse>();
        return result;
    }
    
    public async Task<IResult<GridQueryResponse<TransactionResponse>>> GetPaged(GridQueryRequest gridQueryRequest)
    {
        var response = await _httpClient.PostAsJsonAsync(TransactionEndpoints.GetPaged, gridQueryRequest);
        var result = await response.ToResult<GridQueryResponse<TransactionResponse>>();
        return result;
    }
    
    public async Task<IResult<IEnumerable<DocumentResponse>>> GetDocuments(int transactionId)
    {
        var response = await _httpClient.GetAsync($"{TransactionEndpoints.GetDocuments}?transactionId={transactionId}");
        var result = await response.ToResult<IEnumerable<DocumentResponse>>();
        return result;
    }

    public async Task<IResult<int?>> Put(AddTransactionRequest transactionRequest)
    {
        var response = await _httpClient.PutAsJsonAsync(TransactionEndpoints.Put, transactionRequest);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Post(SetTransactionRequest transactionRequest)
    {        
        var response = await _httpClient.PostAsJsonAsync(TransactionEndpoints.Post, transactionRequest);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Post(List<SetTransactionRequest> transactionRequests)
    {        
        var response = await _httpClient.PostAsJsonAsync(TransactionEndpoints.MassUpdate, transactionRequests);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Delete(int transactionId)
    {
        var response = await _httpClient.DeleteAsync($"{TransactionEndpoints.Delete}/{transactionId}");
        var result = await response.ToResult<int?>();
        return result;
    }
}