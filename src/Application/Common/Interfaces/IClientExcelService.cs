using System.Data;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Application.Common.Interfaces;

public interface IClientExcelService
{
    Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(Stream stream,
        Dictionary<string, Func<DataRow, TEntity, object>> mappers, List<string> sheetNames);

    Task<string> ExportAsync<TData>(IEnumerable<TData> data, Dictionary<string, Func<TData, object>> mappers,
        string sheetName = "Sheet1");
}