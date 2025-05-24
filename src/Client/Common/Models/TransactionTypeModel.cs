using System.Linq.Expressions;
using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common.Models;

public class TransactionTypeModel : IEquatable<TransactionTypeModel>
{
    public TransactionTypeModel(int transactionTypeId)
    {
        TransactionTypeId = transactionTypeId;
    }
    public int TransactionTypeId { get; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public required IEnumerable<ProviderModel> Providers { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }

    public string GetProviders()
    {
        var providers = string.Join(",", Providers.Select(t => t.Name));
        return providers.Length > 100 ? string.Concat(providers.AsSpan(0, 100), "...") : providers;
    }

    public static Expression<Func<TransactionTypeResponse, TransactionTypeModel>> Projection
    {
        get
        {
            return x => new TransactionTypeModel(x.TransactionTypeId)
            {
                Code = x.Code,
                Description = x.Description,
                Providers = x.Providers.AsQueryable().Select(ProviderModel.Projection).ToList(),
                AllowDelete = x.AllowDelete,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId,
            };
        }
    }
    
    public static TransactionTypeModel FromResponse(TransactionTypeResponse response)
    {
        return Projection.Compile().Invoke(response);
    }

    public bool Equals(TransactionTypeModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return TransactionTypeId == other.TransactionTypeId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TransactionTypeModel)obj);
    }

    public override int GetHashCode()
    {
        return TransactionTypeId;
    }
}