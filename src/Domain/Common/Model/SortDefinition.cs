namespace HsaLedger.Domain.Common.Model;

public class SortDefinition
{
    public string Property { get; set; } = string.Empty; // Name of the property to sort by
    public string Direction { get; set; } = "Ascending"; // "Ascending" or "Descending"
}
