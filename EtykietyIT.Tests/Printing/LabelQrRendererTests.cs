using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using EtykietyIT.Printing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Net.Codecrete.QrCodeGenerator;

namespace EtykietyIT.Tests.Printing;

[TestClass]
public sealed class LabelQrRendererTests
{
    [TestMethod]
    public void CreateQrCode_EncodesExactAssetIdWithoutAdditionalCharacters()
    {
        const string assetId = "IT-000123";
        QrCode actual = LabelQrRenderer.CreateQrCode(assetId);
        QrCode expected = QrCode.EncodeText(assetId, QrCode.Ecc.Medium);
        QrCode withNewLine = QrCode.EncodeText(
            $"{assetId}\r\n",
            QrCode.Ecc.Medium);

        AssertQrMatricesEqual(expected, actual);
        AssertQrMatricesNotEqual(actual, withNewLine);
    }

    [TestMethod]
    public void CreateQrCode_PreservesLeadingZeros()
    {
        QrCode withLeadingZeros = LabelQrRenderer.CreateQrCode("IT-000123");
        QrCode withoutLeadingZeros = LabelQrRenderer.CreateQrCode("IT-123");

        AssertQrMatricesNotEqual(withLeadingZeros, withoutLeadingZeros);
    }

    [TestMethod]
    public void CalculateLayout_UsesFourModuleQuietZoneForVersionOne()
    {
        QrCode qrCode = LabelQrRenderer.CreateQrCode("IT-000123");
        LabelQrLayout layout = LabelQrRenderer.CalculateLayout(
            qrCode.Size,
            300.0f,
            LabelRenderMode.Print,
            15.0f);

        Assert.AreEqual(21, qrCode.Size);
        Assert.AreEqual(29, layout.TotalModules);
        Assert.AreEqual(
            4,
            (layout.TotalModules - layout.QrModules) / 2);
    }

    [TestMethod]
    public void CalculateLayout_UsesSixWholeDotsAt300Dpi()
    {
        LabelQrLayout layout = LabelQrRenderer.CalculateLayout(
            21,
            300.0f,
            LabelRenderMode.Print,
            15.0f);

        Assert.AreEqual(6, layout.DotsPerModule);
        Assert.AreEqual(6.0 * 25.4 / 300.0, layout.ModuleSizeMm, 0.0001);
        Assert.AreEqual(29.0 * 6.0 * 25.4 / 300.0, layout.ActualFootprintMm, 0.0001);
        Assert.AreEqual(14.732, layout.ActualFootprintMm, 0.001);
        Assert.IsLessThanOrEqualTo(
            LabelQrRenderer.PreferredQrFootprintMm,
            layout.ActualFootprintMm);
    }

    [TestMethod]
    [DataRow(15.0f, 6, 14.732f)]
    [DataRow(13.0f, 5, 12.2767f)]
    [DataRow(11.0f, 4, 9.8213f)]
    public void CalculateLayout_SelectsLargestWholeModuleThatFits(
        float maxFootprintMm,
        int expectedDotsPerModule,
        float expectedFootprintMm)
    {
        LabelQrLayout layout = LabelQrRenderer.CalculateLayout(
            21,
            300.0f,
            LabelRenderMode.Print,
            maxFootprintMm);
        Assert.AreEqual(expectedDotsPerModule, layout.DotsPerModule);
        Assert.AreEqual(
            expectedFootprintMm,
            layout.ActualFootprintMm,
            0.001f);
        Assert.IsLessThanOrEqualTo(
            maxFootprintMm,
            layout.ActualFootprintMm);
    }

    [TestMethod]
    public void CalculateLayout_RejectsNineMillimetersAt300Dpi()
    {
        InvalidOperationException exception = Assert.ThrowsExactly<
            InvalidOperationException>(() =>
                LabelQrRenderer.CalculateLayout(
                    21,
                    300.0f,
                    LabelRenderMode.Print,
                    9.0f));

        StringAssert.Contains(exception.Message, "Dostępne miejsce: 9,0 mm");
        StringAssert.Contains(
            exception.Message,
            "Minimalny wymagany rozmiar przy 300 DPI: 9,8 mm");
        StringAssert.Contains(exception.Message, "4 punkty na moduł");
    }

