namespace HsaLedger.Client.Common.Models;

public class NewTransactionTypeModel
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<ProviderModel> Providers { get; set; } = [];
}