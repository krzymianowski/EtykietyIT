using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using EtykietyIT.Services;

namespace EtykietyIT.Printing;

public sealed class LabelPrintJob : IDisposable
{
    private const string Company = "Dolnośląskie Młyny S.A.";
    private const int PaperSizeTolerance = 8;
    private const float HundredthsInchToMillimeters = 0.254f;

    private readonly LabelPrintOptions _options;
    private readonly PaperSelection _paperSelection;
    private readonly StringFormat _center;
    private readonly Pen _cutPen;

    private int _index;
    private bool _disposed;

    public LabelPrintJob(LabelPrintOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        ValidateOptions(options);
        _options = options;

        Document = new PrintDocument
        {
            DocumentName = "Etykiety inwentarzowe IT",
            OriginAtMargins = false
        };
        Document.PrinterSettings.PrinterName = options.PrinterName;

        _paperSelection = SelectPaperSize(
            Document.PrinterSettings,
            options.WidthMm,
            options.HeightMm);

        Document.DefaultPageSettings.PaperSize = _paperSelection.PaperSize;
        Document.DefaultPageSettings.Landscape = _paperSelection.Landscape;
        Document.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

        _center = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        _cutPen = new Pen(Color.Black, 0.25f)
        {
            DashStyle = DashStyle.Dash
        };

        Document.BeginPrint += OnBeginPrint;
        Document.PrintPage += OnPrintPage;
    }

    public PrintDocument Document { get; }

    public string PaperName => _paperSelection.PaperSize.PaperName;

    public bool IsLandscape => _paperSelection.Landscape;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Document.BeginPrint -= OnBeginPrint;
        Document.PrintPage -= OnPrintPage;
        _center.Dispose();
        _cutPen.Dispose();
        Document.Dispose();

