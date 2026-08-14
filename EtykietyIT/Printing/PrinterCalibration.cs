namespace EtykietyIT.Printing;

public sealed record PrinterCalibration(
    double OffsetXmm = 0.0,
    double OffsetYmm = 0.0);
