using System.Net.Http.Json;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Models;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using HsaLedger.Client.Infrastructure.Managers.Routes;
using HsaLedger.Shared.Common.Extensions;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers;

public class ProviderManager : IProviderManager
{
    private readonly HttpClient _httpClient;

    public ProviderManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IResult<IEnumerable<ProviderResponse>>> Get()
    {
        var response = await _httpClient.GetAsync(ProviderEndpoints.Get);
        var result = await response.ToResult<IEnumerable<ProviderResponse>>();
        return result;
    }
    
    public async Task<IResult<IEnumerable<ProviderModel>>> GetUiModel()
    {
        var response = await _httpClient.GetAsync(ProviderEndpoints.GetUiModel);
        var result = await response.ToResult<IEnumerable<ProviderModel>>();
        return result;
    }

    public async Task<IResult<int?>> Put(AddProviderRequest providerRequest)
    {
        var response = await _httpClient.PutAsJsonAsync(ProviderEndpoints.Put, providerRequest);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Post(SetProviderRequest providerRequest)
    {        
        var response = await _httpClient.PostAsJsonAsync(ProviderEndpoints.Post, providerRequest);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Delete(int providerId)
    {
        var response = await _httpClient.DeleteAsync($"{ProviderEndpoints.Delete}/{providerId}");
        var result = await response.ToResult<int?>();
        return result;
    }
}