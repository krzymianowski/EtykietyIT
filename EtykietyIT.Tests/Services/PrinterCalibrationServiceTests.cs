using EtykietyIT.Models;
using EtykietyIT.Persistence;
using EtykietyIT.Printing;
using EtykietyIT.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Services;

[TestClass]
public sealed class PrinterCalibrationServiceTests
{
    [TestMethod]
    public async Task GetCalibrationAsync_ReturnsZeroOffsets_WhenEntryDoesNotExist()
    {
        string directoryPath = CreateTemporaryDirectory();
        string filePath = Path.Combine(directoryPath, "printer-calibrations.json");

        try
        {
            var service = new PrinterCalibrationService(new JsonFileStore(), filePath);

            PrinterCalibration calibration = await service.GetCalibrationAsync(
                "Nieznana drukarka");

            Assert.AreEqual(0.0, calibration.OffsetXmm);
            Assert.AreEqual(0.0, calibration.OffsetYmm);
            Assert.IsFalse(File.Exists(filePath));
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }

    [TestMethod]
    public async Task SaveAndLoadAsync_RoundTripsCalibrationCaseInsensitively()
    {
        string directoryPath = CreateTemporaryDirectory();
        string filePath = Path.Combine(directoryPath, "printer-calibrations.json");

        try
        {
            var service = new PrinterCalibrationService(new JsonFileStore(), filePath);
            var expected = new PrinterCalibration(-0.4, 0.0);

            await service.SaveCalibrationAsync("DYMO LabelWriter 550", expected);

            var reloadedService = new PrinterCalibrationService(
                new JsonFileStore(),
                filePath);
            PrinterCalibration actual = await reloadedService.GetCalibrationAsync(
                "dymo labelwriter 550");

            Assert.AreEqual(expected, actual);
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }

    [TestMethod]
    public async Task SaveCalibrationAsync_UpdatesExistingCaseInsensitiveEntry()
    {
        string directoryPath = CreateTemporaryDirectory();
        string filePath = Path.Combine(directoryPath, "printer-calibrations.json");

        try
        {
            var store = new JsonFileStore();
            var service = new PrinterCalibrationService(store, filePath);

            await service.SaveCalibrationAsync(
                "DYMO LabelWriter 550",
                new PrinterCalibration(-0.4, 0.0));
            await service.SaveCalibrationAsync(
                "dymo labelwriter 550",
                new PrinterCalibration(-0.2, 0.1));

            PrinterCalibrationDocument? document =
                await store.LoadAsync<PrinterCalibrationDocument>(filePath);

            Assert.IsNotNull(document);
            Assert.HasCount(1, document.Printers);
            Assert.AreEqual(-0.2, document.Printers[0].OffsetXmm);
            Assert.AreEqual(0.1, document.Printers[0].OffsetYmm);
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }

    private static string CreateTemporaryDirectory()
    {
        string directoryPath = Path.Combine(
            Path.GetTempPath(),
            $"EtykietyIT.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(directoryPath);
        return directoryPath;
    }
}
