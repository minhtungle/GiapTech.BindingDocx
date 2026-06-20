using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using GiapTech.BindingDocx.Application.Workspace.DTOs;
using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using System.Text.RegularExpressions;

namespace GiapTech.BindingDocx.Infrastructure.Services.Workspace;

public class TemplateKeyExtractor : ITemplateKeyExtractor
{
    private static readonly Regex KeyPattern = new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);

    public GroupKeysDto ExtractKeys(string groupPath)
    {
        var singleFields = new HashSet<string>();
        var tableFiles = new List<TableFileInfoDto>();
        var fileInfos = new List<TemplateFileDto>();

        var files = Directory.GetFiles(groupPath)
            .Where(f => f.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                     || f.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f)
            .ToList();

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file).ToLowerInvariant();
            if (ext == ".docx")
            {
                var keys = ExtractDocxKeys(file).ToList();
                foreach (var key in keys) singleFields.Add(key);
                fileInfos.Add(new TemplateFileDto
                {
                    FileName = Path.GetFileName(file),
                    DisplayName = Path.GetFileNameWithoutExtension(file).Replace("_", " "),
                    FileType = "docx",
                    KeyCount = keys.Count,
                    Keys = keys
                });
            }
            else if (ext == ".xlsx")
            {
                var (singles, tables) = ExtractXlsxKeys(file);
                var singleList = singles.ToList();
                foreach (var key in singleList) singleFields.Add(key);
                tableFiles.AddRange(tables);

                var allXlsxKeys = singleList
                    .Concat(tables.SelectMany(t => t.Columns))
                    .Distinct()
                    .ToList();

                fileInfos.Add(new TemplateFileDto
                {
                    FileName = Path.GetFileName(file),
                    DisplayName = Path.GetFileNameWithoutExtension(file).Replace("_", " "),
                    FileType = "xlsx",
                    KeyCount = allXlsxKeys.Count,
                    Keys = singleList
                });
            }
        }

        var tableColumns = tableFiles.SelectMany(t => t.Columns).ToHashSet();
        var cleanedSingleFields = singleFields.Where(k => !tableColumns.Contains(k)).ToList();

        // Remove table column keys from each docx file's Keys as well
        foreach (var fi in fileInfos.Where(f => f.FileType == "docx"))
            fi.Keys = fi.Keys.Where(k => !tableColumns.Contains(k)).ToList();

        return new GroupKeysDto
        {
            SingleFields = cleanedSingleFields,
            TableFiles = tableFiles,
            Files = fileInfos
        };
    }

    private IEnumerable<string> ExtractDocxKeys(string filePath)
    {
        var keys = new HashSet<string>();
        try
        {
            using var doc = WordprocessingDocument.Open(filePath, false);
            var body = doc.MainDocumentPart!.Document.Body!;

            foreach (var para in body.Descendants<Paragraph>())
            {
                var text = string.Concat(para.Descendants<Text>().Select(t => t.Text));
                foreach (Match m in KeyPattern.Matches(text))
                    keys.Add(m.Groups[1].Value);
            }

            foreach (var headerPart in doc.MainDocumentPart.HeaderParts)
            {
                var text = string.Concat(headerPart.Header.Descendants<Text>().Select(t => t.Text));
                foreach (Match m in KeyPattern.Matches(text))
                    keys.Add(m.Groups[1].Value);
            }
            foreach (var footerPart in doc.MainDocumentPart.FooterParts)
            {
                var text = string.Concat(footerPart.Footer.Descendants<Text>().Select(t => t.Text));
                foreach (Match m in KeyPattern.Matches(text))
                    keys.Add(m.Groups[1].Value);
            }
        }
        catch { /* skip unreadable files */ }
        return keys;
    }

    private (IEnumerable<string> Singles, List<TableFileInfoDto> Tables) ExtractXlsxKeys(string filePath)
    {
        var singles = new HashSet<string>();
        var tables = new List<TableFileInfoDto>();

        try
        {
            using var wb = new XLWorkbook(filePath);
            foreach (var ws in wb.Worksheets)
            {
                var tableColumns = new List<string>();
                var sheetSingles = new HashSet<string>();

                foreach (var row in ws.RowsUsed())
                {
                    var keyCells = row.CellsUsed()
                        .Where(c => KeyPattern.IsMatch(c.Value.ToString() ?? ""))
                        .ToList();

                    if (keyCells.Count >= 2)
                    {
                        foreach (var cell in keyCells)
                        {
                            var m = KeyPattern.Match(cell.Value.ToString() ?? "");
                            if (m.Success) tableColumns.Add(m.Groups[1].Value);
                        }
                    }
                    else if (keyCells.Count == 1)
                    {
                        var m = KeyPattern.Match(keyCells[0].Value.ToString() ?? "");
                        if (m.Success) sheetSingles.Add(m.Groups[1].Value);
                    }
                }

                foreach (var k in sheetSingles) singles.Add(k);

                if (tableColumns.Count > 0)
                {
                    tables.Add(new TableFileInfoDto
                    {
                        FileName = Path.GetFileName(filePath),
                        SheetName = ws.Name,
                        Columns = tableColumns
                    });
                }
            }
        }
        catch { /* skip unreadable files */ }

        // Assign DisplayName and TableKey based on count
        var displayBase = Path.GetFileNameWithoutExtension(filePath).Replace("_", " ");
        var fileBase = Path.GetFileNameWithoutExtension(filePath);

        if (tables.Count == 1)
        {
            tables[0].DisplayName = displayBase;
            tables[0].TableKey = Path.GetFileName(filePath);
        }
        else
        {
            for (int i = 0; i < tables.Count; i++)
            {
                tables[i].DisplayName = $"{displayBase} - Bảng {i + 1}";
                tables[i].TableKey = $"{fileBase}_bang_{i + 1}.xlsx";
            }
        }

        return (singles, tables);
    }
}
