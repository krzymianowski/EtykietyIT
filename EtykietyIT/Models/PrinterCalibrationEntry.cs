namespace EtykietyIT.Models;

public sealed record PrinterCalibrationEntry
{
    public string PrinterName { get; init; } = string.Empty;

    public double OffsetXmm { get; init; }

    public double OffsetYmm { get; init; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PrinterName))
        {
            throw new InvalidOperationException("Nazwa drukarki jest wymagana.");
        }

        if (!double.IsFinite(OffsetXmm) || !double.IsFinite(OffsetYmm))
        {
            throw new InvalidOperationException("Kalibracja drukarki musi być liczbą skończoną.");
        }

        if (OffsetXmm is < -10.0 or > 10.0 || OffsetYmm is < -10.0 or > 10.0)
        {
            throw new InvalidOperationException(
                "Kalibracja drukarki musi mieścić się w zakresie od -10,0 do 10,0 mm.");
        }
    }
}
