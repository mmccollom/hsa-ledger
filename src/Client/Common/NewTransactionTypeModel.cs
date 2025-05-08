using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common;

public class NewTransactionTypeModel
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<ProviderResponse> Providers { get; set; } = [];
}