namespace EtykietyIT.Printing;

public sealed record LabelPrintOptions(
    string PrinterName,
    int StartNumber,
    int Quantity,
    double WidthMm,
    double HeightMm,
    int Columns,
    int Rows,
    bool DrawCutLines,
    LabelContentOptions Content,
    PrinterCalibration? Calibration = null,
    LabelRenderMode RenderMode = LabelRenderMode.Print);
