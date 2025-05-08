namespace HsaLedger.Application.Requests;

public class SetProviderRequest
{
    public int ProviderId { get; set; }
    public required string Name { get; set; }
    public required List<int> TransactionTypeIds { get; set; }
}