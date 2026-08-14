using System.Text;
using EtykietyIT.Models;
using EtykietyIT.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Services;

[TestClass]
public sealed class PrintHistoryServiceTests
{
    [TestMethod]
    public void ReadAll_ReturnsEmptyHistory_WhenFileDoesNotExist()
    {
        WithHistoryFile((service, _) =>
        {
            PrintHistoryReadResult result = service.ReadAll();

            Assert.IsEmpty(result.Entries);
            Assert.AreEqual(0, result.SkippedRecordCount);
        });
    }

    [TestMethod]
    public void Append_WritesOneEntryAsOneJsonLine()
    {
        WithHistoryFile((service, filePath) =>
        {
            PrintHistoryEntry entry = CreateEntry(100, 2);

            service.Append(entry);

            Assert.HasCount(1, File.ReadAllLines(filePath));
            Assert.AreEqual(entry, service.ReadAll().Entries.Single());
        });
    }

    [TestMethod]
    public async Task AppendAsync_AppendsMultipleEntries()
    {
        await WithHistoryFileAsync(async (service, _) =>
        {
            await service.AppendAsync(CreateEntry(100, 2));
            await service.AppendAsync(CreateEntry(102, 3));
            await service.AppendAsync(CreateEntry(105, 1));

            PrintHistoryReadResult result = await service.ReadAllAsync();

            Assert.HasCount(3, result.Entries);
            Assert.AreEqual(0, result.SkippedRecordCount);
        });
    }

