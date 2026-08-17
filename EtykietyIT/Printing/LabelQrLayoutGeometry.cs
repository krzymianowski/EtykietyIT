namespace EtykietyIT.Printing;

internal readonly record struct LabelQrLayoutGeometry(
    RectangleF QrRect,
    RectangleF TitleRect,
    RectangleF AssetIdRect,
    RectangleF CompanyBarRect,
    float AvailableQrFootprintMm,
    float LayoutX,
    float LayoutY,
    float LayoutWidth,
    float LayoutHeight,
    float WhiteAreaHeight)
{
    internal const float TextGapMm = 1.5f;
    internal const float MinimumTextWidthMm = 10.0f;

    internal static LabelQrLayoutGeometry Calculate(
        float layoutX0,
        float layoutX1,
        float layoutY0,
        float layoutY1)
    {
        float layoutWidth = layoutX1 - layoutX0;
        float layoutHeight = layoutY1 - layoutY0;

        float titleY = layoutY0 + layoutHeight * 0.07f;
        float titleHeight = Math.Max(3.0f, layoutHeight * 0.14f);
        float numberY = layoutY0 + layoutHeight * 0.23f;
        float numberHeight = Math.Max(6.0f, layoutHeight * 0.38f);
        float barHeight = Math.Max(5.0f, layoutHeight * 0.19f);
        float barY =
            layoutY1 - barHeight - Math.Max(0.8f, layoutHeight * 0.04f);

        float whiteAreaHeight = barY - layoutY0;
        float qrAvailableWidth =
            layoutWidth - TextGapMm - MinimumTextWidthMm;
        float qrZoneSize = Math.Min(
            LabelQrRenderer.PreferredQrFootprintMm,
            Math.Min(qrAvailableWidth, whiteAreaHeight));
        float qrY = layoutY0 + (whiteAreaHeight - qrZoneSize) / 2.0f;
        var qrRect = new RectangleF(
            layoutX0,
            qrY,
            qrZoneSize,
            qrZoneSize);

        float textX = layoutX0 + qrZoneSize + TextGapMm;
        float textWidth = layoutX1 - textX;

        return new LabelQrLayoutGeometry(
            qrRect,
            new RectangleF(textX, titleY, textWidth, titleHeight),
            new RectangleF(textX, numberY, textWidth, numberHeight),
            new RectangleF(layoutX0, barY, layoutWidth, barHeight),
            qrZoneSize,
            layoutX0,
            layoutY0,
            layoutWidth,
            layoutHeight,
            whiteAreaHeight);
    }
}
