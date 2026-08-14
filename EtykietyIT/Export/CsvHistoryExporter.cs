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

    public async Task ExportAsync(
        IEnumerable<PrintHistoryEntry> entries,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        PrintHistoryEntry[] entriesToExport =
            HistoryExportSchema.MaterializeAndValidate(entries);

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
            CreateCsvRow(HistoryExportSchema.Columns.Select(
                column => column.Header)).AsMemory(),
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
        return HistoryExportSchema.Columns
            .Select(column => FormatValue(column, entry))
            .ToArray();
    }

    private static string FormatValue(
        HistoryExportColumn column,
        PrintHistoryEntry entry)
    {
        object? value = column.GetValue(entry);
        return column.ValueKind switch
        {
            HistoryExportValueKind.Text => value as string ?? string.Empty,
            HistoryExportValueKind.LocalDateTime =>
                ((DateTimeOffset)value!).ToLocalTime().ToString(
                    "yyyy-MM-dd HH:mm:ss zzz",
                    CultureInfo.InvariantCulture),
            HistoryExportValueKind.UtcDateTime =>
                ((DateTimeOffset)value!).ToString(
                    "O",
                    CultureInfo.InvariantCulture),
            HistoryExportValueKind.Integer => Convert.ToInt32(
                value,
                CultureInfo.InvariantCulture).ToString(
                    CultureInfo.InvariantCulture),
            HistoryExportValueKind.Number => FormatNumber(Convert.ToDouble(
                value,
                CultureInfo.InvariantCulture)),
            HistoryExportValueKind.Boolean => FormatBoolean((bool)value!),
            _ => throw new InvalidOperationException(
                $"Nieobsługiwany typ wartości eksportu: {column.ValueKind}.")
        };
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
