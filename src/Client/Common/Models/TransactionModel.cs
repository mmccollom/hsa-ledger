using System.Linq.Expressions;
using HsaLedger.Application.Responses.Projections;
using Microsoft.AspNetCore.Components.Forms;

namespace HsaLedger.Client.Common.Models;

public class TransactionModel
{
    public int TransactionId { get; set; }
    public int TransactionTypeId { get; set; }
    public TransactionTypeModel TransactionType { get; set; } = null!;
    public int ProviderId { get; set; }
    public ProviderModel Provider { get; set; } = null!;
    public int? PersonId { get; set; }
    public PersonModel? Person { get; set; }

    public string GetPersonName()
    {
        return Person?.Name ?? string.Empty;
    }
    public DateTime? Date { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public bool IsHsaWithdrawn { get; set; }
    public bool IsAudited { get; set; }
    public required List<DocumentModel> Documents { get; set; }
    public bool AllowDelete { get; set; }
    public bool IsDocumentAvailable { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }

    public string GetDocumentNames()
    {
        var documents = string.Join(",", Documents.Select(t => t.Fullname));
        return documents.Length > 100 ? string.Concat(documents.AsSpan(0, 100), "...") : documents;
    }
    
    public List<IBrowserFile>? FilesPendingUpload { get; set; }
    public bool IsPendingUpload { get; set; }

    public static Expression<Func<TransactionResponse, TransactionModel>> Projection
    {
        get
        {
            return x => new TransactionModel
            {
                TransactionId = x.TransactionId,
                TransactionTypeId = x.TransactionTypeId,
                TransactionType = TransactionTypeModel.FromResponse(x.TransactionType),
                ProviderId = x.ProviderId,
                Provider = ProviderModel.FromResponse(x.Provider),
                PersonId = x.PersonId,
                Person = x.Person != null ? PersonModel.FromResponse(x.Person) : null,
                Date = x.Date,
                Amount = x.Amount,
                IsPaid = x.IsPaid,
                IsHsaWithdrawn = x.IsHsaWithdrawn,
                IsAudited = x.IsAudited,
                Documents = x.Documents.AsQueryable().Select(DocumentModel.Projection).ToList(),
                IsDocumentAvailable = x.Documents.Count != 0,
                AllowDelete = true,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
            };
        }
    }
}