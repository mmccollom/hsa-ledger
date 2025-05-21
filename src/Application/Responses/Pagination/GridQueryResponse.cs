namespace HsaLedger.Application.Responses.Pagination;

public class GridQueryResponse<T>
{
    public int TotalItems { get; set; }
    public IEnumerable<T>? Items { get; set; }
}