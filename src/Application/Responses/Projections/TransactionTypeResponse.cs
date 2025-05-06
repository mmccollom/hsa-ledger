using System.Linq.Expressions;

namespace HsaLedger.Application.Responses.Projections;

public class TransactionTypeResponse
{
    public int TransactionTypeId { get; set; }
    public required string Code { get; set; }
    public required string Description { get; set; }
    public required ICollection<ProviderResponse> Providers { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public static Expression<Func<Domain.Entities.TransactionType, TransactionTypeResponse>> Projection
    {
        get
        {
            return x => new TransactionTypeResponse
            {
                TransactionTypeId = x.TransactionTypeId,
                Code = x.Code,
                Description = x.Description,
                Providers = x.Providers.AsQueryable().Select(ProviderResponse.Projection).ToList(),
                AllowDelete = x.Transactions.Count == 0 && x.Providers.Count == 0,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId
            };
        }
    }
    
    public static TransactionTypeResponse FromEntity(Domain.Entities.TransactionType entity)
    {
        return Projection.Compile().Invoke(entity);
    }
}