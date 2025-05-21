using System.Linq.Expressions;

namespace HsaLedger.Domain.Common.Model;

public class DocumentModel
{
    public required int DocumentId { get; set; }
    public required int TransactionId { get; set; }
    public required string Fullname { get; set; }
    public required string Name { get; set; }
    public required string Extension { get; set; }
    public required byte[] Content { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public static Expression<Func<Entities.Document, DocumentModel>> Projection
    {
        get
        {
            return x => new DocumentModel
            {
                DocumentId = x.DocumentId,
                TransactionId = x.TransactionId,
                Fullname = x.Fullname,
                Name = x.Name,
                Extension = x.Extension,
                Content = x.Content,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId
            };
        }
    }
}