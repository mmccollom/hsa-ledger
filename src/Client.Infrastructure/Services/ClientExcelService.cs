using System.Data;
using System.Globalization;
using ClosedXML.Excel;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Services;

public class ClientExcelService : IClientExcelService
{
    public async Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(Stream stream,
        Dictionary<string, Func<DataRow, TEntity, object>> mappers, List<string> sheetNames)
    {
        var result = new List<TEntity>();
        var wb = new XLWorkbook(stream);

        foreach (var sheetName in sheetNames)
        {
            var ws = wb.Worksheet(sheetName);

            var dt = new DataTable();
            var titlesInFirstRow = true;
            
            var range = ws.RangeUsed();
            if (range != null)
            {
                var firstRow = range.FirstRow();
                foreach (var firstRowCell in firstRow.CellsUsed())
                {
                    dt.Columns.Add(titlesInFirstRow
                        ? firstRowCell.Value.ToString()
                        : $"Column {firstRowCell.Address.ColumnNumber}");
                }
            }
            else
            {
                // Handle the case where the worksheet is empty (log, throw, etc.)
                Console.WriteLine("Worksheet is empty, no data to load.");
            }

            var startRow = titlesInFirstRow ? 2 : 1;
            var headers = mappers.Keys.Select(x => x).ToList();
            var errors = new List<string>();
            foreach (var header in headers)
            {
                if (!dt.Columns.Contains(header))
                {
                    errors.Add($"Header '{header}' does not exist in table!");
                }
            }

            if (errors.Any())
            {
                return await Result<IEnumerable<TEntity>>.FailAsync(errors);
            }

            var lastRow = ws.LastRowUsed()?.RowNumber();
            for (var rowNum = startRow; rowNum <= lastRow; rowNum++)
            {
                try
                {
                    var wsRow = ws.Row(rowNum).CellsUsed();

                    DataRow row = dt.Rows.Add();
                    var item = (TEntity)Activator.CreateInstance(typeof(TEntity))!;
                    foreach (var cell in wsRow)
                    {
                        if (cell.DataType == XLDataType.DateTime)
                        {
                            DateTime dateTimeValue = cell.GetDateTime();
                            row[cell.WorksheetColumn().ColumnNumber() - 1] =
                                dateTimeValue.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            row[cell.WorksheetColumn().ColumnNumber() - 1] = cell.Value.ToString();
                        }
                    }

                    headers.ForEach(x => mappers[x](row, item));
                    result.Add(item);
                }
                catch (Exception e)
                {
                    return await Result<IEnumerable<TEntity>>.FailAsync(e.Message);
                }
            }
        }

        return await Result<IEnumerable<TEntity>>.SuccessAsync(result, "Import Success");
    }

    public async Task<string> ExportAsync<TData>(IEnumerable<TData> data, Dictionary<string, Func<TData, object>> mappers,
        string sheetName = "Sheet1")
    {
        using var ms = new MemoryStream();
        var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(sheetName);

        ws.Style.Font.FontSize = 11;
        ws.Style.Font.FontName = "Calibri";

        var colIndex = 1;
        var rowIndex = 1;

        var headers = mappers.Keys.Select(x => x).ToList();

        foreach (var header in headers)
        {
            var cell = ws.Cell(rowIndex, colIndex);

            var fill = cell.Style.Fill;
            fill.PatternType = XLFillPatternValues.Solid;
            fill.BackgroundColor = XLColor.LightBlue;

            var border = cell.Style.Border;
            border.BottomBorder = XLBorderStyleValues.Thin;
            border.TopBorder = XLBorderStyleValues.Thin;
            border.LeftBorder = XLBorderStyleValues.Thin;
            border.RightBorder = XLBorderStyleValues.Thin;

            cell.Value = header;

            colIndex++;
        }

        var dataList = data.ToList();
        foreach (var item in dataList)
        {
            colIndex = 1;
            rowIndex++;

            var result = headers.Select(header => mappers[header](item));

            foreach (var value in result)
            {
                var cell = ws.Cell(rowIndex, colIndex++);

                switch (value)
                {
                    case string s:
                        cell.SetValue(s);
                        break;
                    case int i:
                        cell.SetValue(i);
                        break;
                    case decimal d:
                        cell.SetValue(d);
                        break;
                    case double dbl:
                        cell.SetValue(dbl);
                        break;
                    case bool b:
                        cell.SetValue(b);
                        break;
                    case DateTime dt:
                        cell.SetValue(dt);
                        cell.Style.DateFormat.Format = "yyyy-mm-dd"; // Optional: set Excel-friendly format
                        break;
                    case null:
                        cell.Clear(); // or SetValue(string.Empty)
                        break;
                    default:
                        cell.SetValue(value.ToString());
                        break;
                }
            }
        }

        var range = ws.Range(1, 1, dataList.Count + 1, headers.Count);
        range.SetAutoFilter(true);
        wb.SaveAs(ms);

        var byteArray = ms.ToArray();
        return await Task.FromResult(Convert.ToBase64String(byteArray));
    }
}