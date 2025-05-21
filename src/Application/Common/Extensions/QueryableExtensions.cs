using System.Linq.Dynamic.Core;

namespace HsaLedger.Application.Common.Extensions;

internal static class QueryableExtensions
{
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string property, bool ascending)
    {
        if (string.IsNullOrWhiteSpace(property)) return query;
        return query.OrderBy($"{property} {(ascending ? "ascending" : "descending")}");
    }
}