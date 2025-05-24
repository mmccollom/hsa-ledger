using System.Linq.Expressions;
using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common.Models;

public class DocumentModel
{
    public required int DocumentId { get; set; }
    public required int TransactionId { get; set; }
    public required string Fullname { get; set; }
    public required string Name { get; set; }
    public required string Extension { get; set; }
    //public required byte[] Content { get; set; } // Not needed in the model
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public static Expression<Func<DocumentResponse, DocumentModel>> Projection
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
                //Content = x.Content,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId
            };
        }
    }
}