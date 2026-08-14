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
    public string? OrganizationProfileId { get; init; }

    public string? OrganizationProfileName { get; init; }

    public int StartNumber { get; init; }

    public int EndNumber { get; init; }

    public string FirstAssetId { get; init; } = string.Empty;

    public string LastAssetId { get; init; } = string.Empty;

    public string Prefix { get; init; } = string.Empty;

    public int Digits { get; init; }

    public string CompanyName { get; init; } = string.Empty;

    public string PrinterName { get; init; } = string.Empty;

    public double OffsetXmm { get; init; }

    public double OffsetYmm { get; init; }

    public string ProfileId { get; init; } = string.Empty;

    public string ProfileName { get; init; } = string.Empty;

    public double WidthMm { get; init; }

    public double HeightMm { get; init; }

    public int Columns { get; init; }

    public int Rows { get; init; }

    public bool DrawCutLines { get; init; }

    public int SmallLabelQuantity { get; init; }

    public int PhysicalLabelQuantity { get; init; }

    public bool QrEnabled { get; init; }

    public void Validate()
    {
        bool hasOrganizationId = !string.IsNullOrWhiteSpace(
            OrganizationProfileId);
        bool hasOrganizationName = !string.IsNullOrWhiteSpace(
            OrganizationProfileName);
        if (hasOrganizationId != hasOrganizationName)
        {
            throw new InvalidOperationException(
                "Snapshot historii musi zawierać oba pola organizacji albo żadnego.");
        }

        if (hasOrganizationId &&
            !OrganizationProfile.IsValidId(OrganizationProfileId))
        {
            throw new InvalidOperationException(
                "Snapshot historii zawiera nieprawidłowy identyfikator organizacji.");
        }

        if (string.IsNullOrWhiteSpace(PrinterName) ||
            string.IsNullOrWhiteSpace(CompanyName) ||
            string.IsNullOrWhiteSpace(Prefix) ||
            string.IsNullOrWhiteSpace(FirstAssetId) ||
            string.IsNullOrWhiteSpace(LastAssetId) ||
            string.IsNullOrWhiteSpace(ProfileId) ||
            string.IsNullOrWhiteSpace(ProfileName))
        {
            throw new InvalidOperationException("Snapshot wydruku zawiera brakujące dane tekstowe.");
        }

        if (Digits is < AssetIdSettings.MinimumDigits or > AssetIdSettings.MaximumDigits ||
            StartNumber < 0 || EndNumber < StartNumber)
        {
            throw new InvalidOperationException("Snapshot wydruku zawiera nieprawidłowy zakres Asset ID.");
        }

        if (SmallLabelQuantity < 1 || PhysicalLabelQuantity < 1 ||
            Columns < 1 || Rows < 1)
        {
            throw new InvalidOperationException("Snapshot wydruku zawiera nieprawidłową liczbę etykiet.");
        }

        if ((long)EndNumber - StartNumber + 1 != SmallLabelQuantity)
        {
            throw new InvalidOperationException(
                "Liczba małych etykiet nie odpowiada zakresowi Asset ID.");
        }

        if (!double.IsFinite(WidthMm) || !double.IsFinite(HeightMm) ||
            WidthMm <= 0.0 || HeightMm <= 0.0)
        {
            throw new InvalidOperationException("Snapshot wydruku zawiera nieprawidłowe wymiary.");
        }

        if (!double.IsFinite(OffsetXmm) || !double.IsFinite(OffsetYmm))
        {
            throw new InvalidOperationException("Snapshot wydruku zawiera nieprawidłową kalibrację.");
        }
    }
}
