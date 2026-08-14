using System.Text;
using EtykietyIT.Models;
using EtykietyIT.Persistence;
using EtykietyIT.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Services;

[TestClass]
public sealed class SettingsMigrationTests
{
    [TestMethod]
    public async Task LoadAsync_MigratesV1AndPreservesAllOrganizationValues()
    {
        await WithMigrationAsync(async context =>
        {
            const string legacyJson = """
                {
                  "schemaVersion": 1,
                  "companyName": "Dolnośląskie Młyny S.A.",
                  "assetId": {
                    "prefix": "DM-",
                    "digits": 8
                  },
                  "defaultPrinterName": "DYMO LabelWriter 550",
                  "defaultProfileId": "builtin.89x41.1up",
                  "nextAssetNumber": 24
                }
                """;
            await File.WriteAllTextAsync(
                context.SettingsFilePath,
                legacyJson,
                new UTF8Encoding(false));

            ApplicationSettings settings = await context.SettingsService.LoadAsync();
            OrganizationProfile profile =
                (await context.OrganizationService.GetByIdAsync(
                    settings.ActiveOrganizationProfileId))!;

            Assert.AreEqual(2, settings.SchemaVersion);
            Assert.AreEqual(profile.Id, settings.ActiveOrganizationProfileId);
            Assert.AreEqual("Dolnośląskie Młyny S.A.", profile.Name);
            Assert.AreEqual("Dolnośląskie Młyny S.A.", profile.CompanyName);
            Assert.AreEqual("DM-", profile.AssetId.Prefix);
            Assert.AreEqual(8, profile.AssetId.Digits);
            Assert.AreEqual(24, profile.NextAssetNumber);
            Assert.AreEqual("builtin.89x41.1up", profile.DefaultLabelProfileId);
            Assert.AreEqual("DYMO LabelWriter 550", profile.DefaultPrinterName);
            Assert.IsTrue(File.Exists(context.BackupFilePath));
            Assert.AreEqual(
                legacyJson,
                await File.ReadAllTextAsync(context.BackupFilePath));

            string migratedSettingsJson = await File.ReadAllTextAsync(
                context.SettingsFilePath);
            StringAssert.Contains(migratedSettingsJson, "\"schemaVersion\": 2");
            StringAssert.Contains(
                migratedSettingsJson,
                "\"activeOrganizationProfileId\"");
            Assert.DoesNotContain("companyName", migratedSettingsJson);
        });
    }

    [TestMethod]
    public async Task LoadAsync_DoesNotRepeatCompletedMigration()
    {
        await WithMigrationAsync(async context =>
        {
            await File.WriteAllTextAsync(
                context.SettingsFilePath,
                """
                {
                  "schemaVersion": 1,
                  "companyName": "Firma Testowa",
                  "assetId": { "prefix": "FT-", "digits": 5 },
                  "defaultPrinterName": null,
                  "defaultProfileId": "builtin.89x41.2up",
                  "nextAssetNumber": 77
                }
                """,
                new UTF8Encoding(false));

            ApplicationSettings firstLoad = await context.SettingsService.LoadAsync();
            string[] filesAfterMigration = Directory.GetFiles(
                context.OrganizationsDirectory,
                "organization.*.json");
            byte[] backupAfterMigration = await File.ReadAllBytesAsync(
                context.BackupFilePath);

            var secondSettingsService = new SettingsService(
                new JsonFileStore(),
                context.SettingsFilePath,
                context.BackupFilePath,
                context.OrganizationService);
            ApplicationSettings secondLoad = await secondSettingsService.LoadAsync();

            Assert.AreEqual(firstLoad, secondLoad);
            CollectionAssert.AreEqual(
                filesAfterMigration,
                Directory.GetFiles(
                    context.OrganizationsDirectory,
                    "organization.*.json"));
            CollectionAssert.AreEqual(
                backupAfterMigration,
                await File.ReadAllBytesAsync(context.BackupFilePath));
        });
    }

    [TestMethod]
    public async Task LoadAsync_RollsBackCreatedFilesWhenFinalSettingsWriteFails()
    {
        await WithMigrationAsync(async context =>
        {
            const string legacyJson = """
                {
                  "schemaVersion": 1,
                  "companyName": "Firma z blokadą",
                  "assetId": { "prefix": "FB-", "digits": 6 },
                  "defaultPrinterName": null,
                  "defaultProfileId": "builtin.89x41.2up",
                  "nextAssetNumber": 9
                }
                """;
            await File.WriteAllTextAsync(
                context.SettingsFilePath,
                legacyJson,
                new UTF8Encoding(false));

            await using (var lockStream = new FileStream(
                context.SettingsFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                await Assert.ThrowsExactlyAsync<IOException>(
                    () => context.SettingsService.LoadAsync());
            }

            Assert.AreEqual(
                legacyJson,
                await File.ReadAllTextAsync(context.SettingsFilePath));
            Assert.IsFalse(File.Exists(context.BackupFilePath));
            Assert.IsEmpty(Directory.Exists(context.OrganizationsDirectory)
                ? Directory.GetFiles(
                    context.OrganizationsDirectory,
                    "organization.*.json")
                : Array.Empty<string>());
        });
    }

    private static async Task WithMigrationAsync(
        Func<MigrationContext, Task> test)
    {
        string rootDirectory = Path.Combine(
            Path.GetTempPath(),
            $"EtykietyIT.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(rootDirectory);

        try
        {
            var context = new MigrationContext(rootDirectory);
            await test(context);
        }
        finally
        {
            Directory.Delete(rootDirectory, true);
        }
    }

    private sealed class MigrationContext
    {
        public MigrationContext(string rootDirectory)
        {
            SettingsFilePath = Path.Combine(rootDirectory, "settings.json");
            BackupFilePath = Path.Combine(rootDirectory, "settings.v1.backup.json");
            OrganizationsDirectory = Path.Combine(rootDirectory, "organizations");

            var store = new JsonFileStore();
            OrganizationService = new OrganizationProfileService(
                store,
                OrganizationsDirectory);
            SettingsService = new SettingsService(
                store,
                SettingsFilePath,
                BackupFilePath,
                OrganizationService);
        }

        public string SettingsFilePath { get; }

        public string BackupFilePath { get; }

        public string OrganizationsDirectory { get; }

        public OrganizationProfileService OrganizationService { get; }

        public SettingsService SettingsService { get; }
    }
}
