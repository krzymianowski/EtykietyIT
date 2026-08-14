using System.Globalization;
using System.Text;
using EtykietyIT.Models;

namespace EtykietyIT.Export;

public sealed class CsvHistoryExporter : IHistoryExporter
{
    private static readonly UTF8Encoding Utf8WithBom = new(
        encoderShouldEmitUTF8Identifier: true);
    private static readonly CultureInfo PolishCulture =
        CultureInfo.GetCultureInfo("pl-PL");

    private static readonly string[] Headers =
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

    public async Task ExportAsync(
        IEnumerable<PrintHistoryEntry> entries,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        PrintHistoryEntry[] entriesToExport = entries.ToArray();
        foreach (PrintHistoryEntry entry in entriesToExport)
        {
            entry.Validate();
        }

        await using var stream = new FileStream(
            Path.GetFullPath(filePath),
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            FileOptions.Asynchronous);
        await using var writer = new StreamWriter(
            stream,
            Utf8WithBom,
            bufferSize: 4096,
            leaveOpen: false)
        {
            NewLine = "\r\n"
        };

        await writer.WriteLineAsync(
            CreateCsvRow(Headers).AsMemory(),
            cancellationToken);

        foreach (PrintHistoryEntry entry in entriesToExport)
        {
            await writer.WriteLineAsync(
                CreateCsvRow(CreateValues(entry)).AsMemory(),
                cancellationToken);
        }
    }

    private static string[] CreateValues(PrintHistoryEntry entry)
    {
        PrintHistorySnapshot snapshot = entry.Snapshot;
        return
        [
            entry.Id.ToString("D"),
            entry.TimestampUtc.ToLocalTime().ToString(
                "yyyy-MM-dd HH:mm:ss zzz",
                CultureInfo.InvariantCulture),
            entry.TimestampUtc.ToString("O", CultureInfo.InvariantCulture),
            entry.ApplicationVersion,
            snapshot.FirstAssetId,
            snapshot.LastAssetId,
            snapshot.StartNumber.ToString(CultureInfo.InvariantCulture),
            snapshot.EndNumber.ToString(CultureInfo.InvariantCulture),
            snapshot.SmallLabelQuantity.ToString(CultureInfo.InvariantCulture),
            snapshot.PhysicalLabelQuantity.ToString(CultureInfo.InvariantCulture),
            snapshot.CompanyName,
            snapshot.Prefix,
            snapshot.Digits.ToString(CultureInfo.InvariantCulture),
            snapshot.PrinterName,
            FormatNumber(snapshot.OffsetXmm),
            FormatNumber(snapshot.OffsetYmm),
            snapshot.ProfileId,
            snapshot.ProfileName,
            FormatNumber(snapshot.WidthMm),
            FormatNumber(snapshot.HeightMm),
            snapshot.Columns.ToString(CultureInfo.InvariantCulture),
            snapshot.Rows.ToString(CultureInfo.InvariantCulture),
            FormatBoolean(snapshot.DrawCutLines),
            FormatBoolean(snapshot.QrEnabled)
        ];
    }

    private static string CreateCsvRow(IEnumerable<string> values)
    {
        return string.Join(';', values.Select(Escape));
    }

    private static string Escape(string value)
    {
        string normalizedValue = value
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Replace("\n", "\r\n", StringComparison.Ordinal);

        bool requiresQuotes = normalizedValue.Contains(';') ||
            normalizedValue.Contains('"') ||
            normalizedValue.Contains('\r') ||
            normalizedValue.Contains('\n');

        return requiresQuotes
            ? $"\"{normalizedValue.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : normalizedValue;
    }

    private static string FormatNumber(double value)
    {
        return value.ToString("0.0###", PolishCulture);
    }

    private static string FormatBoolean(bool value)
    {
        return value ? "Tak" : "Nie";
    }
}