        _disposed = true;
    }

    private static void ValidateOptions(LabelPrintOptions options)
    {
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

        if (options.WidthMm < 20 || options.HeightMm < 15)
        {
            throw new InvalidOperationException("Rozmiar etykiety jest zbyt mały.");
        }

        if (options.Columns < 1 || options.Rows < 1)
        {
            throw new InvalidOperationException(
                "Układ etykiet musi mieć co najmniej 1 kolumnę i 1 wiersz.");
        }
    }

    private static PaperSelection SelectPaperSize(
        PrinterSettings printerSettings,
        double widthMm,
        double heightMm)
    {
        int targetWidth = (int)Math.Round((widthMm / 25.4) * 100);
        int targetHeight = (int)Math.Round((heightMm / 25.4) * 100);

        foreach (PaperSize paper in printerSettings.PaperSizes)
        {
            bool normal =
                Math.Abs(paper.Width - targetWidth) <= PaperSizeTolerance &&
                Math.Abs(paper.Height - targetHeight) <= PaperSizeTolerance;

            bool rotated =
                Math.Abs(paper.Width - targetHeight) <= PaperSizeTolerance &&
                Math.Abs(paper.Height - targetWidth) <= PaperSizeTolerance;

            if (normal)
            {
                return new PaperSelection(paper, false);
            }

            if (rotated)
            {
                return new PaperSelection(paper, true);
            }
        }

        var customPaper = new PaperSize(
            $"{widthMm} x {heightMm} mm",
            targetWidth,
            targetHeight);

        return new PaperSelection(customPaper, false);
    }

    private void OnBeginPrint(object? sender, PrintEventArgs e)
    {
        _index = 0;
    }

    private void OnPrintPage(object? sender, PrintPageEventArgs e)
    {
        Graphics graphics = e.Graphics ?? throw new InvalidOperationException(
            "Sterownik drukarki nie udostępnił obiektu Graphics.");
        graphics.PageUnit = GraphicsUnit.Millimeter;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        float pageWidth = e.PageSettings.Bounds.Width * HundredthsInchToMillimeters;
        float pageHeight = e.PageSettings.Bounds.Height * HundredthsInchToMillimeters;

        if (pageWidth <= 0)
        {
            pageWidth = (float)_options.WidthMm;
        }

        if (pageHeight <= 0)
        {
            pageHeight = (float)_options.HeightMm;
        }

        bool isPreview = _options.RenderMode == LabelRenderMode.Preview;
        float safeEdgeX;
        float safeEdgeY;

        if (isPreview)
        {
            safeEdgeX = Math.Max(1.0f, Math.Min(1.5f, pageWidth * 0.02f));
            safeEdgeY = Math.Max(1.0f, Math.Min(1.5f, pageHeight * 0.035f));
        }
        else
        {
            float hardLeft =
                e.PageSettings.HardMarginX * HundredthsInchToMillimeters;
            float hardTop =
                e.PageSettings.HardMarginY * HundredthsInchToMillimeters;

            float printableWidth = graphics.VisibleClipBounds.Width;
            float printableHeight = graphics.VisibleClipBounds.Height;

            float hardRight = pageWidth - hardLeft - printableWidth;
            float hardBottom = pageHeight - hardTop - printableHeight;

            if (hardLeft < 0 || hardLeft > pageWidth / 2.0f)
            {
                hardLeft = 0.0f;
            }

            if (hardTop < 0 || hardTop > pageHeight / 2.0f)
            {
                hardTop = 0.0f;
            }

            if (hardRight < 0 || hardRight > pageWidth / 2.0f)
            {
                hardRight = 0.0f;
            }

            if (hardBottom < 0 || hardBottom > pageHeight / 2.0f)
            {
                hardBottom = 0.0f;
            }

            safeEdgeX = Math.Max(1.0f, Math.Max(hardLeft, hardRight));
            safeEdgeY = Math.Max(1.0f, Math.Max(hardTop, hardBottom));

            graphics.TranslateTransform(-hardLeft, -hardTop);

            PrinterCalibration calibration =
                _options.Calibration ?? new PrinterCalibration();

            if (calibration.OffsetXmm != 0.0 || calibration.OffsetYmm != 0.0)
            {
                graphics.TranslateTransform(
                    (float)calibration.OffsetXmm,
                    (float)calibration.OffsetYmm);
            }
        }

        float cellWidth = pageWidth / _options.Columns;
        float cellHeight = pageHeight / _options.Rows;

        if (_options.DrawCutLines)
        {
            if (_options.Columns > 1)
            {
                for (int column = 1; column < _options.Columns; column++)
                {
                    float cutX = cellWidth * column;
                    graphics.DrawLine(
                        _cutPen,
                        cutX,
                        safeEdgeY,
                        cutX,
                        pageHeight - safeEdgeY);
                }
            }

            if (_options.Rows > 1)
            {
                for (int row = 1; row < _options.Rows; row++)
                {
                    float cutY = cellHeight * row;
                    graphics.DrawLine(
                        _cutPen,
                        safeEdgeX,
                        cutY,
                        pageWidth - safeEdgeX,
                        cutY);
                }
            }
        }

        int slotsPerPage = _options.Columns * _options.Rows;

        for (int slot = 0; slot < slotsPerPage; slot++)
        {
            if (_index >= _options.Quantity)
            {
                break;
            }

            int column = slot % _options.Columns;
            int row = slot / _options.Columns;

            float cellX0 = column * cellWidth;
            float cellX1 = (column + 1) * cellWidth;
            float cellY0 = row * cellHeight;
            float cellY1 = (row + 1) * cellHeight;

            float cellPadX = Math.Max(0.8f, Math.Min(1.5f, cellWidth * 0.035f));
            float cellPadY = Math.Max(0.8f, Math.Min(1.2f, cellHeight * 0.035f));

            float layoutX0 = Math.Max(cellX0 + cellPadX, safeEdgeX);
            float layoutX1 = Math.Min(cellX1 - cellPadX, pageWidth - safeEdgeX);
            float layoutY0 = Math.Max(cellY0 + cellPadY, safeEdgeY);
            float layoutY1 = Math.Min(cellY1 - cellPadY, pageHeight - safeEdgeY);

            float layoutWidth = layoutX1 - layoutX0;
            float layoutHeight = layoutY1 - layoutY0;

            if (layoutWidth < 10.0f || layoutHeight < 10.0f)
            {
                throw new InvalidOperationException(
                    "Obszar drukowalny sterownika jest zbyt mały dla wybranego układu etykiet.");
            }

            int number = _options.StartNumber + _index;
            string assetId = AssetIdFormatter.Format(number);

            float titleY = layoutY0 + layoutHeight * 0.07f;
            float titleHeight = Math.Max(3.0f, layoutHeight * 0.14f);

            float numberY = layoutY0 + layoutHeight * 0.23f;
            float numberHeight = Math.Max(6.0f, layoutHeight * 0.38f);

            float barHeight = Math.Max(5.0f, layoutHeight * 0.19f);
            float barY =
                layoutY1 - barHeight - Math.Max(0.8f, layoutHeight * 0.04f);

            var titleRect = new RectangleF(
                layoutX0,
                titleY,
                layoutWidth,
                titleHeight);
            var numberRect = new RectangleF(
                layoutX0,
                numberY,
                layoutWidth,
                numberHeight);
            var barRect = new RectangleF(
                layoutX0,
                barY,
                layoutWidth,
                barHeight);

            float titleMax =
                Math.Min(9.0f, Math.Max(5.0f, layoutHeight * 0.18f));
            float numberMax =
                Math.Min(24.0f, Math.Max(8.0f, layoutHeight * 0.50f));
            float companyMax =
                Math.Min(9.0f, Math.Max(4.5f, layoutHeight * 0.16f));

            Font titleFont = CreateFittingFont(
                graphics,
                "Nr inwentarzowy",
                titleRect,
                "Arial",
                FontStyle.Regular,
                titleMax,
                4.0f);
            Font numberFont = CreateFittingFont(
                graphics,
                assetId,
                numberRect,
                "Arial",
                FontStyle.Bold,
                numberMax,
                6.0f);
            Font companyFont = CreateFittingFont(
                graphics,
                Company,
                barRect,
                "Arial",
                FontStyle.Bold,
                companyMax,
                3.5f);

            try
            {
                graphics.DrawString(
                    "Nr inwentarzowy",
                    titleFont,
                    Brushes.Black,
                    titleRect,
                    _center);

                graphics.DrawString(
                    assetId,
                    numberFont,
                    Brushes.Black,
                    numberRect,
                    _center);

                graphics.FillRectangle(Brushes.Black, barRect);
                graphics.DrawString(
                    Company,
                    companyFont,
                    Brushes.White,
                    barRect,
                    _center);
            }
            finally
            {
                titleFont.Dispose();
                numberFont.Dispose();
                companyFont.Dispose();
            }

            _index++;
        }

        e.HasMorePages = _index < _options.Quantity;
    }

    private static Font CreateFittingFont(
        Graphics graphics,
        string text,
        RectangleF rectangle,
        string fontName,
        FontStyle style,
        float maxPt,
        float minPt)
    {
        float size = maxPt;

        while (size >= minPt)
        {
            var font = new Font(fontName, size, style);
            SizeF measured = graphics.MeasureString(text, font, 10000);

            if (measured.Width <= rectangle.Width &&
                measured.Height <= rectangle.Height * 1.15f)
            {
                return font;
            }

            font.Dispose();
            size -= 0.5f;
        }

        return new Font(fontName, minPt, style);
    }
}
