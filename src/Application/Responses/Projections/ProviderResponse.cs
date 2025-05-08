using System.Linq.Expressions;

namespace HsaLedger.Application.Responses.Projections;

public class ProviderResponse
{
    public int ProviderId { get; set; }
    public required string Name { get; set; }
    public required List<TransactionTypeResponse> TransactionTypes { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public static Expression<Func<Domain.Entities.Provider, ProviderResponse>> Projection
    {
        get
        {
            return x => new ProviderResponse
            {
                ProviderId = x.ProviderId,
                Name = x.Name,
                TransactionTypes = x.TransactionTypes.AsQueryable().Select(TransactionTypeResponse.Projection).ToList(),
                AllowDelete = x.Transactions.Count == 0 && x.TransactionTypes.Count == 0,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId
            };
        }
    }
    
    public static ProviderResponse FromEntity(Domain.Entities.Provider entity)
    {
        return Projection.Compile().Invoke(entity);
    }
}