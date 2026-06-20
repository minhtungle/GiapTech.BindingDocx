using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace GiapTech.BindingDocx.Infrastructure.Services.Workspace;

public class TemplateRenderer : ITemplateRenderer
{
    private static readonly Regex KeyPattern = new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);

    public byte[] RenderDocx(string templatePath, Dictionary<string, string> data)
    {
        var templateBytes = File.ReadAllBytes(templatePath);
        using var ms = new MemoryStream();
        ms.Write(templateBytes, 0, templateBytes.Length);
        ms.Position = 0;

        using var doc = WordprocessingDocument.Open(ms, true);
        var body = doc.MainDocumentPart!.Document.Body!;

        foreach (var para in body.Descendants<Paragraph>().ToList())
            ProcessParagraph(para, data);

        foreach (var headerPart in doc.MainDocumentPart.HeaderParts)
            foreach (var para in headerPart.Header.Descendants<Paragraph>().ToList())
                ProcessParagraph(para, data);

        foreach (var footerPart in doc.MainDocumentPart.FooterParts)
            foreach (var para in footerPart.Footer.Descendants<Paragraph>().ToList())
                ProcessParagraph(para, data);

        doc.Save();
        return ms.ToArray();
    }

    public byte[] RenderXlsx(string templatePath, Dictionary<string, string> singleFields,
        Dictionary<string, List<Dictionary<string, string>>>? tableDataBySheet = null)
    {
        var templateBytes = File.ReadAllBytes(templatePath);
        using var ms = new MemoryStream(templateBytes);
        using var wb = new XLWorkbook(ms);

        foreach (var ws in wb.Worksheets)
        {
            // Resolve table data for this sheet via TableKey lookup
            List<Dictionary<string, string>>? tableData = null;
            if (tableDataBySheet != null)
                tableDataBySheet.TryGetValue(ws.Name, out tableData);

            // Find table template row (row with >= 2 {{key}} cells)
            IXLRow? templateRow = null;
            foreach (var row in ws.RowsUsed().ToList())
            {
                var keyCells = row.CellsUsed()
                    .Where(c => KeyPattern.IsMatch(c.Value.ToString() ?? ""))
                    .ToList();
                if (keyCells.Count >= 2)
                {
                    templateRow = row;
                    break;
                }
            }

            if (templateRow != null && tableData?.Count > 0)
            {
                // Build column → key mapping
                var colMap = new Dictionary<int, string>();
                foreach (var cell in templateRow.CellsUsed())
                {
                    var m = KeyPattern.Match(cell.Value.ToString() ?? "");
                    if (m.Success) colMap[cell.Address.ColumnNumber] = m.Groups[1].Value;
                }

                var insertAt = templateRow.RowNumber();
                ws.Row(insertAt).InsertRowsAbove(tableData.Count);

                var newTemplateRowNum = insertAt + tableData.Count;
                for (int i = 0; i < tableData.Count; i++)
                {
                    var dataRow = tableData[i];
                    foreach (var (colNum, key) in colMap)
                    {
                        var srcCell = ws.Cell(newTemplateRowNum, colNum);
                        var dstCell = ws.Cell(insertAt + i, colNum);
                        dstCell.Value = dataRow.TryGetValue(key, out var val) ? val : "";
                        dstCell.Style = srcCell.Style;
                    }
                }

                ws.Row(newTemplateRowNum).Delete();
            }

            // Replace single field cells
            foreach (var cell in ws.CellsUsed().ToList())
            {
                var val = cell.Value.ToString() ?? "";
                if (!val.Contains("{{")) continue;
                foreach (var (key, value) in singleFields)
                    val = val.Replace($"{{{{{key}}}}}", value);
                cell.Value = val;
            }
        }

        using var output = new MemoryStream();
        wb.SaveAs(output);
        return output.ToArray();
    }

    public byte[] CreateZip(Dictionary<string, byte[]> files)
    {
        using var zipMs = new MemoryStream();
        using (var zip = new ZipArchive(zipMs, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (fileName, content) in files)
            {
                var entry = zip.CreateEntry(fileName, CompressionLevel.Fastest);
                using var entryStream = entry.Open();
                entryStream.Write(content, 0, content.Length);
            }
        }
        return zipMs.ToArray();
    }

    private static void ProcessParagraph(Paragraph para, Dictionary<string, string> data)
    {
        var fullText = string.Concat(para.Descendants<Text>().Select(t => t.Text));
        if (!fullText.Contains("{{")) return;

        foreach (var (key, value) in data)
            fullText = fullText.Replace($"{{{{{key}}}}}", value);

        // Preserve formatting from first run
        var firstRun = para.Descendants<Run>().FirstOrDefault();
        RunProperties? runProps = null;
        if (firstRun?.RunProperties != null)
            runProps = (RunProperties)firstRun.RunProperties.CloneNode(true);

        // Clear all runs
        foreach (var run in para.Descendants<Run>().ToList())
            run.Remove();

        // Add single run with replaced text
        var newRun = new Run();
        if (runProps != null) newRun.AppendChild(runProps);
        newRun.AppendChild(new Text(fullText) { Space = SpaceProcessingModeValues.Preserve });
        para.AppendChild(newRun);
    }
}
