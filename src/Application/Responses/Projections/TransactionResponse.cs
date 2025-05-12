using System.Linq.Expressions;

namespace HsaLedger.Application.Responses.Projections;

public class TransactionResponse
{
    public int TransactionId { get; set; }
    public int TransactionTypeId { get; set; }
    public virtual TransactionTypeResponse TransactionType { get; set; } = null!;
    public int ProviderId { get; set; }
    public virtual ProviderResponse Provider { get; set; } = null!;
    public int? PersonId { get; set; }
    public virtual PersonResponse? Person { get; set; }
    public DateTime? Date { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public bool IsHsaWithdrawn { get; set; }
    public bool IsAudited { get; set; }
    public required List<DocumentResponse> Documents { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public static Expression<Func<Domain.Entities.Transaction, TransactionResponse>> Projection
    {
        get
        {
            return x => new TransactionResponse
            {
                TransactionId = x.TransactionId,
                TransactionTypeId = x.TransactionTypeId,
                ProviderId = x.ProviderId,
                PersonId = x.PersonId,
                Date = x.Date,
                Amount = x.Amount / 100.0m, // Convert from int to currency with decimal
                IsPaid = x.IsPaid,
                IsHsaWithdrawn = x.IsHsaWithdrawn,
                IsAudited = x.IsAudited,
                TransactionType = TransactionTypeResponse.FromEntity(x.TransactionType),
                Provider = ProviderResponse.FromEntity(x.Provider),
                Person = x.Person != null ? PersonResponse.FromEntity(x.Person) : null,
                AllowDelete = true,
                Documents = x.Documents.AsQueryable().Select(DocumentResponse.Projection).ToList(),
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId
            };
        }
    }
    
    public static TransactionResponse FromEntity(Domain.Entities.Transaction entity)
    {
        return Projection.Compile().Invoke(entity);
    }
}