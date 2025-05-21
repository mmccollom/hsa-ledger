using System.Net.Http.Json;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using HsaLedger.Client.Infrastructure.Managers.Routes;
using HsaLedger.Domain.Common.Model;
using HsaLedger.Shared.Common.Extensions;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers;

public class TransactionTypeManager : ITransactionTypeManager
{
    private readonly HttpClient _httpClient;

    public TransactionTypeManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IResult<IEnumerable<TransactionTypeResponse>>> Get()
    {
        var response = await _httpClient.GetAsync(TransactionTypeEndpoints.Get);
        var result = await response.ToResult<IEnumerable<TransactionTypeResponse>>();
        return result;
    }
    
    public async Task<IResult<IEnumerable<TransactionTypeModel>>> GetUiModel()
    {
        var response = await _httpClient.GetAsync(TransactionTypeEndpoints.GetUiModel);
        var result = await response.ToResult<IEnumerable<TransactionTypeModel>>();
        return result;
    }

    public async Task<IResult<int?>> Put(AddTransactionTypeRequest transactionTypeRequest)
    {
        var response = await _httpClient.PutAsJsonAsync(TransactionTypeEndpoints.Put, transactionTypeRequest);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Post(SetTransactionTypeRequest transactionTypeRequest)
    {        
        var response = await _httpClient.PostAsJsonAsync(TransactionTypeEndpoints.Post, transactionTypeRequest);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Delete(int transactionTypeId)
    {
        var response = await _httpClient.DeleteAsync($"{TransactionTypeEndpoints.Delete}/{transactionTypeId}");
        var result = await response.ToResult<int?>();
        return result;
    }
}