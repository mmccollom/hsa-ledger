using HsaLedger.Domain.Common.Model;

namespace HsaLedger.Application.Requests;

public class GridQueryRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<FilterDefinition>? Filters { get; set; }
    public List<SortDefinition>? Sorts { get; set; }
}