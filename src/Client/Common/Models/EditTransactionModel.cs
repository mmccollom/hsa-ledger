using HsaLedger.Application.Responses.Models;

namespace HsaLedger.Client.Common.Models;

public class EditTransactionModel
{
    public TransactionTypeModel? TransactionType { get; set; }
    public ProviderModel? Provider { get; set; }
    public PersonModel? Person { get; set; }
    public DateTime? Date { get; set; }
    
    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set
        {
            _amount = value;
            _isAmountDirty = true;
        }
    }
    private bool _isAmountDirty = false;
    public bool IsAmountDirty => _isAmountDirty;
    public bool? IsPaid { get; set; }
    public bool? IsHsaWithdrawn { get; set; }
    public bool? IsAudited { get; set; }
}