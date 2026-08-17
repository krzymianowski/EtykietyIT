using System.Drawing;
using EtykietyIT.Printing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Printing;

[TestClass]
public sealed class LabelPrintabilityValidatorTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    [DataRow(1, 1, false)]
    [DataRow(2, 1, false)]
    [DataRow(1, 1, true)]
    [DataRow(2, 1, true)]
    [DataRow(2, 2, false)]
    [DataRow(2, 2, true)]
    public void Accepted89By41Layouts_ArePrintableAt300Dpi(
        int columns,
        int rows,
        bool qrEnabled)
    {
        LabelPrintabilityResult result = Validate(
            CreateOptions(columns, rows, qrEnabled),
            safeEdgeMm: 1.5f);

        TestContext.WriteLine(
            $"{columns}x{rows}, QR={qrEnabled}: " +
            $"title={result.TitleFontSizePt:0.##} pt, " +
            $"asset={result.AssetIdFontSizePt:0.##} pt, " +
            $"company={result.CompanyFontSizePt:0.##} pt, " +
            $"white={result.WhiteAreaHeightMm:0.###} mm, " +
            $"QR={result.QrDotsPerModule?.ToString() ?? "—"} dots/module");

        Assert.IsTrue(result.IsPrintable, FormatIssues(result));
    }

    [TestMethod]
    public void TwoByTwoQr_ReportsPhysicallyValidatedQrGeometry()
    {
        LabelPrintabilityResult result = Validate(CreateOptions(2, 2, true));

        Assert.IsTrue(result.IsPrintable, FormatIssues(result));
        Assert.AreEqual(21, result.QrSize);
        Assert.AreEqual(29, result.QrTotalModules);
        Assert.AreEqual(5, result.QrDotsPerModule);
        Assert.AreEqual(
            12.2767f,
            result.QrFootprintMm.GetValueOrDefault(),
            0.001f);
    }

    [TestMethod]
    public void Dymo550GoldenCase_TwoByTwoQr_IsPrintableWithDriverSafeEdges()
    {
        LabelPrintabilityResult result = Validate(
            CreateOptions(2, 2, true, quantity: 4),
            safeEdgeMm: 1.5f);

        Assert.IsTrue(result.IsPrintable, FormatIssues(result));
        Assert.IsFalse(result.Issues.Any(issue =>
            issue.Severity == LabelPrintabilitySeverity.Error &&
            issue.Code is "TITLE_ASSET_ID_COLLISION" or
                "ASSET_ID_OVERLAPS_COMPANY_BAR" or
                "QR_NOT_PRINTABLE" or
                "COMPANY_FONT_TOO_SMALL"));
        Assert.AreEqual(5, result.QrDotsPerModule);
        Assert.AreEqual(
            12.2767f,
            result.QrFootprintMm.GetValueOrDefault(),
            0.001f);

        LabelPrintabilityGeometrySnapshot validatorGeometry =
            result.DiagnosticGeometry.GetValueOrDefault();
        LabelQrLayoutGeometry rendererGeometry =
            LabelQrLayoutGeometry.Calculate(
                1.5f,
                43.0f,
                1.5f,
                19.7f);

        AssertRectanglesEqual(
            rendererGeometry.TitleRect,
            validatorGeometry.TitleRect);
        AssertRectanglesEqual(
            rendererGeometry.AssetIdRect,
            validatorGeometry.AssetIdRect);
        AssertRectanglesEqual(
            rendererGeometry.CompanyBarRect,
            validatorGeometry.CompanyBarRect);
        AssertRectanglesEqual(
            rendererGeometry.QrRect,
            validatorGeometry.QrRect);

        Assert.IsGreaterThan(
            rendererGeometry.AssetIdRect.Top,
            rendererGeometry.TitleRect.Bottom,
            "Golden case powinien odtwarzać dawne fałszywe przecięcie stref layoutu.");
        Assert.IsLessThanOrEqualTo(
            validatorGeometry.AssetIdTextBounds.Top,
            validatorGeometry.TitleTextBounds.Bottom,
            "Faktycznie wyśrodkowane teksty nie mogą na siebie nachodzić.");

        WriteGoldenGeometryDiagnostics(
            result,
            rendererGeometry,
            validatorGeometry);
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(4)]
    public void TwoByTwoQr_IsPrintableForActuallyOccupiedSlots(int quantity)
    {
        LabelPrintabilityResult result = Validate(
            CreateOptions(2, 2, true, quantity: quantity),
            safeEdgeMm: 1.5f);

        Assert.IsTrue(result.IsPrintable, FormatIssues(result));
        Assert.AreEqual(5, result.QrDotsPerModule);
    }

    [TestMethod]
    public void ThreeByThree_ReturnsConcreteGeometryIssues()
    {
        LabelPrintabilityResult result = Validate(
            CreateOptions(3, 3, false),
            safeEdgeMm: 1.5f);

        Assert.IsFalse(result.IsPrintable);
        TestContext.WriteLine(
            $"3x3: cell={result.CellWidthMm:0.###} × " +
            $"{result.CellHeightMm:0.###} mm, " +
            $"content={result.ContentAreaWidthMm:0.###} × " +
            $"{result.ContentAreaHeightMm:0.###} mm, " +
            $"white={result.WhiteAreaHeightMm:0.###} mm");
        TestContext.WriteLine(FormatIssues(result));
        Assert.HasCount(
            1,
            result.Issues.Where(issue =>
                issue.Code == "ASSET_ID_OVERLAPS_COMPANY_BAR"));
        Assert.AreEqual(89.0 / 3.0, result.CellWidthMm, 0.01);
        Assert.AreEqual(41.0 / 3.0, result.CellHeightMm, 0.01);
    }

    [TestMethod]
    public void CellTooShortForWhiteArea_ReturnsError()
    {
        LabelPrintabilityResult result = Validate(CreateOptions(2, 8, false));

        Assert.IsFalse(result.IsPrintable);
        Assert.IsTrue(result.Issues.Any(issue =>
            issue.Code is "CONTENT_AREA_TOO_SMALL" or "WHITE_AREA_INVALID"));
    }

    [TestMethod]
    public void AssetIdThatWouldEnterCompanyBar_ReturnsError()
    {
        LabelPrintabilityResult result = Validate(CreateOptions(3, 3, false));

        Assert.IsTrue(result.Issues.Any(issue =>
            issue.Code == "ASSET_ID_OVERLAPS_COMPANY_BAR" &&
            issue.Severity == LabelPrintabilitySeverity.Error));
    }

    [TestMethod]
    public void AssetIdBelowReadableMinimum_ReturnsError()
    {
        LabelPrintabilityResult result = Validate(CreateOptions(
            2,
            2,
            false,
            prefix: new string('A', 17)));

        TestContext.WriteLine(
            $"Graniczny Asset ID: {result.AssetIdFontSizePt:0.##} pt");
        Assert.IsFalse(result.IsPrintable);
        Assert.IsTrue(
            result.Issues.Any(issue =>
                issue.Code == "ASSET_ID_FONT_TOO_SMALL"),
            FormatIssues(result));
    }

    [TestMethod]
    public void VeryLongPrefix_ReturnsAssetIdDoesNotFitError()
    {
        LabelPrintabilityResult result = Validate(CreateOptions(
            2,
            2,
            false,
            prefix: new string('W', 100)));

        Assert.IsFalse(result.IsPrintable);
        Assert.IsTrue(result.Issues.Any(issue =>
            issue.Code == "ASSET_ID_DOES_NOT_FIT"));
    }

    [TestMethod]
    public void VeryLongCompanyName_ReturnsError()
    {
        LabelPrintabilityResult result = Validate(CreateOptions(
            2,
            2,
            false,
            companyName: string.Join(' ', Enumerable.Repeat(
                "Nadzwyczajnie długa nazwa organizacji",
                10))));

        Assert.IsFalse(result.IsPrintable);
        Assert.IsTrue(result.Issues.Any(issue =>
            issue.Code is "COMPANY_FONT_TOO_SMALL" or
                "COMPANY_DOES_NOT_FIT"));
    }

    [TestMethod]
    public void QrBelowFourDotsPerModule_ReturnsError()
    {
        LabelPrintabilityResult result = Validate(
            CreateOptions(2, 2, true),
            dpiX: 96.0f);

        Assert.IsFalse(result.IsPrintable);
        Assert.IsTrue(result.Issues.Any(issue =>
            issue.Code == "QR_NOT_PRINTABLE"));
        Assert.IsLessThan(
            LabelQrRenderer.MinimumDotsPerModule,
            result.QrDotsPerModule.GetValueOrDefault());
    }

    [TestMethod]
    public void ThreeByThreeWithQr_ReturnsGeometryAndQrErrors()
    {
        LabelPrintabilityResult result = Validate(
            CreateOptions(3, 3, true),
            safeEdgeMm: 1.5f);

        Assert.IsFalse(result.IsPrintable);
        Assert.IsTrue(result.Issues.Any(issue =>
            issue.Code == "QR_NOT_PRINTABLE"));
        Assert.IsTrue(result.Issues.Any(issue =>
            issue.Code == "ASSET_ID_OVERLAPS_COMPANY_BAR"));
    }

    [TestMethod]
    public void LargerQrMatrix_IsEvaluatedDynamically()
    {
        LabelPrintabilityResult result = Validate(CreateOptions(
            2,
            2,
            true,
            prefix: new string('A', 60)));

        Assert.IsGreaterThan(21, result.QrSize.GetValueOrDefault());
        Assert.AreEqual(
            result.QrSize + LabelQrRenderer.QuietZoneModules * 2,
            result.QrTotalModules);
        Assert.IsFalse(result.IsPrintable);
        Assert.IsTrue(result.Issues.Any(issue =>
            issue.Code == "QR_NOT_PRINTABLE"));
    }

    [TestMethod]
    public void PreviewAndPrint_UseTheSamePhysicalPreflightResult()
    {
        LabelPrintOptions printOptions = CreateOptions(3, 3, true) with
        {
            RenderMode = LabelRenderMode.Print
        };
        LabelPrintOptions previewOptions = printOptions with
        {
            RenderMode = LabelRenderMode.Preview
        };

        LabelPrintabilityResult printResult = Validate(printOptions);
        LabelPrintabilityResult previewResult = Validate(previewOptions);

        Assert.AreEqual(printResult.IsPrintable, previewResult.IsPrintable);
        CollectionAssert.AreEqual(
            printResult.Issues.Select(issue => issue.Code).ToArray(),
            previewResult.Issues.Select(issue => issue.Code).ToArray());
        Assert.IsFalse(previewResult.IsPrintable);
    }

    private static LabelPrintOptions CreateOptions(
        int columns,
        int rows,
        bool qrEnabled,
        string prefix = "IT-",
        string companyName = "Dolnośląskie Młyny S.A.",
        int? quantity = null)
    {
        return new LabelPrintOptions(
            "Test printer",
            123,
            quantity ?? columns * rows,
            89.0,
            41.0,
            columns,
            rows,
            true,
            new LabelContentOptions(companyName, prefix, 6, qrEnabled),
            RenderMode: LabelRenderMode.Print);
    }

    private static LabelPrintabilityResult Validate(
        LabelPrintOptions options,
        float dpiX = 300.0f,
        float safeEdgeMm = 1.0f)
    {
        using var bitmap = new Bitmap(1600, 800);
        bitmap.SetResolution(dpiX, dpiX);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.PageUnit = GraphicsUnit.Millimeter;

        return new LabelPrintabilityValidator().ValidateGeometry(
            options,
            graphics,
            89.0f,
            41.0f,
            safeEdgeMm,
            safeEdgeMm,
            dpiX);
    }

    private void WriteGoldenGeometryDiagnostics(
        LabelPrintabilityResult result,
        LabelQrLayoutGeometry renderer,
        LabelPrintabilityGeometrySnapshot validator)
    {
        TestContext.WriteLine("Renderer / współdzielona geometria QR:");
        TestContext.WriteLine("pageWidth=89,000; pageHeight=41,000");
        TestContext.WriteLine("cellWidth=44,500; cellHeight=20,500");
        TestContext.WriteLine(
            $"layoutX0={renderer.LayoutX:0.000}; " +
            $"layoutY0={renderer.LayoutY:0.000}; " +
            $"layoutWidth={renderer.LayoutWidth:0.000}; " +
            $"layoutHeight={renderer.LayoutHeight:0.000}");
        TestContext.WriteLine(
            $"barY={renderer.CompanyBarRect.Y:0.000}; " +
            $"barHeight={renderer.CompanyBarRect.Height:0.000}; " +
            $"whiteAreaHeight={renderer.WhiteAreaHeight:0.000}");
        TestContext.WriteLine(
            $"qrMax={renderer.AvailableQrFootprintMm:0.000}; " +
            $"qrActual={result.QrFootprintMm:0.000}");
        TestContext.WriteLine($"titleRect={FormatRectangle(renderer.TitleRect)}");
        TestContext.WriteLine($"assetIdRect={FormatRectangle(renderer.AssetIdRect)}");
        TestContext.WriteLine(
            $"companyRect={FormatRectangle(renderer.CompanyBarRect)}");

        TestContext.WriteLine("Validator:");
        TestContext.WriteLine(
            $"layout={FormatRectangle(validator.LayoutRect)}; " +
            $"qrRect={FormatRectangle(validator.QrRect)}");
        TestContext.WriteLine(
            $"titleRect={FormatRectangle(validator.TitleRect)}; " +
            $"titleText={FormatRectangle(validator.TitleTextBounds)}");
        TestContext.WriteLine(
            $"assetIdRect={FormatRectangle(validator.AssetIdRect)}; " +
            $"assetIdText={FormatRectangle(validator.AssetIdTextBounds)}");
        TestContext.WriteLine(
            $"companyRect={FormatRectangle(validator.CompanyBarRect)}; " +
            $"companyText={FormatRectangle(validator.CompanyTextBounds)}");
        TestContext.WriteLine(
            $"fonts: title={result.TitleFontSizePt:0.00} pt; " +
            $"asset={result.AssetIdFontSizePt:0.00} pt; " +
            $"company={result.CompanyFontSizePt:0.00} pt");
    }

    private static string FormatRectangle(RectangleF rectangle)
    {
        return $"[{rectangle.X:0.000}, {rectangle.Y:0.000}, " +
            $"{rectangle.Width:0.000}, {rectangle.Height:0.000}]";
    }

    private static void AssertRectanglesEqual(
        RectangleF expected,
        RectangleF actual)
    {
        Assert.AreEqual(expected.X, actual.X, 0.0001f);
        Assert.AreEqual(expected.Y, actual.Y, 0.0001f);
        Assert.AreEqual(expected.Width, actual.Width, 0.0001f);
        Assert.AreEqual(expected.Height, actual.Height, 0.0001f);
    }

    private static string FormatIssues(LabelPrintabilityResult result)
    {
        return string.Join(
            Environment.NewLine,
            result.Issues.Select(issue =>
                $"{issue.Severity} {issue.Code}: {issue.Message}"));
    }
}
