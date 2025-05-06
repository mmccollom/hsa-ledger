namespace HsaLedger.Application.Requests;

public class AddTransactionRequest
{
    public int TransactionTypeId { get; set; }
    public int ProviderId { get; set; }
    public int? PersonId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public bool IsHsaWithdrawn { get; set; }
    public bool IsAudited { get; set; }
}