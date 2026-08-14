using System.Globalization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EtykietyIT.Models;
using SpreadsheetFont = DocumentFormat.OpenXml.Spreadsheet.Font;

namespace EtykietyIT.Export;

public sealed class XlsxHistoryExporter : IHistoryExporter
{
    public const string WorksheetName = "Historia wydruków";

    private const uint DefaultStyleIndex = 0;
    private const uint HeaderStyleIndex = 1;
    private const uint DateTimeStyleIndex = 2;
    private const uint IntegerStyleIndex = 3;
    private const uint NumberStyleIndex = 4;
    private const uint WrappedTextStyleIndex = 5;

    public Task ExportAsync(
        IEnumerable<PrintHistoryEntry> entries,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        PrintHistoryEntry[] entriesToExport =
            HistoryExportSchema.MaterializeAndValidate(entries);
        string fullPath = Path.GetFullPath(filePath);

        return Task.Run(
            () => CreateWorkbook(
                entriesToExport,
                fullPath,
                cancellationToken),
            cancellationToken);
    }

    private static void CreateWorkbook(
        IReadOnlyList<PrintHistoryEntry> entries,
        string filePath,
        CancellationToken cancellationToken)
    {
        using SpreadsheetDocument document = SpreadsheetDocument.Create(
            filePath,
            SpreadsheetDocumentType.Workbook);

        WorkbookPart workbookPart = document.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        WorkbookStylesPart stylesPart =
            workbookPart.AddNewPart<WorkbookStylesPart>();
        stylesPart.Stylesheet = CreateStylesheet();
        stylesPart.Stylesheet.Save();

        WorksheetPart worksheetPart =
            workbookPart.AddNewPart<WorksheetPart>();
        var sheetData = new SheetData();
        worksheetPart.Worksheet = new Worksheet(
            CreateSheetViews(),
            CreateColumns(),
            sheetData,
            new AutoFilter
            {
                Reference = $"A1:{GetColumnName(HistoryExportSchema.Columns.Count)}" +
                    $"{entries.Count + 1}"
            });

        sheetData.Append(CreateHeaderRow());
        for (int entryIndex = 0; entryIndex < entries.Count; entryIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            sheetData.Append(CreateDataRow(entries[entryIndex], entryIndex + 2));
        }

        var sheets = workbookPart.Workbook.AppendChild(new Sheets());
        sheets.Append(new Sheet
        {
            Id = workbookPart.GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = WorksheetName
        });

        worksheetPart.Worksheet.Save();
        workbookPart.Workbook.Save();
    }

    private static SheetViews CreateSheetViews()
    {
        return new SheetViews(
            new SheetView(
                new Pane
                {
                    VerticalSplit = 1,
                    TopLeftCell = "A2",
                    ActivePane = PaneValues.BottomLeft,
                    State = PaneStateValues.Frozen
                })
            {
                WorkbookViewId = 0
            });
    }

    private static Columns CreateColumns()
    {
        var columns = new Columns();
        for (int index = 0; index < HistoryExportSchema.Columns.Count; index++)
        {
            double width = Math.Clamp(
                HistoryExportSchema.Columns[index].Width,
                8,
                40);
            columns.Append(new Column
            {
                Min = (uint)index + 1,
                Max = (uint)index + 1,
                Width = width,
                CustomWidth = true
            });
        }

        return columns;
    }

    private static Row CreateHeaderRow()
    {
        var row = new Row
        {
            RowIndex = 1,
            Height = 30,
            CustomHeight = true
        };

        for (int index = 0; index < HistoryExportSchema.Columns.Count; index++)
        {
            row.Append(CreateTextCell(
                GetCellReference(index, 1),
                HistoryExportSchema.Columns[index].Header,
                HeaderStyleIndex));
        }

        return row;
    }

    private static Row CreateDataRow(PrintHistoryEntry entry, int rowIndex)
    {
        var row = new Row { RowIndex = (uint)rowIndex };

        for (int columnIndex = 0;
            columnIndex < HistoryExportSchema.Columns.Count;
            columnIndex++)
        {
            HistoryExportColumn column =
                HistoryExportSchema.Columns[columnIndex];
            row.Append(CreateDataCell(
                GetCellReference(columnIndex, rowIndex),
                column,
                entry));
        }

        return row;
    }

