using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Globalization;
using EtykietyIT.Services;
using Net.Codecrete.QrCodeGenerator;

namespace EtykietyIT.Printing;

public sealed class LabelPrintabilityValidator
{
    internal const float MinimumReadableTitleFontPt = 5.0f;
    internal const float MinimumReadableAssetIdFontPt = 8.0f;
    internal const float MinimumReadableCompanyFontPt = 4.5f;

    private const float HundredthsInchToMillimeters = 0.254f;
    private const float GeometryToleranceMm = 0.01f;

    private static readonly CultureInfo PolishCulture =
        CultureInfo.GetCultureInfo("pl-PL");

    public LabelPrintabilityResult Validate(LabelPrintOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        try
        {
            ValidateBasicOptions(options);

            using var document = new PrintDocument
            {
                DocumentName = "Etykiety inwentarzowe IT",
                OriginAtMargins = false
            };
            document.PrinterSettings.PrinterName = options.PrinterName;

            PaperSelection paperSelection = LabelPrintJob.SelectPaperSize(
                document.PrinterSettings,
                options.WidthMm,
                options.HeightMm);
            document.DefaultPageSettings.PaperSize = paperSelection.PaperSize;
            document.DefaultPageSettings.Landscape = paperSelection.Landscape;
            document.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

            using Graphics graphics =
                document.PrinterSettings.CreateMeasurementGraphics(
                    document.DefaultPageSettings);
            graphics.PageUnit = GraphicsUnit.Millimeter;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            PageSettings pageSettings = document.DefaultPageSettings;
            float pageWidth =
                pageSettings.Bounds.Width * HundredthsInchToMillimeters;
            float pageHeight =
                pageSettings.Bounds.Height * HundredthsInchToMillimeters;

            if (pageWidth <= 0.0f)
            {
                pageWidth = (float)options.WidthMm;
            }

            if (pageHeight <= 0.0f)
            {
                pageHeight = (float)options.HeightMm;
            }

            float hardLeft =
                pageSettings.HardMarginX * HundredthsInchToMillimeters;
            float hardTop =
                pageSettings.HardMarginY * HundredthsInchToMillimeters;
            float printableWidth = graphics.VisibleClipBounds.Width;
            float printableHeight = graphics.VisibleClipBounds.Height;
            float hardRight = pageWidth - hardLeft - printableWidth;
            float hardBottom = pageHeight - hardTop - printableHeight;

            hardLeft = NormalizeHardMargin(hardLeft, pageWidth);
            hardTop = NormalizeHardMargin(hardTop, pageHeight);
            hardRight = NormalizeHardMargin(hardRight, pageWidth);
            hardBottom = NormalizeHardMargin(hardBottom, pageHeight);

            float safeEdgeX = Math.Max(1.0f, Math.Max(hardLeft, hardRight));
            float safeEdgeY = Math.Max(1.0f, Math.Max(hardTop, hardBottom));

            return ValidateGeometry(
                options,
                graphics,
                pageWidth,
                pageHeight,
                safeEdgeX,
                safeEdgeY,
                graphics.DpiX);
        }
        catch (Exception exception) when (
            exception is InvalidOperationException or ArgumentException)
        {
            return CreateConfigurationFailure(options, exception.Message);
        }
    }

    internal LabelPrintabilityResult ValidateGeometry(
        LabelPrintOptions options,
        Graphics graphics,
        float runtimePageWidthMm,
        float runtimePageHeightMm,
        float safeEdgeX,
        float safeEdgeY,
        float printerDpiX)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(graphics);

        var issues = new Dictionary<string, LabelPrintabilityIssue>(
            StringComparer.Ordinal);
        void AddIssue(
            LabelPrintabilitySeverity severity,
            string code,
            string message)
        {
            issues.TryAdd(code, new LabelPrintabilityIssue(severity, code, message));
        }

