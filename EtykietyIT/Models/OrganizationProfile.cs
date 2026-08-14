namespace EtykietyIT.Models;

public sealed record OrganizationProfile
{
    public const int CurrentSchemaVersion = 1;
    public const string IdPrefix = "organization.";

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = "Domyślna organizacja";

    public string CompanyName { get; init; } = "Moja firma";

    public AssetIdSettings AssetId { get; init; } = new();

    public int NextAssetNumber { get; init; } = 1;

    public string DefaultLabelProfileId { get; init; } =
        "builtin.89x41.2up";

    public string? DefaultPrinterName { get; init; }

    public void Validate()
    {
        if (SchemaVersion != CurrentSchemaVersion)
        {
            throw new InvalidOperationException(
                $"Nieobsługiwana wersja profilu organizacji: {SchemaVersion}.");
        }

        if (!IsValidId(Id))
        {
            throw new InvalidOperationException(
                "Identyfikator organizacji musi mieć format organization.<guid>.");
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException(
                "Nazwa profilu organizacji nie może być pusta.");
        }

        if (string.IsNullOrWhiteSpace(CompanyName))
        {
            throw new InvalidOperationException(
                "Nazwa firmy na etykiecie nie może być pusta.");
        }

        if (AssetId is null)
        {
            throw new InvalidOperationException(
                "Brak ustawień formatu Asset ID organizacji.");
        }

        AssetId.Validate();

        if (NextAssetNumber < 0)
        {
            throw new InvalidOperationException(
                "Następny numer Asset ID nie może być ujemny.");
        }

        if (string.IsNullOrWhiteSpace(DefaultLabelProfileId))
        {
            throw new InvalidOperationException(
                "Domyślny profil etykiety jest wymagany.");
        }
    }

    public static bool IsValidId(string? id)
    {
        return id is not null &&
            id.StartsWith(IdPrefix, StringComparison.Ordinal) &&
            Guid.TryParseExact(id[IdPrefix.Length..], "D", out _);
    }
}
