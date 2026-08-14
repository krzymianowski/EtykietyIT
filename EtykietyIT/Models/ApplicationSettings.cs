namespace EtykietyIT.Models;

public sealed record ApplicationSettings
{
    public const int CurrentSchemaVersion = 2;

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    public string ActiveOrganizationProfileId { get; init; } = string.Empty;

    public void Validate()
    {
        if (SchemaVersion != CurrentSchemaVersion)
        {
            throw new InvalidOperationException(
                $"Nieobsługiwana wersja ustawień: {SchemaVersion}.");
        }

        if (!OrganizationProfile.IsValidId(ActiveOrganizationProfileId))
        {
            throw new InvalidOperationException(
                "Identyfikator aktywnej organizacji musi mieć format organization.<guid>.");
        }
    }
}
