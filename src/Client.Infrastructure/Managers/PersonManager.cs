using System.Net.Http.Json;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using HsaLedger.Client.Infrastructure.Managers.Routes;
using HsaLedger.Domain.Common.Model;
using HsaLedger.Shared.Common.Extensions;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers;

public class PersonManager : IPersonManager
{
    private readonly HttpClient _httpClient;

    public PersonManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IResult<IEnumerable<PersonResponse>>> Get()
    {
        var response = await _httpClient.GetAsync(PersonEndpoints.Get);
        var result = await response.ToResult<IEnumerable<PersonResponse>>();
        return result;
    }
    
    public async Task<IResult<IEnumerable<PersonModel>>> GetUiModel()
    {
        var response = await _httpClient.GetAsync(PersonEndpoints.GetUiModel);
        var result = await response.ToResult<IEnumerable<PersonModel>>();
        return result;
    }

    public async Task<IResult<int?>> Put(AddPersonRequest personRequest)
    {
        var response = await _httpClient.PutAsJsonAsync(PersonEndpoints.Put, personRequest);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Post(SetPersonRequest personRequest)
    {        
        var response = await _httpClient.PostAsJsonAsync(PersonEndpoints.Post, personRequest);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Delete(int personId)
    {
        var response = await _httpClient.DeleteAsync($"{PersonEndpoints.Delete}/{personId}");
        var result = await response.ToResult<int?>();
        return result;
    }
}