    [TestMethod]
    public async Task ReadAllAsync_RoundTripsCompleteSnapshot()
    {
        await WithHistoryFileAsync(async (service, _) =>
        {
            PrintHistoryEntry expected = CreateEntry(
                123,
                5,
                companyName: "Dolnośląskie Młyny S.A.");

            await service.AppendAsync(expected);
            PrintHistoryEntry actual = (await service.ReadAllAsync())
                .Entries
                .Single();

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Snapshot, actual.Snapshot);
        });
    }

    [TestMethod]
    public async Task AppendAsync_PreservesBytesOfEarlierEntries()
    {
        await WithHistoryFileAsync(async (service, filePath) =>
        {
            await service.AppendAsync(CreateEntry(1, 1));
            byte[] firstFileContents = await File.ReadAllBytesAsync(filePath);

            await service.AppendAsync(CreateEntry(2, 1));
            byte[] appendedFileContents = await File.ReadAllBytesAsync(filePath);

            Assert.IsTrue(appendedFileContents.AsSpan().StartsWith(firstFileContents));
            Assert.IsGreaterThan(
                firstFileContents.Length,
                appendedFileContents.Length);
        });
    }

    [TestMethod]
    public async Task ReadAllAsync_SkipsOneCorruptedLineAndReturnsValidEntries()
    {
        await WithHistoryFileAsync(async (service, filePath) =>
        {
            PrintHistoryEntry first = CreateEntry(10, 2);
            PrintHistoryEntry second = CreateEntry(12, 2);
            await service.AppendAsync(first);
            await File.AppendAllTextAsync(
                filePath,
                "{ uszkodzony rekord }\n",
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            await service.AppendAsync(second);

            PrintHistoryReadResult result = await service.ReadAllAsync();

            Assert.HasCount(2, result.Entries);
            Assert.AreEqual(first, result.Entries[0]);
            Assert.AreEqual(second, result.Entries[1]);
            Assert.AreEqual(1, result.SkippedRecordCount);
        });
    }

    [TestMethod]
    public async Task AppendAsync_WritesUnescapedUtf8Text()
    {
        await WithHistoryFileAsync(async (service, filePath) =>
        {
            const string companyName = "Zażółć gęślą jaźń S.A.";
            await service.AppendAsync(CreateEntry(1, 1, companyName));

            string fileContents = await File.ReadAllTextAsync(
                filePath,
                new UTF8Encoding(
                    encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: true));

            StringAssert.Contains(fileContents, companyName);
            Assert.AreEqual(
                companyName,
                (await service.ReadAllAsync()).Entries.Single().Snapshot.CompanyName);
        });
    }

    [TestMethod]
    public async Task ReadAllAsync_PreservesFileOrder()
    {
        await WithHistoryFileAsync(async (service, _) =>
        {
            PrintHistoryEntry first = CreateEntry(30, 1);
            PrintHistoryEntry second = CreateEntry(10, 1);
            PrintHistoryEntry third = CreateEntry(20, 1);
            await service.AppendAsync(first);
            await service.AppendAsync(second);
            await service.AppendAsync(third);

            IReadOnlyList<PrintHistoryEntry> entries =
                (await service.ReadAllAsync()).Entries;

            CollectionAssert.AreEqual(
                new[] { first.Id, second.Id, third.Id },
                entries.Select(entry => entry.Id).ToArray());
        });
    }

    [TestMethod]
    public void Matches_FindsAssetIdInsidePrintedRange()
    {
        PrintHistoryEntry entry = CreateEntry(120, 10);

        Assert.IsTrue(PrintHistorySearch.Matches(entry, "IT-000125"));
        Assert.IsFalse(PrintHistorySearch.Matches(entry, "IT-000130"));
    }

    [TestMethod]
    public async Task AppendAsync_RoundTripsOrganizationSnapshotForNewEntry()
    {
        await WithHistoryFileAsync(async (service, _) =>
        {
            PrintHistoryEntry original = CreateEntry(1, 1);
            PrintHistoryEntry entry = original with
            {
                Snapshot = original.Snapshot with
                {
                    OrganizationProfileId =
                        "organization.3a271df1-99e5-4fa5-bf37-27f323370c42",
                    OrganizationProfileName = "Oddział Wrocław"
                }
            };

            await service.AppendAsync(entry);
            PrintHistorySnapshot snapshot = (await service.ReadAllAsync())
                .Entries
                .Single()
                .Snapshot;

            Assert.AreEqual(
                entry.Snapshot.OrganizationProfileId,
                snapshot.OrganizationProfileId);
            Assert.AreEqual("Oddział Wrocław", snapshot.OrganizationProfileName);
        });
    }

    [TestMethod]
    public async Task ReadAllAsync_ReadsLegacyLineWithoutOrganizationFields()
    {
        await WithHistoryFileAsync(async (service, filePath) =>
        {
            const string legacyLine = """
                {"schemaVersion":1,"id":"9d025a9d-d26e-4fc9-a47d-af83be0bdf52","timestampUtc":"2026-08-14T10:00:00+00:00","applicationVersion":"3.0.0","snapshot":{"startNumber":1,"endNumber":2,"firstAssetId":"IT-000001","lastAssetId":"IT-000002","prefix":"IT-","digits":6,"companyName":"Stara firma","printerName":"DYMO LabelWriter 450","offsetXmm":0.0,"offsetYmm":0.0,"profileId":"builtin.89x41.2up","profileName":"89 × 41 mm — 2 etykiety","widthMm":89.0,"heightMm":41.0,"columns":2,"rows":1,"drawCutLines":true,"smallLabelQuantity":2,"physicalLabelQuantity":1,"qrEnabled":false}}
                """;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            await File.WriteAllTextAsync(filePath, $"{legacyLine}\n");

            PrintHistoryReadResult result = await service.ReadAllAsync();

            Assert.HasCount(1, result.Entries);
            Assert.AreEqual(0, result.SkippedRecordCount);
            Assert.IsNull(result.Entries[0].Snapshot.OrganizationProfileId);
            Assert.IsNull(result.Entries[0].Snapshot.OrganizationProfileName);
        });
    }

    private static PrintHistoryEntry CreateEntry(
        int startNumber,
        int quantity,
        string companyName = "Przykładowa Firma S.A.")
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
                CompanyName = companyName,
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

    private static void WithHistoryFile(
        Action<PrintHistoryService, string> test)
    {
        string directoryPath = CreateTemporaryDirectory();
        string filePath = Path.Combine(directoryPath, "history", "print-history.jsonl");

        try
        {
            test(new PrintHistoryService(filePath), filePath);
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }

    private static async Task WithHistoryFileAsync(
        Func<PrintHistoryService, string, Task> test)
    {
        string directoryPath = CreateTemporaryDirectory();
        string filePath = Path.Combine(directoryPath, "history", "print-history.jsonl");

        try
        {
            await test(new PrintHistoryService(filePath), filePath);
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }

    private static string CreateTemporaryDirectory()
    {
        string directoryPath = Path.Combine(
            Path.GetTempPath(),
            $"EtykietyIT.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(directoryPath);
        return directoryPath;
    }
}
