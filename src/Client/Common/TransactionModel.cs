using HsaLedger.Application.Responses.Projections;
using Microsoft.AspNetCore.Components.Forms;

namespace HsaLedger.Client.Common;

public class TransactionModel
{
    public int TransactionId { get; set; }
    public int TransactionTypeId { get; set; }
    public virtual TransactionTypeModel TransactionType { get; set; } = null!;
    public int ProviderId { get; set; }
    public virtual ProviderModel Provider { get; set; } = null!;
    public int? PersonId { get; set; }
    public virtual PersonModel? Person { get; set; }
    public DateTime? Date { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public bool IsHsaWithdrawn { get; set; }
    public bool IsAudited { get; set; }
    public required List<DocumentModel> Documents { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }

    public string GetDocumentNames()
    {
        var documents = string.Join(",", Documents.Select(t => t.Name));
        return documents.Length > 100 ? string.Concat(documents.AsSpan(0, 100), "...") : documents;
    }

    public List<IBrowserFile>? FilesPendingUpload { get; set; }
    public bool IsPendingUpload { get; set; }

    public static TransactionModel FromTransactionTypeResponse(TransactionResponse response)
    {
        return new TransactionModel
        {
            TransactionId = response.TransactionId,
            TransactionTypeId = response.TransactionTypeId,
            TransactionType = TransactionTypeModel.FromTransactionTypeResponse(response.TransactionType),
            ProviderId = response.ProviderId,
            Provider = ProviderModel.FromProviderResponse(response.Provider),
            PersonId = response.PersonId,
            Person = response.Person != null ? PersonModel.FromPersonResponse(response.Person) : null,
            Date = response.Date,
            Amount = response.Amount,
            IsPaid = response.IsPaid,
            IsHsaWithdrawn = response.IsHsaWithdrawn,
            IsAudited = response.IsAudited,
            Documents = [..response.Documents.Select(DocumentModel.FromDocumentResponse)],
            AllowDelete = response.AllowDelete,
            CreatedTime = response.CreatedTime,
            CreatedBy = response.CreatedBy,
            LastUpdatedTime = response.LastUpdatedTime,
            LastUpdatedBy = response.LastUpdatedBy,
        };
    }
}