namespace HsaLedger.Application.Responses.SimpleDto;

public class DashboardResponse
{
    public required decimal TotalSpent { get; set; }
    public required decimal Paid { get; set; }
    public required decimal Unpaid { get; set; }
    public required decimal Withdrawn { get; set; }
    public required decimal NotWithdrawn { get; set; }
    public required int CountOfTransactions { get; set; }
}