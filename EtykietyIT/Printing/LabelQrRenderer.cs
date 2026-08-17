using System.Drawing.Drawing2D;
using System.Globalization;
using Net.Codecrete.QrCodeGenerator;

namespace EtykietyIT.Printing;

internal static class LabelQrRenderer
{
    internal const int QuietZoneModules = 4;
    internal const int MinimumDotsPerModule = 4;
    internal const float PreferredQrFootprintMm = 15.0f;

    private const float MillimetersPerInch = 25.4f;

    private static readonly CultureInfo PolishCulture =
        CultureInfo.GetCultureInfo("pl-PL");

    internal static QrCode CreateQrCode(string assetId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetId);
        return QrCode.EncodeText(assetId, QrCode.Ecc.Medium);
    }

    internal static LabelQrLayout CalculateLayout(
        int qrModules,
        float dpiX,
        LabelRenderMode renderMode,
        float maxFootprintMm)
    {
        if (qrModules <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(qrModules),
                "Liczba modułów QR musi być dodatnia.");
        }

        int totalModules = checked(qrModules + QuietZoneModules * 2);
        float availableFootprintMm = Math.Min(
            PreferredQrFootprintMm,
            maxFootprintMm);

        if (!float.IsFinite(availableFootprintMm) ||
            availableFootprintMm <= 0.0f)
        {
            throw new InvalidOperationException(
                "Brak miejsca na kod QR w białej części etykiety.");
        }

        if (renderMode == LabelRenderMode.Preview)
        {
            float previewModuleSize = availableFootprintMm / totalModules;
            return new LabelQrLayout(
                qrModules,
                totalModules,
                null,
                previewModuleSize,
                availableFootprintMm);
        }

        if (!float.IsFinite(dpiX) || dpiX <= 0.0f)
        {
            throw new InvalidOperationException(
                "Drukarka zgłosiła nieprawidłową rozdzielczość poziomą: " +
                $"{dpiX.ToString("0.##", PolishCulture)} DPI.");
        }

        float dotSizeMm = MillimetersPerInch / dpiX;
        int dotsPerModule = CalculatePhysicalDotsPerModule(
            totalModules,
            dpiX,
            availableFootprintMm);

        if (dotsPerModule < MinimumDotsPerModule)
        {
            float minimumFootprintMm =
                totalModules * MinimumDotsPerModule * dotSizeMm;
            string availableFootprintText =
                availableFootprintMm.ToString("0.0", PolishCulture);
            string dpiText = dpiX.ToString("0.##", PolishCulture);
            string minimumFootprintText =
                minimumFootprintMm.ToString("0.0", PolishCulture);
            throw new InvalidOperationException(
                "QR nie mieści się w wybranym profilu etykiety. " +
                $"Dostępne miejsce: {availableFootprintText} mm. " +
                $"Minimalny wymagany rozmiar przy {dpiText} DPI: " +
                $"{minimumFootprintText} mm " +
                $"({MinimumDotsPerModule} punkty na moduł). " +
                $"Macierz: {qrModules} × {qrModules} modułów, " +
                $"łącznie z quiet zone: {totalModules} × {totalModules}, " +
                $"możliwe: {dotsPerModule} dots/module.");
        }

        float moduleSizeMm = dotsPerModule * dotSizeMm;
        float actualFootprintMm = totalModules * moduleSizeMm;

        return new LabelQrLayout(
            qrModules,
            totalModules,
            dotsPerModule,
            moduleSizeMm,
            actualFootprintMm);
    }

    internal static LabelQrLayout ValidatePhysicalPrint(
        string assetId,
        float dpiX,
        float maxFootprintMm)
    {
        QrCode qrCode = CreateQrCode(assetId);
        return CalculateLayout(
            qrCode.Size,
            dpiX,
            LabelRenderMode.Print,
            maxFootprintMm);
    }

    internal static int CalculatePhysicalDotsPerModule(
        int totalModules,
        float dpiX,
        float availableFootprintMm)
    {
        return (int)MathF.Floor(
            availableFootprintMm /
            totalModules /
            (MillimetersPerInch / dpiX));
    }

    internal static void Draw(
        Graphics graphics,
        string assetId,
        RectangleF zone,
        LabelRenderMode renderMode,
        float? previewPrinterDpiX = null)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        QrCode qrCode = CreateQrCode(assetId);
        float maxFootprintMm = Math.Min(zone.Width, zone.Height);
        LabelQrLayout layout;
        if (renderMode == LabelRenderMode.Preview &&
            previewPrinterDpiX is > 0.0f)
        {
            try
            {
                layout = CalculateLayout(
                    qrCode.Size,
                    previewPrinterDpiX.Value,
                    LabelRenderMode.Print,
                    maxFootprintMm);
            }
            catch (InvalidOperationException)
            {
                layout = CalculateLayout(
                    qrCode.Size,
                    graphics.DpiX,
                    LabelRenderMode.Preview,
                    maxFootprintMm);
            }
        }
        else
        {
            layout = CalculateLayout(
                qrCode.Size,
                graphics.DpiX,
                renderMode,
                maxFootprintMm);
        }

        float footprintX = zone.X + (zone.Width - layout.ActualFootprintMm) / 2.0f;
        float footprintY = zone.Y + (zone.Height - layout.ActualFootprintMm) / 2.0f;
        var footprint = new RectangleF(
            footprintX,
            footprintY,
            layout.ActualFootprintMm,
            layout.ActualFootprintMm);

        GraphicsState state = graphics.Save();
        try
        {
            graphics.SmoothingMode = SmoothingMode.None;
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = PixelOffsetMode.None;

            graphics.FillRectangle(Brushes.White, footprint);

            float modulesX = footprintX + QuietZoneModules * layout.ModuleSizeMm;
            float modulesY = footprintY + QuietZoneModules * layout.ModuleSizeMm;
            foreach (QrRectangle rectangle in qrCode.ToRectangles())
            {
                graphics.FillRectangle(
                    Brushes.Black,
                    modulesX + rectangle.X * layout.ModuleSizeMm,
                    modulesY + rectangle.Y * layout.ModuleSizeMm,
                    rectangle.Width * layout.ModuleSizeMm,
                    rectangle.Height * layout.ModuleSizeMm);
            }
        }
        finally
        {
            graphics.Restore(state);
        }
    }
}

internal readonly record struct LabelQrLayout(
    int QrModules,
    int TotalModules,
    int? DotsPerModule,
    float ModuleSizeMm,
    float ActualFootprintMm);
