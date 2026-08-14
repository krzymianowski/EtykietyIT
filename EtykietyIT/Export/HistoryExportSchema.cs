using EtykietyIT.Models;

namespace EtykietyIT.Export;

internal enum HistoryExportValueKind
{
    Text,
    LocalDateTime,
    UtcDateTime,
    Integer,
    Number,
    Boolean
}

internal sealed record HistoryExportColumn(
    string Header,
    HistoryExportValueKind ValueKind,
    double Width,
    bool WrapText,
    Func<PrintHistoryEntry, object?> GetValue);

internal static class HistoryExportSchema
{
    public static IReadOnlyList<HistoryExportColumn> Columns { get; } =
    [
        new("ID wpisu", HistoryExportValueKind.Text, 38, false,
            entry => entry.Id.ToString("D")),
        new("Data lokalna", HistoryExportValueKind.LocalDateTime, 21, false,
            entry => entry.TimestampUtc),
        new("Data UTC", HistoryExportValueKind.UtcDateTime, 21, false,
            entry => entry.TimestampUtc),
        new("Wersja aplikacji", HistoryExportValueKind.Text, 20, false,
            entry => entry.ApplicationVersion),
        new("Pierwszy Asset ID", HistoryExportValueKind.Text, 18, false,
            entry => entry.Snapshot.FirstAssetId),
        new("Ostatni Asset ID", HistoryExportValueKind.Text, 18, false,
            entry => entry.Snapshot.LastAssetId),
        new("Numer początkowy", HistoryExportValueKind.Integer, 16, false,
            entry => entry.Snapshot.StartNumber),
        new("Numer końcowy", HistoryExportValueKind.Integer, 16, false,
            entry => entry.Snapshot.EndNumber),
        new("Liczba małych etykiet", HistoryExportValueKind.Integer, 18, false,
            entry => entry.Snapshot.SmallLabelQuantity),
        new("Liczba fizycznych etykiet", HistoryExportValueKind.Integer, 20, false,
            entry => entry.Snapshot.PhysicalLabelQuantity),
        new("ID organizacji", HistoryExportValueKind.Text, 38, false,
            entry => entry.Snapshot.OrganizationProfileId ?? string.Empty),
        new("Organizacja", HistoryExportValueKind.Text, 26, true,
            entry => FormatOptionalOrganization(entry.Snapshot.OrganizationProfileName)),
        new("Firma", HistoryExportValueKind.Text, 30, true,
            entry => entry.Snapshot.CompanyName),
        new("Prefiks", HistoryExportValueKind.Text, 12, false,
            entry => entry.Snapshot.Prefix),
        new("Liczba cyfr", HistoryExportValueKind.Integer, 12, false,
            entry => entry.Snapshot.Digits),
        new("Drukarka", HistoryExportValueKind.Text, 30, true,
            entry => entry.Snapshot.PrinterName),
        new("Korekta X [mm]", HistoryExportValueKind.Number, 14, false,
            entry => entry.Snapshot.OffsetXmm),
        new("Korekta Y [mm]", HistoryExportValueKind.Number, 14, false,
            entry => entry.Snapshot.OffsetYmm),
        new("ID profilu", HistoryExportValueKind.Text, 28, false,
            entry => entry.Snapshot.ProfileId),
        new("Nazwa profilu", HistoryExportValueKind.Text, 30, true,
            entry => entry.Snapshot.ProfileName),
        new("Szerokość [mm]", HistoryExportValueKind.Number, 14, false,
            entry => entry.Snapshot.WidthMm),
        new("Wysokość [mm]", HistoryExportValueKind.Number, 14, false,
            entry => entry.Snapshot.HeightMm),
        new("Kolumny", HistoryExportValueKind.Integer, 10, false,
            entry => entry.Snapshot.Columns),
        new("Wiersze", HistoryExportValueKind.Integer, 10, false,
            entry => entry.Snapshot.Rows),
        new("Linie cięcia", HistoryExportValueKind.Boolean, 12, false,
            entry => entry.Snapshot.DrawCutLines),
        new("QR", HistoryExportValueKind.Boolean, 8, false,
            entry => entry.Snapshot.QrEnabled)
    ];

    public static PrintHistoryEntry[] MaterializeAndValidate(
        IEnumerable<PrintHistoryEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        PrintHistoryEntry[] entriesToExport = entries.ToArray();
        foreach (PrintHistoryEntry entry in entriesToExport)
        {
            entry.Validate();
        }

        return entriesToExport;
    }

    private static string FormatOptionalOrganization(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "—" : value;
    }
}
