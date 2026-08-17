using EtykietyIT.Models;
using EtykietyIT.Persistence;
using EtykietyIT.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Services;

[TestClass]
public sealed class OrganizationProfileServiceTests
{
    [TestMethod]
    public async Task CreateAndGetByIdAsync_CreatesOrganizationWithGeneratedId()
    {
        await WithServiceAsync(async (service, directoryPath) =>
        {
            OrganizationProfile created = await service.CreateAsync(
                CreateProfile("Organizacja A", 10));
            OrganizationProfile? loaded = await service.GetByIdAsync(created.Id);

            Assert.IsTrue(OrganizationProfile.IsValidId(created.Id));
            Assert.AreEqual(created, loaded);
            Assert.IsTrue(File.Exists(Path.Combine(
                directoryPath,
                $"{created.Id}.json")));
        });
    }

    [TestMethod]
    public async Task UpdateAsync_PersistsChangesWithoutChangingId()
    {
        await WithServiceAsync(async (service, _) =>
        {
            OrganizationProfile created = await service.CreateAsync(
                CreateProfile("Przed zmianą", 10));
            OrganizationProfile updated = created with
            {
                Name = "Po zmianie",
                CompanyName = "Nowa Firma S.A.",
                NextAssetNumber = 99
            };

            await service.UpdateAsync(updated);
            OrganizationProfile? loaded = await service.GetByIdAsync(created.Id);

            Assert.AreEqual(created.Id, loaded!.Id);
            Assert.AreEqual(updated, loaded);
        });
    }

    [TestMethod]
    public async Task UpdateAsync_RoundTripsDefaultQrEnabled()
    {
        await WithServiceAsync(async (service, _) =>
        {
            OrganizationProfile created = await service.CreateAsync(
                CreateProfile("Organizacja QR", 10));

            await service.UpdateAsync(created with { DefaultQrEnabled = true });
            OrganizationProfile loaded =
                (await service.GetByIdAsync(created.Id))!;

            Assert.IsTrue(loaded.DefaultQrEnabled);
        });
    }

    [TestMethod]
    public async Task GetAllAsync_OldJsonWithoutQrFieldDefaultsToFalse()
    {
        await WithServiceAsync(async (service, directoryPath) =>
        {
            string id = $"organization.{Guid.NewGuid():D}";
            string json = $$"""
                {
                  "schemaVersion": 1,
                  "id": "{{id}}",
                  "name": "Stara organizacja",
                  "companyName": "Stara firma",
                  "assetId": {
                    "prefix": "IT-",
                    "digits": 6
                  },
                  "nextAssetNumber": 1,
                  "defaultLabelProfileId": "builtin.89x41.2up",
                  "defaultPrinterName": null
                }
                """;
            await File.WriteAllTextAsync(
                Path.Combine(directoryPath, $"{id}.json"),
                json);

            OrganizationProfileReadResult result = await service.GetAllAsync();

            Assert.HasCount(1, result.Profiles);
            Assert.IsFalse(result.Profiles[0].DefaultQrEnabled);
        });
    }

    [TestMethod]
    public async Task DuplicateAsync_CreatesIndependentProfileWithNewId()
    {
        await WithServiceAsync(async (service, _) =>
        {
            OrganizationProfile source = await service.CreateAsync(
                CreateProfile("Oddział", 42));

            OrganizationProfile duplicate = await service.DuplicateAsync(source.Id);

            Assert.AreNotEqual(source.Id, duplicate.Id);
            Assert.AreEqual("Oddział — kopia", duplicate.Name);
            Assert.AreEqual(source.CompanyName, duplicate.CompanyName);
            Assert.AreEqual(source.NextAssetNumber, duplicate.NextAssetNumber);
            Assert.HasCount(2, (await service.GetAllAsync()).Profiles);
        });
    }

    [TestMethod]
    public async Task DeleteAsync_RemovesOrganizationWhenAnotherExists()
    {
        await WithServiceAsync(async (service, _) =>
        {
            OrganizationProfile first = await service.CreateAsync(
                CreateProfile("Pierwsza", 1));
            await service.CreateAsync(CreateProfile("Druga", 2));

            await service.DeleteAsync(first.Id);

            Assert.IsNull(await service.GetByIdAsync(first.Id));
            Assert.HasCount(1, (await service.GetAllAsync()).Profiles);
        });
    }