    private static Cell CreateDataCell(
        string cellReference,
        HistoryExportColumn column,
        PrintHistoryEntry entry)
    {
        object? value = column.GetValue(entry);
        return column.ValueKind switch
        {
            HistoryExportValueKind.Text => CreateTextCell(
                cellReference,
                value as string ?? string.Empty,
                column.WrapText
                    ? WrappedTextStyleIndex
                    : DefaultStyleIndex),
            HistoryExportValueKind.LocalDateTime => CreateDateTimeCell(
                cellReference,
                ((DateTimeOffset)value!).ToLocalTime().DateTime),
            HistoryExportValueKind.UtcDateTime => CreateDateTimeCell(
                cellReference,
                ((DateTimeOffset)value!).UtcDateTime),
            HistoryExportValueKind.Integer => CreateNumberCell(
                cellReference,
                Convert.ToInt64(value, CultureInfo.InvariantCulture).ToString(
                    CultureInfo.InvariantCulture),
                IntegerStyleIndex),
            HistoryExportValueKind.Number => CreateNumberCell(
                cellReference,
                Convert.ToDouble(value, CultureInfo.InvariantCulture).ToString(
                    "R",
                    CultureInfo.InvariantCulture),
                NumberStyleIndex),
            HistoryExportValueKind.Boolean => CreateTextCell(
                cellReference,
                (bool)value! ? "Tak" : "Nie",
                DefaultStyleIndex),
            _ => throw new InvalidOperationException(
                $"Nieobsługiwany typ wartości eksportu: {column.ValueKind}.")
        };
    }

    private static Cell CreateTextCell(
        string cellReference,
        string value,
        uint styleIndex)
    {
        var text = new Text(value)
        {
            Space = SpaceProcessingModeValues.Preserve
        };
        return new Cell(new InlineString(text))
        {
            CellReference = cellReference,
            DataType = CellValues.InlineString,
            StyleIndex = styleIndex
        };
    }

    private static Cell CreateDateTimeCell(
        string cellReference,
        DateTime value)
    {
        return CreateNumberCell(
            cellReference,
            value.ToOADate().ToString("R", CultureInfo.InvariantCulture),
            DateTimeStyleIndex);
    }

    private static Cell CreateNumberCell(
        string cellReference,
        string value,
        uint styleIndex)
    {
        return new Cell(new CellValue(value))
        {
            CellReference = cellReference,
            DataType = CellValues.Number,
            StyleIndex = styleIndex
        };
    }

    private static Stylesheet CreateStylesheet()
    {
        var numberingFormats = new NumberingFormats(
            new NumberingFormat
            {
                NumberFormatId = 164,
                FormatCode = "yyyy-mm-dd hh:mm:ss"
            },
            new NumberingFormat
            {
                NumberFormatId = 165,
                FormatCode = "0.0###"
            })
        {
            Count = 2
        };

        var fonts = new Fonts(
            CreateDefaultFont(),
            new SpreadsheetFont(
                new Bold(),
                new FontSize { Val = 11 },
                new FontName { Val = "Calibri" }))
        {
            Count = 2
        };

        var fills = new Fills(
            new Fill(new PatternFill { PatternType = PatternValues.None }),
            new Fill(new PatternFill { PatternType = PatternValues.Gray125 }))
        {
            Count = 2
        };
        var borders = new Borders(new Border()) { Count = 1 };
        var cellStyleFormats = new CellStyleFormats(new CellFormat())
        {
            Count = 1
        };
        var cellFormats = new CellFormats(
            new CellFormat(),
            new CellFormat
            {
                FontId = 1,
                ApplyFont = true,
                ApplyAlignment = true,
                Alignment = new Alignment
                {
                    WrapText = true,
                    Vertical = VerticalAlignmentValues.Center
                }
            },
            new CellFormat
            {
                NumberFormatId = 164,
                ApplyNumberFormat = true
            },
            new CellFormat
            {
                NumberFormatId = 1,
                ApplyNumberFormat = true
            },
            new CellFormat
            {
                NumberFormatId = 165,
                ApplyNumberFormat = true
            },
            new CellFormat
            {
                ApplyAlignment = true,
                Alignment = new Alignment
                {
                    WrapText = true,
                    Vertical = VerticalAlignmentValues.Top
                }
            })
        {
            Count = 6
        };
        var cellStyles = new CellStyles(
            new CellStyle
            {
                Name = "Normal",
                FormatId = 0,
                BuiltinId = 0
            })
        {
            Count = 1
        };

        return new Stylesheet(
            numberingFormats,
            fonts,
            fills,
            borders,
            cellStyleFormats,
            cellFormats,
            cellStyles);
    }

    private static SpreadsheetFont CreateDefaultFont()
    {
        return new SpreadsheetFont(
            new FontSize { Val = 11 },
            new FontName { Val = "Calibri" });
    }

    private static string GetCellReference(int zeroBasedColumnIndex, int rowIndex)
    {
        return $"{GetColumnName(zeroBasedColumnIndex + 1)}{rowIndex}";
    }

    private static string GetColumnName(int oneBasedColumnIndex)
    {
        var name = new Stack<char>();
        int value = oneBasedColumnIndex;
        while (value > 0)
        {
            value--;
            name.Push((char)('A' + (value % 26)));
            value /= 26;
        }

        return new string(name.ToArray());
    }
}
