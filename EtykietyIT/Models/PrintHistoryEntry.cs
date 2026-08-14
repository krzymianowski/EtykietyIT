namespace EtykietyIT.Models;

public sealed record PrintHistoryEntry
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    public Guid Id { get; init; }

    public DateTimeOffset TimestampUtc { get; init; }

    public string ApplicationVersion { get; init; } = string.Empty;

    public PrintHistorySnapshot Snapshot { get; init; } = new();

    public void Validate()
    {
        if (SchemaVersion != CurrentSchemaVersion)
        {
            throw new InvalidOperationException(
                $"Nieobsługiwana wersja wpisu historii: {SchemaVersion}.");
        }

        if (Id == Guid.Empty)
        {
            throw new InvalidOperationException("Identyfikator wpisu historii jest wymagany.");
        }

        if (TimestampUtc == default || TimestampUtc.Offset != TimeSpan.Zero)
        {
            throw new InvalidOperationException("Czas wpisu historii musi być zapisany w UTC.");
        }

        if (string.IsNullOrWhiteSpace(ApplicationVersion))
        {
            throw new InvalidOperationException("Wersja aplikacji jest wymagana.");
        }

        if (Snapshot is null)
        {
            throw new InvalidOperationException("Snapshot wydruku jest wymagany.");
        }

        Snapshot.Validate();
    }
}

public sealed record PrintHistorySnapshot
{
    public string PrinterName { get; init; } = string.Empty;

    public string CompanyName { get; init; } = string.Empty;

    public string AssetIdPrefix { get; init; } = string.Empty;

    public int AssetIdDigits { get; init; }

    public int StartNumber { get; init; }

    public int EndNumber { get; init; }

    public string FirstAssetId { get; init; } = string.Empty;

    public string LastAssetId { get; init; } = string.Empty;

    public int Quantity { get; init; }

    public int PhysicalLabels { get; init; }

    public string ProfileId { get; init; } = string.Empty;

    public string ProfileName { get; init; } = string.Empty;

    public double WidthMm { get; init; }

    public double HeightMm { get; init; }

    public int Columns { get; init; }

    public int Rows { get; init; }

    public bool DrawCutLines { get; init; }

    public double CalibrationOffsetXmm { get; init; }

    public double CalibrationOffsetYmm { get; init; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PrinterName) ||
            string.IsNullOrWhiteSpace(CompanyName) ||
            string.IsNullOrWhiteSpace(AssetIdPrefix) ||
            string.IsNullOrWhiteSpace(FirstAssetId) ||
            string.IsNullOrWhiteSpace(LastAssetId) ||
            string.IsNullOrWhiteSpace(ProfileId) ||
            string.IsNullOrWhiteSpace(ProfileName))
        {
            throw new InvalidOperationException("Snapshot wydruku zawiera brakujące dane tekstowe.");
        }

        if (AssetIdDigits < 1 || StartNumber < 0 || EndNumber < StartNumber)
        {
            throw new InvalidOperationException("Snapshot wydruku zawiera nieprawidłowy zakres Asset ID.");
        }

        if (Quantity < 1 || PhysicalLabels < 1 || Columns < 1 || Rows < 1)
        {
            throw new InvalidOperationException("Snapshot wydruku zawiera nieprawidłową liczbę etykiet.");
        }

        if (!double.IsFinite(WidthMm) || !double.IsFinite(HeightMm) ||
            WidthMm <= 0.0 || HeightMm <= 0.0)
        {
            throw new InvalidOperationException("Snapshot wydruku zawiera nieprawidłowe wymiary.");
        }

        if (!double.IsFinite(CalibrationOffsetXmm) ||
            !double.IsFinite(CalibrationOffsetYmm))
        {
            throw new InvalidOperationException("Snapshot wydruku zawiera nieprawidłową kalibrację.");
        }
    }
}
