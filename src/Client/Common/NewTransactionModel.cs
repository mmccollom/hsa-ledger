using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common;

public class NewTransactionModel
{
    public TransactionTypeResponse TransactionType { get; set; } = null!;
    public ProviderResponse Provider { get; set; } = null!;
    public PersonResponse? Person { get; set; }
    public DateTime? Date { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public bool IsHsaWithdrawn { get; set; }
    public bool IsAudited { get; set; }
}