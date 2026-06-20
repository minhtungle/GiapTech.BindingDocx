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
                    KeyCount = keys.Count
                });
            }
            else if (ext == ".xlsx")
            {
                var (singles, table) = ExtractXlsxKeys(file);
                var singleList = singles.ToList();
                foreach (var key in singleList) singleFields.Add(key);
                if (table != null) tableFiles.Add(table);
                fileInfos.Add(new TemplateFileDto
                {
                    FileName = Path.GetFileName(file),
                    DisplayName = Path.GetFileNameWithoutExtension(file).Replace("_", " "),
                    FileType = "xlsx",
                    KeyCount = table?.Columns.Count ?? singleList.Count
                });
            }
        }

        // Remove table columns from single fields (table keys are handled separately)
        var tableColumns = tableFiles.SelectMany(t => t.Columns).ToHashSet();
        var cleanedSingleFields = singleFields.Where(k => !tableColumns.Contains(k)).ToList();

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

            // Merge text runs per paragraph then extract keys
            foreach (var para in body.Descendants<Paragraph>())
            {
                var text = string.Concat(para.Descendants<Text>().Select(t => t.Text));
                foreach (Match m in KeyPattern.Matches(text))
                    keys.Add(m.Groups[1].Value);
            }

            // Headers and footers
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

    private (IEnumerable<string> Singles, TableFileInfoDto? Table) ExtractXlsxKeys(string filePath)
    {
        var singles = new HashSet<string>();
        TableFileInfoDto? tableInfo = null;

        try
        {
            using var wb = new XLWorkbook(filePath);
            foreach (var ws in wb.Worksheets)
            {
                var tableColumns = new List<string>();

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
                        if (m.Success) singles.Add(m.Groups[1].Value);
                    }
                }

                if (tableColumns.Count > 0 && tableInfo == null)
                {
                    tableInfo = new TableFileInfoDto
                    {
                        FileName = Path.GetFileName(filePath),
                        DisplayName = Path.GetFileNameWithoutExtension(filePath).Replace("_", " "),
                        SheetName = ws.Name,
                        Columns = tableColumns
                    };
                }
            }
        }
        catch { /* skip unreadable files */ }

        return (singles, tableInfo);
    }
}
