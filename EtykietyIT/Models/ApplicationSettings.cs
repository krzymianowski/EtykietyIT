namespace EtykietyIT.Models;

public sealed record ApplicationSettings
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    public string CompanyName { get; init; } = "Dolnośląskie Młyny S.A.";

    public AssetIdSettings AssetId { get; init; } = new();

    public string? DefaultPrinterName { get; init; }

    public string DefaultProfileId { get; init; } = "builtin.89x41.2up";

    public int NextAssetNumber { get; init; } = 11;

    public void Validate()
    {
        if (SchemaVersion != CurrentSchemaVersion)
        {
            throw new InvalidOperationException(
                $"Nieobsługiwana wersja ustawień: {SchemaVersion}.");
        }

        if (string.IsNullOrWhiteSpace(CompanyName))
        {
            throw new InvalidOperationException("Nazwa firmy nie może być pusta.");
        }

        if (AssetId is null)
        {
            throw new InvalidOperationException("Brak ustawień formatu Asset ID.");
        }

        AssetId.Validate();

        if (string.IsNullOrWhiteSpace(DefaultProfileId))
        {
            throw new InvalidOperationException("Identyfikator domyślnego profilu jest wymagany.");
        }

        if (NextAssetNumber < 0)
        {
            throw new InvalidOperationException("Następny numer Asset ID nie może być ujemny.");
        }
    }
}
