namespace HsaLedger.Domain.Common.Model;

public class FilterDefinition
{
    public string Property { get; set; } = string.Empty;    // Property name
    public string Operator { get; set; } = "Contains";      // "Contains", "Equals", "StartsWith", etc.
    public string? Value { get; set; }                      // Raw value as string (to be parsed if needed)
}