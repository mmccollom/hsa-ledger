namespace HsaLedger.Application.Requests;

public class AddProviderRequest
{
    public required string Name { get; set; }
    public required List<int> TransactionTypeIds { get; set; }
}