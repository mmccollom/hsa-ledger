using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common;

public class ProviderModel
{
    public required int ProviderId { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<TransactionTypeResponse> TransactionTypes { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }

    public string GetTransactionTypes()
    {
        return string.Join(",", this.TransactionTypes.Select(t => t.Code));
    }

    public static ProviderModel FromProviderResponse(ProviderResponse response)
    {
        return new ProviderModel
        {
            ProviderId = response.ProviderId,
            Name = response.Name,
            TransactionTypes = response.TransactionTypes,
            AllowDelete = response.AllowDelete,
            CreatedTime = response.CreatedTime,
            CreatedBy = response.CreatedBy,
            LastUpdatedTime = response.LastUpdatedTime,
            LastUpdatedBy = response.LastUpdatedBy,
            LockId = response.LockId,
        };
    }
}