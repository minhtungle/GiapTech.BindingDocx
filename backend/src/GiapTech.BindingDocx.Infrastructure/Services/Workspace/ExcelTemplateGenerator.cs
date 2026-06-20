using ClosedXML.Excel;
using GiapTech.BindingDocx.Application.Workspace.DTOs;
using GiapTech.BindingDocx.Application.Workspace.Interfaces;

namespace GiapTech.BindingDocx.Infrastructure.Services.Workspace;

public class ExcelTemplateGenerator : IExcelTemplateGenerator
{
    private const string SingleFieldsSheet = "Thong tin chung";

    public byte[] GenerateTemplate(GroupKeysDto keys)
    {
        using var wb = new XLWorkbook();

        // Sheet 1: single fields
        var ws = wb.Worksheets.Add(SingleFieldsSheet);
        StyleHeaderRow(ws.Row(1));
        ws.Cell(1, 1).Value = "Trường thông tin";
        ws.Cell(1, 2).Value = "Giá trị";
        ws.Column(1).Width = 35;
        ws.Column(2).Width = 50;

        int row = 2;
        foreach (var field in keys.SingleFields)
        {
            ws.Cell(row, 1).Value = field;
            ws.Cell(row, 2).Value = "";
            ws.Row(row).Cell(1).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
            row++;
        }
        ws.SheetView.FreezeRows(1);

        // One sheet per xlsx table file — sheet named after TableKey base so ExcelDataParser matches
        foreach (var tableFile in keys.TableFiles)
        {
            var baseName = Path.GetFileNameWithoutExtension(tableFile.TableKey);
            var sheetName = baseName.Length > 31 ? baseName[..31] : baseName;
            var tws = wb.Worksheets.Add(sheetName);

            StyleHeaderRow(tws.Row(1));
            for (int i = 0; i < tableFile.Columns.Count; i++)
            {
                tws.Cell(1, i + 1).Value = tableFile.Columns[i];
                tws.Column(i + 1).Width = 20;
            }

            // 10 empty rows for data
            for (int r = 2; r <= 11; r++)
                for (int c = 1; c <= tableFile.Columns.Count; c++)
                    tws.Cell(r, c).Value = "";

            tws.SheetView.FreezeRows(1);
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private static void StyleHeaderRow(IXLRow row)
    {
        row.Style.Font.Bold = true;
        row.Style.Fill.BackgroundColor = XLColor.FromArgb(31, 78, 121);
        row.Style.Font.FontColor = XLColor.White;
        row.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    }
}
