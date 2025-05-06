namespace HsaLedger.Application.Requests;

public class SetProviderRequest
{
    public int ProviderId { get; set; }
    public required string Name { get; set; }
}