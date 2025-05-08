namespace HsaLedger.Client.Common;

public class DataLoadState<T>
{
    public List<T> Items { get; set; } = [];
    public bool IsLoading { get; set; } = true;
    public bool HasError { get; set; } = false;
    public string? ErrorMessage { get; set; }
}