namespace HsaLedger.Application.Requests;

public class AddTransactionTypeRequest
{
    public required string Code { get; set; }
    public required string Description { get; set; }
    public required IEnumerable<int> ProviderIds { get; set; }
}