        if (!float.IsFinite(runtimePageWidthMm) || runtimePageWidthMm <= 0.0f ||
            !float.IsFinite(runtimePageHeightMm) || runtimePageHeightMm <= 0.0f)
        {
            AddIssue(
                LabelPrintabilitySeverity.Error,
                "PAGE_SIZE_INVALID",
                "Sterownik drukarki zwrócił nieprawidłowy rozmiar strony.");
            return CreateResult(
                issues.Values,
                runtimePageWidthMm,
                runtimePageHeightMm,
                0.0f,
                0.0f,
                printerDpiX);
        }

        if (options.Columns <= 0 || options.Rows <= 0)
        {
            AddIssue(
                LabelPrintabilitySeverity.Error,
                "GRID_SIZE_INVALID",
                "Liczba kolumn i wierszy musi być dodatnia.");
            return CreateResult(
                issues.Values,
                runtimePageWidthMm,
                runtimePageHeightMm,
                0.0f,
                0.0f,
                printerDpiX);
        }

        float cellWidth = runtimePageWidthMm / options.Columns;
        float cellHeight = runtimePageHeightMm / options.Rows;
        if (!float.IsFinite(cellWidth) || cellWidth <= 0.0f ||
            !float.IsFinite(cellHeight) || cellHeight <= 0.0f)
        {
            AddIssue(
                LabelPrintabilitySeverity.Error,
                "CELL_SIZE_INVALID",
                "Wyliczony rozmiar pojedynczej etykiety jest nieprawidłowy.");
            return CreateResult(
                issues.Values,
                runtimePageWidthMm,
                runtimePageHeightMm,
                cellWidth,
                cellHeight,
                printerDpiX);
        }

        if (options.Quantity <= 0)
        {
            AddIssue(
                LabelPrintabilitySeverity.Error,
                "QUANTITY_INVALID",
                "Liczba etykiet musi być dodatnia.");
        }

        int slotsPerPage;
        try
        {
            slotsPerPage = checked(options.Columns * options.Rows);
        }
        catch (OverflowException)
        {
            slotsPerPage = 0;
            AddIssue(
                LabelPrintabilitySeverity.Error,
                "GRID_SIZE_INVALID",
                "Układ kolumn i wierszy jest zbyt duży.");
        }

        Dictionary<int, string> assetIdsBySlot = GetLongestAssetIdsBySlot(
            options,
            graphics,
            slotsPerPage,
            AddIssue);

        float minimumContentWidth = float.PositiveInfinity;
        float minimumContentHeight = float.PositiveInfinity;
        float minimumWhiteWidth = float.PositiveInfinity;
        float minimumWhiteHeight = float.PositiveInfinity;
        float minimumBarHeight = float.PositiveInfinity;
        float minimumTitleFont = float.PositiveInfinity;
        float minimumAssetFont = float.PositiveInfinity;
        float minimumCompanyFont = float.PositiveInfinity;
        int? maximumQrSize = null;
        int? maximumQrTotalModules = null;
        int? minimumQrDotsPerModule = null;
        float? minimumQrFootprint = null;
        LabelPrintabilityGeometrySnapshot? diagnosticGeometry = null;

