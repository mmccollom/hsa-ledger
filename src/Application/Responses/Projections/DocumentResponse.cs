using System.Linq.Expressions;

namespace HsaLedger.Application.Responses.Projections;

public class DocumentResponse
{
    public int DocumentId { get; set; }
    public int TransactionId { get; set; }
    public required string Fullname { get; set; }
    public required string Name { get; set; }
    public required string Extension { get; set; }
    public required byte[] Content { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public static Expression<Func<Domain.Entities.Document, DocumentResponse>> Projection
    {
        get
        {
            return x => new DocumentResponse
            {
                DocumentId = x.DocumentId,
                TransactionId = x.TransactionId,
                Fullname = x.Fullname,
                Name = x.Name,
                Extension = x.Extension,
                Content = x.Content,
                AllowDelete = true,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId
            };
        }
    }
    
    public static DocumentResponse FromEntity(Domain.Entities.Document entity)
    {
        return Projection.Compile().Invoke(entity);
    }
}