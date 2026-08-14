using System.Globalization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Validation;
using EtykietyIT.Export;
using EtykietyIT.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Export;

[TestClass]
public sealed class XlsxHistoryExporterTests
{
    private static readonly string[] ExpectedHeaders =
    {
        "ID wpisu",
        "Data lokalna",
        "Data UTC",
        "Wersja aplikacji",
        "Pierwszy Asset ID",
        "Ostatni Asset ID",
        "Numer początkowy",
        "Numer końcowy",
        "Liczba małych etykiet",
        "Liczba fizycznych etykiet",
        "ID organizacji",
        "Organizacja",
        "Firma",
        "Prefiks",
        "Liczba cyfr",
        "Drukarka",
        "Korekta X [mm]",
        "Korekta Y [mm]",
        "ID profilu",
        "Nazwa profilu",
        "Szerokość [mm]",
        "Wysokość [mm]",
        "Kolumny",
        "Wiersze",
        "Linie cięcia",
        "QR"
    };

    [TestMethod]
    public async Task ExportAsync_CreatesValidWorkbookWithExpectedSheet()
    {
        await WithExportFileAsync(async filePath =>
        {
            await new XlsxHistoryExporter().ExportAsync(
                new[] { CreateEntry(1, 1) },
                filePath);

            Assert.IsTrue(File.Exists(filePath));
            Assert.IsGreaterThan(0, new FileInfo(filePath).Length);

            using SpreadsheetDocument document = OpenWorkbook(filePath);
            Sheet sheet = GetSheet(document);
            ValidationErrorInfo[] errors = new OpenXmlValidator()
                .Validate(document)
                .ToArray();

            Assert.AreEqual(XlsxHistoryExporter.WorksheetName, sheet.Name?.Value);
            Assert.HasCount(0, errors, string.Join(
                Environment.NewLine,
                errors.Select(error => error.Description)));
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesExpectedHeaders()
    {
        await WithExportFileAsync(async filePath =>
        {
            await new XlsxHistoryExporter().ExportAsync(
                Array.Empty<PrintHistoryEntry>(),
                filePath);

            using SpreadsheetDocument document = OpenWorkbook(filePath);
            Row headerRow = GetRows(document).Single();
            string[] headers = headerRow.Elements<Cell>()
                .Select(ReadText)
                .ToArray();

            CollectionAssert.AreEqual(ExpectedHeaders, headers);
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesSingleEntry()
    {
        await WithExportFileAsync(async filePath =>
        {
            PrintHistoryEntry entry = CreateEntry(123, 4);

            await new XlsxHistoryExporter().ExportAsync(new[] { entry }, filePath);

            using SpreadsheetDocument document = OpenWorkbook(filePath);
            Row[] rows = GetRows(document);

            Assert.HasCount(2, rows);
            Assert.AreEqual(entry.Id.ToString("D"), ReadCellText(document, "A2"));
            Assert.AreEqual("IT-000123", ReadCellText(document, "E2"));
            Assert.AreEqual("IT-000126", ReadCellText(document, "F2"));
            Assert.AreEqual("Tak", ReadCellText(document, "Y2"));
            Assert.AreEqual("Nie", ReadCellText(document, "Z2"));
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesMultipleEntriesInInputOrder()
    {
        await WithExportFileAsync(async filePath =>
        {
            PrintHistoryEntry[] entries =
            [
                CreateEntry(10, 1),
                CreateEntry(20, 2),
                CreateEntry(30, 3)
            ];

            await new XlsxHistoryExporter().ExportAsync(entries, filePath);

            using SpreadsheetDocument document = OpenWorkbook(filePath);
            Row[] dataRows = GetRows(document).Skip(1).ToArray();

            Assert.HasCount(entries.Length, dataRows);
            CollectionAssert.AreEqual(
                entries.Select(entry => entry.Id.ToString("D")).ToArray(),
                dataRows.Select(row => ReadText(row.Elements<Cell>().First()))
                    .ToArray());
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesOrganizationAndLegacyFallback()
    {
        await WithExportFileAsync(async filePath =>
        {
            const string organizationId =
                "organization.3a271df1-99e5-4fa5-bf37-27f323370c42";
            const string organizationName = "Oddział; \"Północ — Łódź\"";
            PrintHistoryEntry currentEntry = WithOrganization(
                CreateEntry(1, 1),
                organizationId,
                organizationName);
            PrintHistoryEntry legacyEntry = CreateEntry(2, 1);

            await new XlsxHistoryExporter().ExportAsync(
                new[] { currentEntry, legacyEntry },
                filePath);

            using SpreadsheetDocument document = OpenWorkbook(filePath);

            Assert.AreEqual(organizationId, ReadCellText(document, "K2"));
            Assert.AreEqual(organizationName, ReadCellText(document, "L2"));
            Assert.AreEqual(string.Empty, ReadCellText(document, "K3"));
            Assert.AreEqual("—", ReadCellText(document, "L3"));
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesDatesAsNumericExcelDates()
    {
        await WithExportFileAsync(async filePath =>
        {
            PrintHistoryEntry entry = CreateEntry(1, 1);

            await new XlsxHistoryExporter().ExportAsync(new[] { entry }, filePath);

            using SpreadsheetDocument document = OpenWorkbook(filePath);
            Cell localDateCell = GetCell(document, "B2");
            Cell utcDateCell = GetCell(document, "C2");

            Assert.AreEqual(CellValues.Number, localDateCell.DataType?.Value);
            Assert.AreEqual(CellValues.Number, utcDateCell.DataType?.Value);
            Assert.AreNotEqual(0U, localDateCell.StyleIndex?.Value ?? 0U);
            Assert.AreNotEqual(0U, utcDateCell.StyleIndex?.Value ?? 0U);
            AssertDateEquals(
                entry.TimestampUtc.ToLocalTime().DateTime,
                ReadExcelDate(localDateCell));
            AssertDateEquals(entry.TimestampUtc.UtcDateTime, ReadExcelDate(utcDateCell));
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesNumbersAsNumericCells()
    {
        await WithExportFileAsync(async filePath =>
        {
            PrintHistoryEntry entry = CreateEntry(123, 4);

            await new XlsxHistoryExporter().ExportAsync(new[] { entry }, filePath);

            using SpreadsheetDocument document = OpenWorkbook(filePath);
            string[] numericReferences =
            [
                "G2", "H2", "I2", "J2", "O2", "Q2", "R2", "U2", "V2",
                "W2", "X2"
            ];

            foreach (string reference in numericReferences)
            {
                Assert.AreEqual(
                    CellValues.Number,
                    GetCell(document, reference).DataType?.Value,
                    $"Komórka {reference} nie jest liczbowa.");
            }

            Assert.AreEqual(-0.4, ReadNumber(GetCell(document, "Q2")), 0.000001);
            Assert.AreEqual(0.0, ReadNumber(GetCell(document, "R2")), 0.000001);
            Assert.AreEqual(89.0, ReadNumber(GetCell(document, "U2")), 0.000001);
            Assert.AreEqual(41.0, ReadNumber(GetCell(document, "V2")), 0.000001);
            Assert.AreEqual(4.0, ReadNumber(GetCell(document, "I2")), 0.000001);
            Assert.AreEqual(2.0, ReadNumber(GetCell(document, "J2")), 0.000001);
        });
    }

    [TestMethod]
    public async Task ExportAsync_AddsFilterFrozenHeaderAndBoundedWidths()
    {
        await WithExportFileAsync(async filePath =>
        {
            await new XlsxHistoryExporter().ExportAsync(
                new[] { CreateEntry(1, 1) },
                filePath);

            using SpreadsheetDocument document = OpenWorkbook(filePath);
            Worksheet worksheet = GetWorksheet(document);
            Pane? pane = worksheet.GetFirstChild<SheetViews>()?
                .GetFirstChild<SheetView>()?
                .GetFirstChild<Pane>();
            Column[] columns = worksheet.GetFirstChild<Columns>()!
                .Elements<Column>()
                .ToArray();

            Assert.AreEqual("A1:Z2", worksheet.GetFirstChild<AutoFilter>()?
                .Reference?.Value);
            Assert.AreEqual(PaneStateValues.Frozen, pane?.State?.Value);
            Assert.AreEqual(1D, pane?.VerticalSplit?.Value);
            Assert.HasCount(ExpectedHeaders.Length, columns);
            Assert.IsTrue(columns.All(column => column.CustomWidth?.Value == true));
            Assert.IsTrue(columns.All(column => column.Width?.Value <= 40));
            Assert.AreNotEqual(0U, GetCell(document, "A1").StyleIndex?.Value ?? 0U);
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesHeaderOnlyForEmptyCollection()
    {
        await WithExportFileAsync(async filePath =>
        {
            await new XlsxHistoryExporter().ExportAsync(
                Array.Empty<PrintHistoryEntry>(),
                filePath);

            using SpreadsheetDocument document = OpenWorkbook(filePath);

            Assert.HasCount(1, GetRows(document));
            Assert.AreEqual("A1:Z1", GetWorksheet(document)
                .GetFirstChild<AutoFilter>()?.Reference?.Value);
        });
    }

    private static PrintHistoryEntry CreateEntry(int startNumber, int quantity)
    {
        int endNumber = startNumber + quantity - 1;
        return new PrintHistoryEntry
        {
            Id = Guid.NewGuid(),
            TimestampUtc = new DateTimeOffset(
                2026,
                8,
                14,
                10,
                startNumber % 60,
                0,
                TimeSpan.Zero),
            ApplicationVersion = "3.0.0-test",
            Snapshot = new PrintHistorySnapshot
            {
                StartNumber = startNumber,
                EndNumber = endNumber,
                FirstAssetId = $"IT-{startNumber:D6}",
                LastAssetId = $"IT-{endNumber:D6}",
                Prefix = "IT-",
                Digits = 6,
                CompanyName = "Zażółć gęślą jaźń S.A.",
                PrinterName = "DYMO LabelWriter 550",
                OffsetXmm = -0.4,
                OffsetYmm = 0.0,
                ProfileId = "builtin.89x41.2up",
                ProfileName = "89 × 41 mm — 2 etykiety",
                WidthMm = 89.0,
                HeightMm = 41.0,
                Columns = 2,
                Rows = 1,
                DrawCutLines = true,
                SmallLabelQuantity = quantity,
                PhysicalLabelQuantity = (int)Math.Ceiling(quantity / 2.0),
                QrEnabled = false
            }
        };
    }

    private static PrintHistoryEntry WithOrganization(
        PrintHistoryEntry entry,
        string organizationId,
        string organizationName)
    {
        return entry with
        {
            Snapshot = entry.Snapshot with
            {
                OrganizationProfileId = organizationId,
                OrganizationProfileName = organizationName
            }
        };
    }

    private static SpreadsheetDocument OpenWorkbook(string filePath)
    {
        return SpreadsheetDocument.Open(filePath, false);
    }

    private static Sheet GetSheet(SpreadsheetDocument document)
    {
        WorkbookPart workbookPart = document.WorkbookPart ??
            throw new InvalidOperationException("Brak części skoroszytu.");
        Workbook workbook = workbookPart.Workbook ??
            throw new InvalidOperationException("Brak definicji skoroszytu.");
        return workbook.GetFirstChild<Sheets>()!
            .Elements<Sheet>()
            .Single();
    }

    private static WorksheetPart GetWorksheetPart(SpreadsheetDocument document)
    {
        Sheet sheet = GetSheet(document);
        string relationshipId = sheet.Id?.Value ??
            throw new InvalidOperationException("Arkusz nie ma relacji do części.");
        WorkbookPart workbookPart = document.WorkbookPart ??
            throw new InvalidOperationException("Brak części skoroszytu.");
        return (WorksheetPart)workbookPart.GetPartById(relationshipId);
    }

    private static Worksheet GetWorksheet(SpreadsheetDocument document)
    {
        return GetWorksheetPart(document).Worksheet ??
            throw new InvalidOperationException("Brak definicji arkusza.");
    }

    private static Row[] GetRows(SpreadsheetDocument document)
    {
        return GetWorksheet(document).GetFirstChild<SheetData>()!
            .Elements<Row>()
            .ToArray();
    }

    private static Cell GetCell(
        SpreadsheetDocument document,
        string cellReference)
    {
        return GetRows(document)
            .SelectMany(row => row.Elements<Cell>())
            .Single(cell => cell.CellReference?.Value == cellReference);
    }

    private static string ReadCellText(
        SpreadsheetDocument document,
        string cellReference)
    {
        return ReadText(GetCell(document, cellReference));
    }

    private static string ReadText(Cell cell)
    {
        return cell.InlineString?.InnerText ?? cell.CellValue?.InnerText ?? string.Empty;
    }

    private static double ReadNumber(Cell cell)
    {
        return double.Parse(
            cell.CellValue!.InnerText,
            NumberStyles.Float,
            CultureInfo.InvariantCulture);
    }

    private static DateTime ReadExcelDate(Cell cell)
    {
        return DateTime.FromOADate(ReadNumber(cell));
    }

    private static void AssertDateEquals(DateTime expected, DateTime actual)
    {
        Assert.IsLessThan(
            TimeSpan.FromMilliseconds(1),
            (expected - actual).Duration());
    }

    private static async Task WithExportFileAsync(Func<string, Task> test)
    {
        string directoryPath = Path.Combine(
            Path.GetTempPath(),
            $"EtykietyIT.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(directoryPath);
        string filePath = Path.Combine(directoryPath, "history.xlsx");

        try
        {
            await test(filePath);
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }
}
