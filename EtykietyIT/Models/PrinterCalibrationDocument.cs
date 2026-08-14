namespace EtykietyIT.Models;

public sealed record PrinterCalibrationDocument
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    public List<PrinterCalibrationEntry> Printers { get; init; } = [];

    public void Validate()
    {
        if (SchemaVersion != CurrentSchemaVersion)
        {
            throw new InvalidOperationException(
                $"Nieobsługiwana wersja kalibracji drukarek: {SchemaVersion}.");
        }

        if (Printers is null)
        {
            throw new InvalidOperationException("Brak kolekcji kalibracji drukarek.");
        }

        var printerNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (PrinterCalibrationEntry entry in Printers)
        {
            if (entry is null)
            {
                throw new InvalidOperationException("Pusty wpis kalibracji drukarki.");
            }

            entry.Validate();

            if (!printerNames.Add(entry.PrinterName))
            {
                throw new InvalidOperationException(
                    $"Powielona kalibracja drukarki: {entry.PrinterName}.");
            }
        }
    }
}
