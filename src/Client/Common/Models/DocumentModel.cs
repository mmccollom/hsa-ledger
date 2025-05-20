using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common.Models;

public class DocumentModel
{
    public required int DocumentId { get; set; }
    public required int TransactionId { get; set; }
    public required string Fullname { get; set; }
    public required string Name { get; set; }
    public required string Extension { get; set; }
    public required byte[] Content { get; set; }
    public required bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public static DocumentModel FromDocumentResponse(DocumentResponse response)
    {
        return new DocumentModel
        {
            DocumentId = response.DocumentId,
            TransactionId = response.TransactionId,
            Fullname = response.Fullname,
            Name = response.Name,
            Extension = response.Extension,
            Content = response.Content,
            AllowDelete = response.AllowDelete,
            CreatedTime = response.CreatedTime,
            CreatedBy = response.CreatedBy,
            LastUpdatedTime = response.LastUpdatedTime,
            LastUpdatedBy = response.LastUpdatedBy,
            LockId = response.LockId,
        };
    }
}