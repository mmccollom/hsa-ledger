using System.Net.Http.Json;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using HsaLedger.Client.Infrastructure.Managers.Routes;
using HsaLedger.Shared.Common.Extensions;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers;

public class DocumentManager : IDocumentManager
{
    private readonly HttpClient _httpClient;

    public DocumentManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult<IEnumerable<DocumentResponse>>> Get()
    {
        var response = await _httpClient.GetAsync(DocumentEndpoints.Get);
        var result = await response.ToResult<IEnumerable<DocumentResponse>>();
        return result;
    }

    public async Task<IResult<int?>> Put(AddDocumentRequest documentRequest)
    {
        var response = await _httpClient.PutAsJsonAsync(DocumentEndpoints.Put, documentRequest);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Post(SetDocumentRequest documentRequest)
    {        
        var response = await _httpClient.PostAsJsonAsync(DocumentEndpoints.Post, documentRequest);
        var result = await response.ToResult<int?>();
        return result;
    }

    public async Task<IResult<int?>> Delete(int documentId)
    {
        var response = await _httpClient.DeleteAsync($"{DocumentEndpoints.Delete}/{documentId}");
        var result = await response.ToResult<int?>();
        return result;
    }
}