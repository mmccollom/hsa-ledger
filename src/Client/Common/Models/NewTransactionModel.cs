using HsaLedger.Domain.Common.Model;
using Microsoft.AspNetCore.Components.Forms;

namespace HsaLedger.Client.Common.Models;

public class NewTransactionModel
{
    public TransactionTypeModel TransactionType { get; set; } = null!;
    public ProviderModel Provider { get; set; } = null!;
    public PersonModel? Person { get; set; }
    public DateTime? Date { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public bool IsHsaWithdrawn { get; set; }
    public bool IsAudited { get; set; }
    public List<IBrowserFile>? FilesPendingUpload { get; set; }
}