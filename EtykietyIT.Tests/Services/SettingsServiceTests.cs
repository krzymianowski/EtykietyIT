using EtykietyIT.Models;
using EtykietyIT.Persistence;
using EtykietyIT.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Services;

[TestClass]
public sealed class SettingsServiceTests
{
    [TestMethod]
    public async Task LoadAsync_CreatesDefaultSettings_WhenFileDoesNotExist()
    {
        string directoryPath = CreateTemporaryDirectory();
        string filePath = Path.Combine(directoryPath, "settings.json");

        try
        {
            var service = new SettingsService(new JsonFileStore(), filePath);

            ApplicationSettings settings = await service.LoadAsync();

            Assert.AreEqual(new ApplicationSettings(), settings);
            Assert.IsTrue(File.Exists(filePath));
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }

    [TestMethod]
    public async Task SaveAndLoadAsync_RoundTripsSettings()
    {
        string directoryPath = CreateTemporaryDirectory();
        string filePath = Path.Combine(directoryPath, "settings.json");

        try
        {
            var store = new JsonFileStore();
            var service = new SettingsService(store, filePath);
            var expected = new ApplicationSettings
            {
                CompanyName = "Przykładowa Firma S.A.",
                AssetId = new AssetIdSettings
                {
                    Prefix = "ASSET-",
                    Digits = 8
                },
                DefaultPrinterName = "DYMO LabelWriter 550",
                NextAssetNumber = 123
            };

            await service.SaveAsync(expected);
            var reloadedService = new SettingsService(new JsonFileStore(), filePath);
            ApplicationSettings actual = await reloadedService.LoadAsync();

            Assert.AreEqual(expected, actual);
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
