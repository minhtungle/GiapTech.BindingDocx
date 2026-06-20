using ClosedXML.Excel;
using GiapTech.BindingDocx.Application.Workspace.DTOs;
using GiapTech.BindingDocx.Application.Workspace.Interfaces;

namespace GiapTech.BindingDocx.Infrastructure.Services.Workspace;

public class ExcelDataParser : IExcelDataParser
{
    private const string SingleFieldsSheet = "Thong tin chung";

    public ImportDataResultDto Parse(Stream fileStream)
    {
        var singleFields = new Dictionary<string, string>();
        var tableData = new Dictionary<string, List<Dictionary<string, string>>>();
        int totalRows = 0;

        using var wb = new XLWorkbook(fileStream);

        foreach (var ws in wb.Worksheets)
        {
            if (ws.Name.Equals(SingleFieldsSheet, StringComparison.OrdinalIgnoreCase))
            {
                // Col A = key, Col B = value, skip header row 1
                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    var key = row.Cell(1).Value.ToString()?.Trim() ?? "";
                    var value = row.Cell(2).Value.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(key))
                        singleFields[key] = value;
                }
            }
            else
            {
                // Row 1 = headers, rows 2+ = data
                var headerRow = ws.Row(1);
                var headers = new List<string>();
                var col = 1;
                while (!headerRow.Cell(col).IsEmpty())
                {
                    headers.Add(headerRow.Cell(col).Value.ToString()?.Trim() ?? $"col{col}");
                    col++;
                }

                if (headers.Count == 0) continue;

                var rows = new List<Dictionary<string, string>>();
                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    var rowData = new Dictionary<string, string>();
                    bool hasData = false;
                    for (int c = 0; c < headers.Count; c++)
                    {
                        var val = row.Cell(c + 1).Value.ToString()?.Trim() ?? "";
                        rowData[headers[c]] = val;
                        if (!string.IsNullOrEmpty(val)) hasData = true;
                    }
                    if (hasData)
                    {
                        rows.Add(rowData);
                        totalRows++;
                    }
                }

                if (rows.Count > 0)
                {
                    var fileName = ws.Name + ".xlsx";
                    tableData[fileName] = rows;
                }
            }
        }

        return new ImportDataResultDto
        {
            SingleFields = singleFields,
            TableData = tableData,
            TotalRows = Math.Max(1, totalRows)
        };
    }
}