        foreach ((int slot, string assetId) in assetIdsBySlot)
        {
            int column = slot % options.Columns;
            int row = slot / options.Columns;

            float cellX0 = column * cellWidth;
            float cellX1 = (column + 1) * cellWidth;
            float cellY0 = row * cellHeight;
            float cellY1 = (row + 1) * cellHeight;

            float cellPadX = Math.Max(0.8f, Math.Min(1.5f, cellWidth * 0.035f));
            float cellPadY = Math.Max(0.8f, Math.Min(1.2f, cellHeight * 0.035f));

            float layoutX0 = Math.Max(cellX0 + cellPadX, safeEdgeX);
            float layoutX1 = Math.Min(
                cellX1 - cellPadX,
                runtimePageWidthMm - safeEdgeX);
            float layoutY0 = Math.Max(cellY0 + cellPadY, safeEdgeY);
            float layoutY1 = Math.Min(
                cellY1 - cellPadY,
                runtimePageHeightMm - safeEdgeY);
            float layoutWidth = layoutX1 - layoutX0;
            float layoutHeight = layoutY1 - layoutY0;

            minimumContentWidth = Math.Min(minimumContentWidth, layoutWidth);
            minimumContentHeight = Math.Min(minimumContentHeight, layoutHeight);

            if (layoutWidth <= 0.0f || layoutHeight <= 0.0f)
            {
                AddIssue(
                    LabelPrintabilitySeverity.Error,
                    "CONTENT_AREA_INVALID",
                    "Po uwzględnieniu marginesów i paddingów nie pozostaje dodatni obszar treści.");
                continue;
            }

            if (layoutWidth < 10.0f || layoutHeight < 10.0f)
            {
                AddIssue(
                    LabelPrintabilitySeverity.Error,
                    "CONTENT_AREA_TOO_SMALL",
                    "Obszar treści ma tylko " +
                    $"{FormatPolish(layoutWidth, "0.0")} × " +
                    $"{FormatPolish(layoutHeight, "0.0")} mm.");
            }

            RectangleF titleRect;
            RectangleF numberRect;
            RectangleF barRect;
            RectangleF qrRect = RectangleF.Empty;
            float? qrActualFootprintForSlot = null;
            int? qrDotsPerModuleForSlot = null;

            if (options.Content.QrEnabled)
            {
                LabelQrLayoutGeometry geometry =
                    LabelQrLayoutGeometry.Calculate(
                        layoutX0,
                        layoutX1,
                        layoutY0,
                        layoutY1);
                titleRect = geometry.TitleRect;
                numberRect = geometry.AssetIdRect;
                barRect = geometry.CompanyBarRect;
                qrRect = geometry.QrRect;

                if (geometry.AvailableQrFootprintMm <= 0.0f)
                {
                    AddIssue(
                        LabelPrintabilitySeverity.Error,
                        "QR_AREA_INVALID",
                        "Brak dodatniego obszaru na kod QR.");
                }
                else if (qrRect.Bottom >
                    barRect.Top + GeometryToleranceMm)
                {
                    AddIssue(
                        LabelPrintabilitySeverity.Error,
                        "QR_OVERLAPS_COMPANY_BAR",
                        "Kod QR nachodziłby na czarny pasek firmy.");
                }

                if (titleRect.Width <= 0.0f ||
                    titleRect.Left < qrRect.Right +
                        LabelQrLayoutGeometry.TextGapMm - GeometryToleranceMm)
                {
                    AddIssue(
                        LabelPrintabilitySeverity.Error,
                        "QR_TEXT_COLLISION",
                        "Kod QR kolidowałby ze strefą tekstową.");
                }

                if (titleRect.Width <
                    LabelQrLayoutGeometry.MinimumTextWidthMm)
                {
                    AddIssue(
                        LabelPrintabilitySeverity.Error,
                        "QR_TEXT_AREA_TOO_NARROW",
                        "Po dodaniu QR obszar tekstu ma tylko " +
                        $"{FormatPolish(titleRect.Width, "0.0")} mm szerokości.");
                }

                QrCode qrCode = LabelQrRenderer.CreateQrCode(assetId);
                int totalModules = qrCode.Size +
                    LabelQrRenderer.QuietZoneModules * 2;
                maximumQrSize = Math.Max(maximumQrSize ?? 0, qrCode.Size);
                maximumQrTotalModules = Math.Max(
                    maximumQrTotalModules ?? 0,
                    totalModules);

                int possibleDots = CalculatePhysicalDotsPerModule(
                    totalModules,
                    printerDpiX,
                    geometry.AvailableQrFootprintMm);
                qrDotsPerModuleForSlot = possibleDots;
                minimumQrDotsPerModule = Math.Min(
                    minimumQrDotsPerModule ?? int.MaxValue,
                    possibleDots);

                if (possibleDots > 0 && printerDpiX > 0.0f)
                {
                    float footprint = totalModules * possibleDots *
                        25.4f / printerDpiX;
                    qrActualFootprintForSlot = footprint;
                    minimumQrFootprint = Math.Min(
                        minimumQrFootprint ?? float.PositiveInfinity,
                        footprint);
                }

                try
                {
                    LabelQrLayout qrLayout = LabelQrRenderer.CalculateLayout(
                        qrCode.Size,
                        printerDpiX,
                        LabelRenderMode.Print,
                        geometry.AvailableQrFootprintMm);
                    qrActualFootprintForSlot = qrLayout.ActualFootprintMm;
                    minimumQrFootprint = Math.Min(
                        minimumQrFootprint ?? float.PositiveInfinity,
                        qrLayout.ActualFootprintMm);
                }
                catch (InvalidOperationException exception)
                {
                    AddIssue(
                        LabelPrintabilitySeverity.Error,
                        "QR_NOT_PRINTABLE",
                        exception.Message);
                }
            }
            else
            {
                float titleY = layoutY0 + layoutHeight * 0.07f;
                float titleHeight = Math.Max(3.0f, layoutHeight * 0.14f);
                float numberY = layoutY0 + layoutHeight * 0.23f;
                float numberHeight = Math.Max(6.0f, layoutHeight * 0.38f);
                float barHeight = Math.Max(5.0f, layoutHeight * 0.19f);
                float barY = layoutY1 - barHeight -
                    Math.Max(0.8f, layoutHeight * 0.04f);

                titleRect = new RectangleF(
                    layoutX0,
                    titleY,
                    layoutWidth,
                    titleHeight);
                numberRect = new RectangleF(
                    layoutX0,
                    numberY,
                    layoutWidth,
                    numberHeight);
                barRect = new RectangleF(
                    layoutX0,
                    barY,
                    layoutWidth,
                    barHeight);
            }

            float whiteHeight = barRect.Top - layoutY0;
            minimumWhiteWidth = Math.Min(minimumWhiteWidth, layoutWidth);
            minimumWhiteHeight = Math.Min(minimumWhiteHeight, whiteHeight);
            minimumBarHeight = Math.Min(minimumBarHeight, barRect.Height);

            if (barRect.Height <= 0.0f || barRect.Top < layoutY0 ||
                barRect.Bottom > layoutY1 + GeometryToleranceMm)
            {
                AddIssue(
                    LabelPrintabilitySeverity.Error,
                    "COMPANY_BAR_INVALID",
                    "Brak miejsca na czarny pasek firmy w obszarze etykiety.");
            }

            if (whiteHeight <= 0.0f)
            {
                AddIssue(
                    LabelPrintabilitySeverity.Error,
                    "WHITE_AREA_INVALID",
                    "Po odjęciu paska firmy nie pozostaje dodatnia biała część etykiety.");
            }

            float titleMax =
                Math.Min(9.0f, Math.Max(5.0f, layoutHeight * 0.18f));
            float numberMax =
                Math.Min(24.0f, Math.Max(8.0f, layoutHeight * 0.50f));
            float companyMax =
                Math.Min(9.0f, Math.Max(4.5f, layoutHeight * 0.16f));

            FontFit titleFit = FindFittingFont(
                graphics,
                "Nr inwentarzowy",
                titleRect,
                "Arial",
                FontStyle.Regular,
                titleMax,
                4.0f);
            FontFit assetFit = FindFittingFont(
                graphics,
                assetId,
                numberRect,
                "Arial",
                FontStyle.Bold,
                numberMax,
                6.0f);
            FontFit companyFit = FindFittingFont(
                graphics,
                options.Content.CompanyName,
                barRect,
                "Arial",
                FontStyle.Bold,
                companyMax,
                3.5f);

            minimumTitleFont = Math.Min(minimumTitleFont, titleFit.SizePt);
            minimumAssetFont = Math.Min(minimumAssetFont, assetFit.SizePt);
            minimumCompanyFont = Math.Min(minimumCompanyFont, companyFit.SizePt);

            RectangleF titleTextBounds = CalculateCenteredTextBounds(
                titleRect,
                titleFit.MeasuredSize);
            RectangleF assetTextBounds = CalculateCenteredTextBounds(
                numberRect,
                assetFit.MeasuredSize);
            RectangleF companyTextBounds = CalculateCenteredTextBounds(
                barRect,
                companyFit.MeasuredSize);

            if (titleFit.Fits && assetFit.Fits &&
                titleTextBounds.Bottom >
                    assetTextBounds.Top + GeometryToleranceMm)
            {
                AddIssue(
                    LabelPrintabilitySeverity.Error,
                    "TITLE_ASSET_ID_COLLISION",
                    "Tytuł i Asset ID nachodziłyby na siebie.");
            }

            if (titleFit.Fits &&
                (titleTextBounds.Top < layoutY0 - GeometryToleranceMm ||
                 titleTextBounds.Bottom >
                    barRect.Top + GeometryToleranceMm))
            {
                AddIssue(
                    LabelPrintabilitySeverity.Error,
                    "TITLE_GEOMETRY_INVALID",
                    "Tytuł wychodzi poza białą część etykiety.");
            }

            if (assetFit.Fits &&
                assetTextBounds.Bottom >
                    barRect.Top + GeometryToleranceMm)
            {
                AddIssue(
                    LabelPrintabilitySeverity.Error,
                    "ASSET_ID_OVERLAPS_COMPANY_BAR",
                    "Tekst Asset ID nachodziłby na czarny pasek firmy.");
            }

            diagnosticGeometry ??= new LabelPrintabilityGeometrySnapshot(
                new RectangleF(
                    layoutX0,
                    layoutY0,
                    layoutWidth,
                    layoutHeight),
                qrRect,
                titleRect,
                numberRect,
                barRect,
                titleTextBounds,
                assetTextBounds,
                companyTextBounds,
                qrActualFootprintForSlot,
                qrDotsPerModuleForSlot);

            ValidateFontReadability(
                titleFit,
                MinimumReadableTitleFontPt,
                "TITLE",
                "Tytuł „Nr inwentarzowy”",
                AddIssue);
            ValidateFontReadability(
                assetFit,
                MinimumReadableAssetIdFontPt,
                "ASSET_ID",
                $"Asset ID „{assetId}”",
                AddIssue);
            ValidateFontReadability(
                companyFit,
                MinimumReadableCompanyFontPt,
                "COMPANY",
                "Nazwa firmy",
                AddIssue);
        }

