using System.Text;
using EtykietyIT.Export;
using EtykietyIT.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Export;

[TestClass]
public sealed class CsvHistoryExporterTests
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
    public async Task ExportAsync_WritesRequiredPolishHeaders()
    {
        await WithExportFileAsync(async filePath =>
        {
            await new CsvHistoryExporter().ExportAsync(
                Array.Empty<PrintHistoryEntry>(),
                filePath);

            string headerLine = ReadLines(filePath).Single();

            CollectionAssert.AreEqual(ExpectedHeaders, ParseRow(headerLine));
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesSingleEntry()
    {
        await WithExportFileAsync(async filePath =>
        {
            PrintHistoryEntry entry = CreateEntry(123, 4);

            await new CsvHistoryExporter().ExportAsync(new[] { entry }, filePath);
            string[][] rows = ReadLines(filePath).Select(ParseRow).ToArray();

            Assert.HasCount(2, rows);
            Assert.HasCount(26, rows[1]);
            Assert.AreEqual(entry.Id.ToString("D"), rows[1][0]);
            Assert.AreEqual("IT-000123", rows[1][4]);
            Assert.AreEqual("IT-000126", rows[1][5]);
            Assert.AreEqual("4", rows[1][8]);
            Assert.AreEqual("2", rows[1][9]);
            Assert.AreEqual(string.Empty, rows[1][10]);
            Assert.AreEqual("—", rows[1][11]);
            Assert.AreEqual("Tak", rows[1][24]);
            Assert.AreEqual("Nie", rows[1][25]);
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesQrEnabledAsTak()
    {
        await WithExportFileAsync(async filePath =>
        {
            PrintHistoryEntry original = CreateEntry(123, 1);
            PrintHistoryEntry entry = original with
            {
                Snapshot = original.Snapshot with { QrEnabled = true }
            };

            await new CsvHistoryExporter().ExportAsync(new[] { entry }, filePath);
            string[] values = ParseRow(ReadLines(filePath)[1]);

            Assert.AreEqual("Tak", values[25]);
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesOrganizationInformation()
    {
        await WithExportFileAsync(async filePath =>
        {
            const string organizationId =
                "organization.3a271df1-99e5-4fa5-bf37-27f323370c42";
            const string organizationName = "Oddział Łódź";
            PrintHistoryEntry entry = WithOrganization(
                CreateEntry(123, 1),
                organizationId,
                organizationName);

            await new CsvHistoryExporter().ExportAsync(new[] { entry }, filePath);
            string[] values = ParseRow(ReadLines(filePath)[1]);

            Assert.AreEqual(organizationId, values[10]);
            Assert.AreEqual(organizationName, values[11]);
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesLegacyEntryWithoutOrganization()
    {
        await WithExportFileAsync(async filePath =>
        {
            PrintHistoryEntry legacyEntry = CreateEntry(123, 1);

            await new CsvHistoryExporter().ExportAsync(
                new[] { legacyEntry },
                filePath);
            string[] values = ParseRow(ReadLines(filePath)[1]);

            Assert.AreEqual(string.Empty, values[10]);
            Assert.AreEqual("—", values[11]);
        });
    }

    [TestMethod]
    public async Task ExportAsync_EscapesSemicolonsAndQuotesInOrganizationName()
    {
        await WithExportFileAsync(async filePath =>
        {
            const string organizationName = "Oddział; \"Północ — Łódź\"";
            PrintHistoryEntry entry = WithOrganization(
                CreateEntry(123, 1),
                "organization.a08dc2ef-35b9-4812-99da-ec32d662c8b1",
                organizationName);

            await new CsvHistoryExporter().ExportAsync(new[] { entry }, filePath);
            string dataLine = ReadLines(filePath)[1];
            string[] values = ParseRow(dataLine);

            StringAssert.Contains(
                dataLine,
                "\"Oddział; \"\"Północ — Łódź\"\"\"");
            Assert.AreEqual(organizationName, values[11]);
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesMultipleEntriesInInputOrder()
    {
        await WithExportFileAsync(async filePath =>
        {
            PrintHistoryEntry first = CreateEntry(10, 1);
            PrintHistoryEntry second = CreateEntry(20, 2);
            PrintHistoryEntry third = CreateEntry(30, 3);

            await new CsvHistoryExporter().ExportAsync(
                new[] { first, second, third },
                filePath);
            string[][] rows = ReadLines(filePath).Select(ParseRow).ToArray();

            Assert.HasCount(4, rows);
            CollectionAssert.AreEqual(
                new[]
                {
                    first.Id.ToString("D"),
                    second.Id.ToString("D"),
                    third.Id.ToString("D")
                },
                rows.Skip(1).Select(row => row[0]).ToArray());
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesUtf8BomAndPolishCharacters()
    {
        await WithExportFileAsync(async filePath =>
        {
            const string companyName = "Zażółć gęślą jaźń S.A.";
            PrintHistoryEntry entry = CreateEntry(1, 1) with
            {
                Snapshot = CreateEntry(1, 1).Snapshot with
                {
                    CompanyName = companyName
                }
            };

            await new CsvHistoryExporter().ExportAsync(new[] { entry }, filePath);
            byte[] bytes = await File.ReadAllBytesAsync(filePath);
            string text = await File.ReadAllTextAsync(filePath);

            CollectionAssert.AreEqual(
                new byte[] { 0xEF, 0xBB, 0xBF },
                bytes.Take(3).ToArray());
            StringAssert.Contains(text, companyName);
        });
    }

    [TestMethod]
    public async Task ExportAsync_EscapesSemicolonsAndQuotes()
    {
        await WithExportFileAsync(async filePath =>
        {
            PrintHistoryEntry original = CreateEntry(1, 1);
            PrintHistoryEntry entry = original with
            {
                Snapshot = original.Snapshot with
                {
                    CompanyName = "Firma; \"Polska\" S.A.",
                    ProfileName = "Profil; \"Specjalny\""
                }
            };

            await new CsvHistoryExporter().ExportAsync(new[] { entry }, filePath);
            string dataLine = ReadLines(filePath)[1];
            string[] values = ParseRow(dataLine);

            StringAssert.Contains(dataLine, "\"Firma; \"\"Polska\"\" S.A.\"");
            StringAssert.Contains(dataLine, "\"Profil; \"\"Specjalny\"\"\"");
            Assert.AreEqual(entry.Snapshot.CompanyName, values[12]);
            Assert.AreEqual(entry.Snapshot.ProfileName, values[19]);
        });
    }

    [TestMethod]
    public async Task ExportAsync_UsesPolishDecimalSeparatorForCalibration()
    {
        await WithExportFileAsync(async filePath =>
        {
            PrintHistoryEntry original = CreateEntry(1, 1);
            PrintHistoryEntry entry = original with
            {
                Snapshot = original.Snapshot with
                {
                    OffsetXmm = -0.4,
                    OffsetYmm = 1.25
                }
            };

            await new CsvHistoryExporter().ExportAsync(new[] { entry }, filePath);
            string[] values = ParseRow(ReadLines(filePath)[1]);

            Assert.AreEqual("-0,4", values[16]);
            Assert.AreEqual("1,25", values[17]);
        });
    }

    [TestMethod]
    public async Task ExportAsync_UsesOnlyCrLfLineEndings()
    {
        await WithExportFileAsync(async filePath =>
        {
            await new CsvHistoryExporter().ExportAsync(
                new[] { CreateEntry(1, 1), CreateEntry(2, 1) },
                filePath);
            string text = await File.ReadAllTextAsync(filePath);
            char[] textWithoutCrLf = text
                .Replace("\r\n", string.Empty, StringComparison.Ordinal)
                .ToCharArray();

            Assert.EndsWith("\r\n", text);
            CollectionAssert.DoesNotContain(textWithoutCrLf, '\n');
            CollectionAssert.DoesNotContain(textWithoutCrLf, '\r');
            Assert.HasCount(3, ReadLines(filePath));
        });
    }

    [TestMethod]
    public async Task ExportAsync_WritesHeaderOnlyForEmptyCollection()
    {
        await WithExportFileAsync(async filePath =>
        {
            await new CsvHistoryExporter().ExportAsync(
                Array.Empty<PrintHistoryEntry>(),
                filePath);

            Assert.IsTrue(File.Exists(filePath));
            Assert.HasCount(1, ReadLines(filePath));
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
                CompanyName = "Dolnośląskie Młyny S.A.",
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

    private static string[] ReadLines(string filePath)
    {
        return File.ReadAllText(filePath)
            .Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
    }

    private static string[] ParseRow(string row)
    {
        var values = new List<string>();
        var value = new StringBuilder();
        bool insideQuotes = false;

        for (int index = 0; index < row.Length; index++)
        {
            char character = row[index];
            if (character == '"')
            {
                if (insideQuotes && index + 1 < row.Length && row[index + 1] == '"')
                {
                    value.Append('"');
                    index++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (character == ';' && !insideQuotes)
            {
                values.Add(value.ToString());
                value.Clear();
            }
            else
            {
                value.Append(character);
            }
        }

        values.Add(value.ToString());
        return values.ToArray();
    }

    private static async Task WithExportFileAsync(Func<string, Task> test)
    {
        string directoryPath = Path.Combine(
            Path.GetTempPath(),
            $"EtykietyIT.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(directoryPath);
        string filePath = Path.Combine(directoryPath, "history.csv");

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
