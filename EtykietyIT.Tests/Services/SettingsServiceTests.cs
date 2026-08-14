using EtykietyIT.Models;
using EtykietyIT.Persistence;
using EtykietyIT.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Services;

[TestClass]
public sealed class SettingsServiceTests
{
    [TestMethod]
    public async Task LoadAsync_CreatesNeutralOrganizationForFreshInstallation()
    {
        await WithServicesAsync(async (settingsService, organizationService, paths) =>
        {
            ApplicationSettings settings = await settingsService.LoadAsync();
            OrganizationProfile profile =
                (await organizationService.GetByIdAsync(
                    settings.ActiveOrganizationProfileId))!;

            Assert.AreEqual(2, settings.SchemaVersion);
            Assert.AreEqual("Domyślna organizacja", profile.Name);
            Assert.AreEqual("Moja firma", profile.CompanyName);
            Assert.AreEqual("IT-", profile.AssetId.Prefix);
            Assert.AreEqual(6, profile.AssetId.Digits);
            Assert.AreEqual(1, profile.NextAssetNumber);
            Assert.AreEqual("builtin.89x41.2up", profile.DefaultLabelProfileId);
            Assert.IsNull(profile.DefaultPrinterName);
            Assert.IsTrue(File.Exists(paths.SettingsFilePath));
            Assert.HasCount(
                1,
                Directory.GetFiles(paths.OrganizationsDirectory, "organization.*.json"));
        });
    }

    [TestMethod]
    public async Task SaveAndLoadAsync_RoundTripsActiveOrganizationId()
    {
        await WithServicesAsync(async (settingsService, organizationService, _) =>
        {
            OrganizationProfile first = await organizationService.CreateAsync(
                new OrganizationProfile { Name = "Pierwsza" });
            OrganizationProfile second = await organizationService.CreateAsync(
                new OrganizationProfile { Name = "Druga" });
            var expected = new ApplicationSettings
            {
                ActiveOrganizationProfileId = second.Id
            };

            await settingsService.SaveAsync(expected);
            ApplicationSettings actual = await settingsService.LoadAsync();

            Assert.AreEqual(expected, actual);
            Assert.AreNotEqual(first.Id, actual.ActiveOrganizationProfileId);
        });
    }

    private static async Task WithServicesAsync(
        Func<SettingsService, OrganizationProfileService, TestPaths, Task> test)
    {
        string rootDirectory = CreateTemporaryDirectory();
        var paths = new TestPaths(rootDirectory);

        try
        {
            var store = new JsonFileStore();
            var organizationService = new OrganizationProfileService(
                store,
                paths.OrganizationsDirectory);
            var settingsService = new SettingsService(
                store,
                paths.SettingsFilePath,
                paths.BackupFilePath,
                organizationService);
            await test(settingsService, organizationService, paths);
        }
        finally
        {
            Directory.Delete(rootDirectory, true);
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

    private sealed record TestPaths(string RootDirectory)
    {
        public string SettingsFilePath { get; } =
            Path.Combine(RootDirectory, "settings.json");

        public string BackupFilePath { get; } =
            Path.Combine(RootDirectory, "settings.v1.backup.json");

        public string OrganizationsDirectory { get; } =
            Path.Combine(RootDirectory, "organizations");
    }
}
