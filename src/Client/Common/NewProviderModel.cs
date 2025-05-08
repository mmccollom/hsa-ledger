using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common;

public class NewProviderModel
{
    public string Name { get; set; } = string.Empty;
    public IEnumerable<TransactionTypeResponse> TransactionTypes { get; set; } = [];
}