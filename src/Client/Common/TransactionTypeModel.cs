using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common;

public class TransactionTypeModel
{
    public int TransactionTypeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public required IEnumerable<ProviderResponse> Providers { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public string GetTransactionTypes()
    {
        return string.Join(",", this.Providers.Select(t => t.Name));
    }

    public static TransactionTypeModel FromTransactionTypeResponse(TransactionTypeResponse response)
    {
        return new TransactionTypeModel
        {
            TransactionTypeId = response.TransactionTypeId,
            Code = response.Code,
            Description = response.Description,
            Providers = response.Providers,
            AllowDelete = response.AllowDelete,
            CreatedTime = response.CreatedTime,
            CreatedBy = response.CreatedBy,
            LastUpdatedTime = response.LastUpdatedTime,
            LastUpdatedBy = response.LastUpdatedBy,
            LockId = response.LockId,
        };
    }
}