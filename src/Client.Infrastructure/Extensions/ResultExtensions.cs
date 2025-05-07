using System.Text.Json;
using System.Text.Json.Serialization;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Extensions;

internal static class ResultExtensions
{
    internal static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return responseObject!;
    }

    internal static async Task<IResult> ToResult(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Result>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return responseObject!;
    }

    internal static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return responseObject!;
    }

    internal static async Task<T> ToObject<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<T>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return responseObject!;
    }
}