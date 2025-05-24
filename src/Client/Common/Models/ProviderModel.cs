using System.Linq.Expressions;
using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common.Models;

public class ProviderModel : IEquatable<ProviderModel>
{
    public ProviderModel(int providerId)
    {
        ProviderId = providerId;
    }

    public int ProviderId { get; }
    public required string Name { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }

    public static Expression<Func<ProviderResponse, ProviderModel>> Projection
    {
        get
        {
            return x => new ProviderModel(x.ProviderId)
            {
                Name = x.Name,
                AllowDelete = x.AllowDelete,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId,
            };
        }
    }
    
    public static ProviderModel FromResponse(ProviderResponse response)
    {
        return Projection.Compile().Invoke(response);
    }

    public bool Equals(ProviderModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ProviderId == other.ProviderId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ProviderModel)obj);
    }

    public override int GetHashCode()
    {
        return ProviderId;
    }
}