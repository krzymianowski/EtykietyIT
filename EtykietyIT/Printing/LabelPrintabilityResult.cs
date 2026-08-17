namespace EtykietyIT.Printing;

public sealed record LabelPrintabilityResult
{
    public IReadOnlyList<LabelPrintabilityIssue> Issues { get; init; } =
        Array.Empty<LabelPrintabilityIssue>();

    public bool IsPrintable => Issues.All(
        issue => issue.Severity != LabelPrintabilitySeverity.Error);

    public float RuntimePageWidthMm { get; init; }

    public float RuntimePageHeightMm { get; init; }

    public float CellWidthMm { get; init; }

    public float CellHeightMm { get; init; }

    public float ContentAreaWidthMm { get; init; }

    public float ContentAreaHeightMm { get; init; }

    public float WhiteAreaWidthMm { get; init; }

    public float WhiteAreaHeightMm { get; init; }

    public float CompanyBarHeightMm { get; init; }

    public float TitleFontSizePt { get; init; }

    public float AssetIdFontSizePt { get; init; }

    public float CompanyFontSizePt { get; init; }

    public float PrinterDpiX { get; init; }

    public int? QrSize { get; init; }

    public int? QrTotalModules { get; init; }

    public int? QrDotsPerModule { get; init; }

    public float? QrFootprintMm { get; init; }

    internal LabelPrintabilityGeometrySnapshot? DiagnosticGeometry { get; init; }
}

internal readonly record struct LabelPrintabilityGeometrySnapshot(
    RectangleF LayoutRect,
    RectangleF QrRect,
    RectangleF TitleRect,
    RectangleF AssetIdRect,
    RectangleF CompanyBarRect,
    RectangleF TitleTextBounds,
    RectangleF AssetIdTextBounds,
    RectangleF CompanyTextBounds,
    float? QrActualFootprintMm,
    int? QrDotsPerModule);