    [TestMethod]
    public async Task DeleteAsync_RejectsDeletingLastOrganization()
    {
        await WithServiceAsync(async (service, _) =>
        {
            OrganizationProfile profile = await service.CreateAsync(
                CreateProfile("Jedyna", 1));

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => service.DeleteAsync(profile.Id));

            Assert.IsNotNull(await service.GetByIdAsync(profile.Id));
        });
    }

    [TestMethod]
    public async Task CreateAndUpdateAsync_RejectDuplicateNamesIgnoringCase()
    {
        await WithServiceAsync(async (service, _) =>
        {
            OrganizationProfile first = await service.CreateAsync(
                CreateProfile("Magazyn", 1));
            OrganizationProfile second = await service.CreateAsync(
                CreateProfile("Oddział", 1));

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                service.CreateAsync(CreateProfile("MAGAZYN", 1)));
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                service.UpdateAsync(second with { Name = "magazyn" }));

            Assert.IsNotNull(await service.GetByIdAsync(first.Id));
        });
    }

    [TestMethod]
    public async Task GetAllAsync_SkipsCorruptedFileAndReturnsRemainingProfiles()
    {
        await WithServiceAsync(async (service, directoryPath) =>
        {
            OrganizationProfile valid = await service.CreateAsync(
                CreateProfile("Poprawna", 1));
            string corruptedPath = Path.Combine(
                directoryPath,
                $"organization.{Guid.NewGuid():D}.json");
            await File.WriteAllTextAsync(corruptedPath, "{ uszkodzony json");

            OrganizationProfileReadResult result = await service.GetAllAsync();

            Assert.HasCount(1, result.Profiles);
            Assert.AreEqual(valid, result.Profiles[0]);
            Assert.AreEqual(1, result.SkippedFileCount);
        });
    }

    [TestMethod]
    public async Task CreateAsync_StoresMultipleOrganizationsInSeparateFiles()
    {
        await WithServiceAsync(async (service, directoryPath) =>
        {
            OrganizationProfile first = await service.CreateAsync(
                CreateProfile("A", 11));
            OrganizationProfile second = await service.CreateAsync(
                CreateProfile("B", 22));

            string[] files = Directory.GetFiles(
                directoryPath,
                "organization.*.json");

            Assert.HasCount(2, files);
            Assert.IsTrue(files.Any(path =>
                Path.GetFileName(path) == $"{first.Id}.json"));
            Assert.IsTrue(files.Any(path =>
                Path.GetFileName(path) == $"{second.Id}.json"));
        });
    }

    [TestMethod]
    public async Task UpdateAsync_KeepsIndependentNextAssetNumbers()
    {
        await WithServiceAsync(async (service, _) =>
        {
            OrganizationProfile first = await service.CreateAsync(
                CreateProfile("A", 100));
            OrganizationProfile second = await service.CreateAsync(
                CreateProfile("B", 500));

            await service.UpdateAsync(first with { NextAssetNumber = 103 });

            Assert.AreEqual(
                103,
                (await service.GetByIdAsync(first.Id))!.NextAssetNumber);
            Assert.AreEqual(
                500,
                (await service.GetByIdAsync(second.Id))!.NextAssetNumber);
        });
    }

    private static OrganizationProfile CreateProfile(string name, int nextNumber)
    {
        return new OrganizationProfile
        {
            Name = name,
            CompanyName = $"{name} S.A.",
            AssetId = new AssetIdSettings
            {
                Prefix = "IT-",
                Digits = 6
            },
            NextAssetNumber = nextNumber,
            DefaultLabelProfileId = "builtin.89x41.2up",
            DefaultPrinterName = "DYMO LabelWriter 550"
        };
    }

    private static async Task WithServiceAsync(
        Func<OrganizationProfileService, string, Task> test)
    {
        string directoryPath = Path.Combine(
            Path.GetTempPath(),
            $"EtykietyIT.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(directoryPath);

        try
        {
            var service = new OrganizationProfileService(
                new JsonFileStore(),
                directoryPath);
            await test(service, directoryPath);
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }
}
