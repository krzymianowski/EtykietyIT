namespace EtykietyIT.Models;

public sealed record LabelProfile
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public double WidthMm { get; init; } = 89.0;

    public double HeightMm { get; init; } = 41.0;

    public int Columns { get; init; } = 2;

    public int Rows { get; init; } = 1;

    public bool DrawCutLines { get; init; } = true;

    public void Validate()
    {
        if (SchemaVersion != CurrentSchemaVersion)
        {
            throw new InvalidOperationException(
                $"Nieobsługiwana wersja profilu: {SchemaVersion}.");
        }

        if (string.IsNullOrWhiteSpace(Id))
        {
            throw new InvalidOperationException("Identyfikator profilu jest wymagany.");
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Nazwa profilu jest wymagana.");
        }

        if (!double.IsFinite(WidthMm) || !double.IsFinite(HeightMm))
        {
            throw new InvalidOperationException("Wymiary profilu muszą być liczbami skończonymi.");
        }

        if (WidthMm < 20.0 || HeightMm < 15.0)
        {
            throw new InvalidOperationException("Rozmiar etykiety jest zbyt mały.");
        }

        if (Columns < 1 || Rows < 1)
        {
            throw new InvalidOperationException(
                "Układ etykiet musi mieć co najmniej 1 kolumnę i 1 wiersz.");
        }
    }
}