    [TestMethod]
    [DoNotParallelize]
    public void CalculateLayout_UsesPolishFormattingUnderEnglishCulture()
    {
        CultureInfo previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            InvalidOperationException exception = Assert.ThrowsExactly<
                InvalidOperationException>(() =>
                    LabelQrRenderer.CalculateLayout(
                        21,
                        300.0f,
                        LabelRenderMode.Print,
                        9.0f));

            StringAssert.Contains(
                exception.Message,
                "Dostępne miejsce: 9,0 mm");
            StringAssert.Contains(
                exception.Message,
                "Minimalny wymagany rozmiar przy 300 DPI: 9,8 mm");
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [TestMethod]
    public void TwoByTwoProfile_UsesSmallerQrInsteadOfRequiringFifteenMillimeters()
    {
        float maxFootprintMm = LabelPrintJob.CalculateAvailableQrFootprint(
            89.0f,
            41.0f,
            2,
            2,
            1.0f,
            1.0f,
            0);
        LabelQrLayout layout = LabelQrRenderer.CalculateLayout(
            21,
            300.0f,
            LabelRenderMode.Print,
            maxFootprintMm);
        LabelQrLayout previewLayout = LabelQrRenderer.CalculateLayout(
            21,
            96.0f,
            LabelRenderMode.Preview,
            maxFootprintMm);

        Assert.IsLessThan(
            LabelQrRenderer.PreferredQrFootprintMm,
            maxFootprintMm);
        Assert.AreEqual(5, layout.DotsPerModule);
        Assert.AreEqual(
            maxFootprintMm,
            previewLayout.ActualFootprintMm,
            0.0001f);
        Assert.IsGreaterThanOrEqualTo(
            LabelQrRenderer.MinimumDotsPerModule,
            layout.DotsPerModule.GetValueOrDefault());
    }

    [TestMethod]
    public void CreateQrCode_HandlesLargerMatrixDynamically()
    {
        QrCode qrCode = LabelQrRenderer.CreateQrCode(new string('A', 100));
        LabelQrLayout layout = LabelQrRenderer.CalculateLayout(
            qrCode.Size,
            600.0f,
            LabelRenderMode.Print,
            15.0f);

        Assert.IsGreaterThan(21, qrCode.Size);
        Assert.AreEqual(
            qrCode.Size + LabelQrRenderer.QuietZoneModules * 2,
            layout.TotalModules);
        Assert.IsGreaterThanOrEqualTo(
            LabelQrRenderer.MinimumDotsPerModule,
            layout.DotsPerModule!.Value);
    }

    [TestMethod]
    public void CalculateLayout_RejectsPhysicalPrintBelowFourDotsPerModule()
    {
        InvalidOperationException exception = Assert.ThrowsExactly<
            InvalidOperationException>(() =>
                LabelQrRenderer.CalculateLayout(
                    21,
                    96.0f,
                    LabelRenderMode.Print,
                    15.0f));

        StringAssert.Contains(exception.Message, "96 DPI");
        StringAssert.Contains(exception.Message, "możliwe: 1 dots/module");
        StringAssert.Contains(exception.Message, "4 punkty na moduł");
    }

    [TestMethod]
    public void CalculateLayout_DoesNotRejectPreviewBecauseOfScreenDpi()
    {
        LabelQrLayout layout = LabelQrRenderer.CalculateLayout(
            21,
            96.0f,
            LabelRenderMode.Preview,
            15.0f);

        Assert.IsNull(layout.DotsPerModule);
        Assert.AreEqual(15.0, layout.ActualFootprintMm, 0.0001);
        Assert.AreEqual(15.0 / 29.0, layout.ModuleSizeMm, 0.0001);
    }

    [TestMethod]
    public void Draw_RendersQuietZoneAndRestoresGraphicsState()
    {
        using var bitmap = new Bitmap(600, 600);
        bitmap.SetResolution(300.0f, 300.0f);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.PageUnit = GraphicsUnit.Millimeter;
        graphics.Clear(Color.Magenta);
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        SmoothingMode originalSmoothingMode = graphics.SmoothingMode;

        var zone = new RectangleF(10.0f, 10.0f, 15.0f, 15.0f);
        LabelQrRenderer.Draw(
            graphics,
            "IT-000123",
            zone,
            LabelRenderMode.Preview);

        float moduleSize = 15.0f / 29.0f;
        Color quietZoneColor = bitmap.GetPixel(
            MillimetersToPixels(10.0f + moduleSize, 300.0f),
            MillimetersToPixels(10.0f + moduleSize, 300.0f));
        Color firstDarkModuleColor = bitmap.GetPixel(
            MillimetersToPixels(10.0f + 4.5f * moduleSize, 300.0f),
            MillimetersToPixels(10.0f + 4.5f * moduleSize, 300.0f));

        Assert.AreEqual(Color.White.ToArgb(), quietZoneColor.ToArgb());
        Assert.AreEqual(Color.Black.ToArgb(), firstDarkModuleColor.ToArgb());
        Assert.AreEqual(originalSmoothingMode, graphics.SmoothingMode);
    }

    private static int MillimetersToPixels(float millimeters, float dpi)
    {
        return (int)MathF.Round(millimeters * dpi / 25.4f);
    }

    private static void AssertQrMatricesEqual(QrCode expected, QrCode actual)
    {
        Assert.AreEqual(expected.Size, actual.Size);
        for (int y = 0; y < expected.Size; y++)
        {
            for (int x = 0; x < expected.Size; x++)
            {
                Assert.AreEqual(
                    expected.GetModule(x, y),
                    actual.GetModule(x, y),
                    $"Moduł ({x}, {y}) jest inny.");
            }
        }
    }

    private static void AssertQrMatricesNotEqual(QrCode first, QrCode second)
    {
        if (first.Size != second.Size)
        {
            return;
        }

        bool anyDifference = false;
        for (int y = 0; y < first.Size && !anyDifference; y++)
        {
            for (int x = 0; x < first.Size; x++)
            {
                if (first.GetModule(x, y) != second.GetModule(x, y))
                {
                    anyDifference = true;
                    break;
                }
            }
        }

        Assert.IsTrue(anyDifference, "Macierze QR powinny być różne.");
    }
}