        return new LabelPrintabilityResult
        {
            Issues = issues.Values.ToArray(),
            RuntimePageWidthMm = runtimePageWidthMm,
            RuntimePageHeightMm = runtimePageHeightMm,
            CellWidthMm = cellWidth,
            CellHeightMm = cellHeight,
            ContentAreaWidthMm = NormalizeMinimum(minimumContentWidth),
            ContentAreaHeightMm = NormalizeMinimum(minimumContentHeight),
            WhiteAreaWidthMm = NormalizeMinimum(minimumWhiteWidth),
            WhiteAreaHeightMm = NormalizeMinimum(minimumWhiteHeight),
            CompanyBarHeightMm = NormalizeMinimum(minimumBarHeight),
            TitleFontSizePt = NormalizeMinimum(minimumTitleFont),
            AssetIdFontSizePt = NormalizeMinimum(minimumAssetFont),
            CompanyFontSizePt = NormalizeMinimum(minimumCompanyFont),
            PrinterDpiX = printerDpiX,
            QrSize = maximumQrSize,
            QrTotalModules = maximumQrTotalModules,
            QrDotsPerModule = minimumQrDotsPerModule,
            QrFootprintMm = minimumQrFootprint,
            DiagnosticGeometry = diagnosticGeometry
        };
    }

    private static void ValidateBasicOptions(LabelPrintOptions options)
    {
        ArgumentNullException.ThrowIfNull(options.Content);
        options.Content.Validate();

        bool printerInstalled = PrinterSettings.InstalledPrinters
            .Cast<string>()
            .Any(printerName => string.Equals(
                printerName,
                options.PrinterName,
                StringComparison.OrdinalIgnoreCase));
        if (!printerInstalled)
        {
            throw new InvalidOperationException(
                $"Nie znaleziono drukarki '{options.PrinterName}'.");
        }

        if (options.WidthMm < 20.0 || options.HeightMm < 15.0)
        {
            throw new InvalidOperationException("Rozmiar etykiety jest zbyt mały.");
        }

        if (options.Columns < 1 || options.Rows < 1)
        {
            throw new InvalidOperationException(
                "Układ etykiet musi mieć co najmniej 1 kolumnę i 1 wiersz.");
        }
    }

    private static Dictionary<int, string> GetLongestAssetIdsBySlot(
        LabelPrintOptions options,
        Graphics graphics,
        int slotsPerPage,
        Action<LabelPrintabilitySeverity, string, string> addIssue)
    {
        var result = new Dictionary<int, string>();
        if (slotsPerPage <= 0 || options.Quantity <= 0)
        {
            return result;
        }

        using var comparisonFont = new Font("Arial", 10.0f, FontStyle.Bold);
        var widths = new Dictionary<int, float>();

        for (int index = 0; index < options.Quantity; index++)
        {
            int number;
            try
            {
                number = checked(options.StartNumber + index);
            }
            catch (OverflowException)
            {
                addIssue(
                    LabelPrintabilitySeverity.Error,
                    "ASSET_ID_RANGE_OVERFLOW",
                    "Zakres numerów Asset ID przekracza obsługiwany zakres liczb.");
                break;
            }

            string assetId = AssetIdFormatter.Format(
                number,
                options.Content.AssetIdPrefix,
                options.Content.AssetIdDigits);
            int slot = index % slotsPerPage;
            float width = graphics.MeasureString(assetId, comparisonFont, 10000).Width;
            if (!widths.TryGetValue(slot, out float currentWidth) ||
                width > currentWidth)
            {
                widths[slot] = width;
                result[slot] = assetId;
            }
        }

        return result;
    }

    private static FontFit FindFittingFont(
        Graphics graphics,
        string text,
        RectangleF rectangle,
        string fontName,
        FontStyle style,
        float maxPt,
        float rendererMinimumPt)
    {
        if (rectangle.Width <= 0.0f || rectangle.Height <= 0.0f)
        {
            return new FontFit(rendererMinimumPt, false, SizeF.Empty);
        }

        float size = maxPt;
        while (size >= rendererMinimumPt)
        {
            using var font = new Font(fontName, size, style);
            SizeF measured = graphics.MeasureString(text, font, 10000);
            if (measured.Width <= rectangle.Width &&
                measured.Height <= rectangle.Height * 1.15f)
            {
                return new FontFit(size, true, measured);
            }

            size -= 0.5f;
        }

        using var minimumFont = new Font(fontName, rendererMinimumPt, style);
        SizeF minimumMeasured = graphics.MeasureString(
            text,
            minimumFont,
            10000);
        return new FontFit(rendererMinimumPt, false, minimumMeasured);
    }

    private static RectangleF CalculateCenteredTextBounds(
        RectangleF layoutRectangle,
        SizeF measuredSize)
    {
        return new RectangleF(
            layoutRectangle.X +
                (layoutRectangle.Width - measuredSize.Width) / 2.0f,
            layoutRectangle.Y +
                (layoutRectangle.Height - measuredSize.Height) / 2.0f,
            measuredSize.Width,
            measuredSize.Height);
    }

    private static void ValidateFontReadability(
        FontFit fit,
        float minimumReadablePt,
        string codePrefix,
        string fieldName,
        Action<LabelPrintabilitySeverity, string, string> addIssue)
    {
        if (!fit.Fits)
        {
            addIssue(
                LabelPrintabilitySeverity.Error,
                $"{codePrefix}_DOES_NOT_FIT",
                $"{fieldName} nie mieści się nawet przy minimalnym foncie renderera.");
            return;
        }

        if (fit.SizePt < minimumReadablePt)
        {
            addIssue(
                LabelPrintabilitySeverity.Error,
                $"{codePrefix}_FONT_TOO_SMALL",
                $"{fieldName} mieści się dopiero przy " +
                $"{FormatPolish(fit.SizePt, "0.##")} pt; wymagane minimum to " +
                $"{FormatPolish(minimumReadablePt, "0.##")} pt.");
            return;
        }

        if (fit.SizePt < minimumReadablePt + 0.5f)
        {
            addIssue(
                LabelPrintabilitySeverity.Warning,
                $"{codePrefix}_FONT_NEAR_MINIMUM",
                $"{fieldName} użyje fontu " +
                $"{FormatPolish(fit.SizePt, "0.##")} pt, blisko minimum " +
                $"{FormatPolish(minimumReadablePt, "0.##")} pt.");
        }
    }

    private static string FormatPolish(float value, string format)
    {
        return value.ToString(format, PolishCulture);
    }

    private static int CalculatePhysicalDotsPerModule(
        int totalModules,
        float dpiX,
        float footprintMm)
    {
        if (!float.IsFinite(footprintMm) || footprintMm <= 0.0f ||
            totalModules <= 0 || !float.IsFinite(dpiX) || dpiX <= 0.0f)
        {
            return 0;
        }

        return LabelQrRenderer.CalculatePhysicalDotsPerModule(
            totalModules,
            dpiX,
            footprintMm);
    }

    private static float NormalizeHardMargin(float margin, float pageDimension)
    {
        return margin < 0.0f || margin > pageDimension / 2.0f
            ? 0.0f
            : margin;
    }

    private static float NormalizeMinimum(float value)
    {
        return float.IsPositiveInfinity(value) ? 0.0f : value;
    }

    private static LabelPrintabilityResult CreateConfigurationFailure(
        LabelPrintOptions options,
        string message)
    {
        return new LabelPrintabilityResult
        {
            Issues =
            [
                new LabelPrintabilityIssue(
                    LabelPrintabilitySeverity.Error,
                    "PRINTER_CONFIGURATION_FAILED",
                    message)
            ],
            RuntimePageWidthMm = (float)options.WidthMm,
            RuntimePageHeightMm = (float)options.HeightMm,
            CellWidthMm = options.Columns > 0
                ? (float)options.WidthMm / options.Columns
                : 0.0f,
            CellHeightMm = options.Rows > 0
                ? (float)options.HeightMm / options.Rows
                : 0.0f
        };
    }

    private static LabelPrintabilityResult CreateResult(
        IEnumerable<LabelPrintabilityIssue> issues,
        float pageWidth,
        float pageHeight,
        float cellWidth,
        float cellHeight,
        float dpiX)
    {
        return new LabelPrintabilityResult
        {
            Issues = issues.ToArray(),
            RuntimePageWidthMm = pageWidth,
            RuntimePageHeightMm = pageHeight,
            CellWidthMm = cellWidth,
            CellHeightMm = cellHeight,
            PrinterDpiX = dpiX
        };
    }

    private readonly record struct FontFit(
        float SizePt,
        bool Fits,
        SizeF MeasuredSize);
}
