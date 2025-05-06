namespace HsaLedger.Application.Requests;

public class SetTransactionTypeRequest
{
    public int TransactionTypeId { get; set; }
    public required string Code { get; set; }
    public required string Description { get; set; }
}