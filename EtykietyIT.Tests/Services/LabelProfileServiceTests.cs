using EtykietyIT.Models;
using EtykietyIT.Persistence;
using EtykietyIT.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Services;

[TestClass]
public sealed class LabelProfileServiceTests
{
    [TestMethod]
    public async Task GetAllAsync_LoadsBuiltInProfilesFromApplicationResources()
    {
        string userDirectory = CreateTemporaryDirectory();

        try
        {
            string builtInDirectory = Path.Combine(
                AppContext.BaseDirectory,
                "Resources",
                "Profiles");
            var service = new LabelProfileService(
                new JsonFileStore(),
                builtInDirectory,
                userDirectory);

            IReadOnlyList<LabelProfile> profiles = await service.GetAllAsync();

            Assert.IsTrue(profiles.Any(profile =>
                profile.Id == "builtin.89x41.2up" &&
                profile.WidthMm == 89.0 &&
                profile.HeightMm == 41.0 &&
                profile.Columns == 2 &&
                profile.Rows == 1 &&
                profile.DrawCutLines));
            Assert.IsTrue(profiles.Any(profile =>
                profile.Id == "builtin.89x41.1up" &&
                profile.Columns == 1 &&
                !profile.DrawCutLines));
        }
        finally
        {
            Directory.Delete(userDirectory, true);
        }
    }

    [TestMethod]
    public async Task CreateAndGetByIdAsync_RoundTripsOneUserProfilePerFile()
    {
        await WithServiceAsync(async (service, _, userDirectory) =>
        {
            LabelProfile created = await service.CreateUserProfileAsync(
                CreateProfile("ignored", "Profil użytkownika"));
            LabelProfile? loaded = await service.GetByIdAsync(created.Id);

            Assert.StartsWith("user.", created.Id);
            Assert.AreEqual(created, loaded);
            string[] files = Directory.GetFiles(userDirectory, "user.*.json");
            Assert.HasCount(1, files);
            Assert.AreEqual($"{created.Id}.json", Path.GetFileName(files[0]));
        });
    }

    [TestMethod]
    public async Task UpdateUserProfileAsync_PersistsEditedProfile()
    {
        await WithServiceAsync(async (service, _, _) =>
        {
            LabelProfile created = await service.CreateUserProfileAsync(
                CreateProfile("ignored", "Przed zmianą"));
            LabelProfile edited = created with
            {
                Name = "Po zmianie",
                WidthMm = 100.0,
                Columns = 1,
                DrawCutLines = false
            };

            await service.UpdateUserProfileAsync(edited);
            LabelProfile? loaded = await service.GetByIdAsync(created.Id);

            Assert.AreEqual(edited, loaded);
        });
    }

    [TestMethod]
    public async Task DeleteUserProfileAsync_RemovesProfileAndFile()
    {
        await WithServiceAsync(async (service, _, userDirectory) =>
        {
            LabelProfile created = await service.CreateUserProfileAsync(
                CreateProfile("ignored", "Do usunięcia"));

            await service.DeleteUserProfileAsync(created.Id);

            Assert.IsNull(await service.GetByIdAsync(created.Id));
            Assert.IsEmpty(Directory.GetFiles(userDirectory, "user.*.json"));
        });
    }

    [TestMethod]
    public async Task CloneToUserProfileAsync_ClonesBuiltInProfileWithNewIdentity()
    {
        await WithServiceAsync(async (service, _, _) =>
        {
            LabelProfile source = (await service.GetByIdAsync(
                LabelProfileService.DefaultBuiltInProfileId))!;

            LabelProfile clone = await service.CloneToUserProfileAsync(source.Id);

            Assert.StartsWith("user.", clone.Id);
            Assert.AreNotEqual(source.Id, clone.Id);
            Assert.AreEqual($"{source.Name} — kopia", clone.Name);
            Assert.AreEqual(source.WidthMm, clone.WidthMm);
            Assert.AreEqual(source.HeightMm, clone.HeightMm);
            Assert.AreEqual(source.Columns, clone.Columns);
            Assert.AreEqual(source.Rows, clone.Rows);
            Assert.AreEqual(source.DrawCutLines, clone.DrawCutLines);
        });
    }

    [TestMethod]
    public async Task UpdateAndDelete_RejectBuiltInProfile()
    {
        await WithServiceAsync(async (service, _, _) =>
        {
            LabelProfile builtIn = (await service.GetByIdAsync(
                LabelProfileService.DefaultBuiltInProfileId))!;

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => service.UpdateUserProfileAsync(builtIn));
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => service.DeleteUserProfileAsync(builtIn.Id));
        });
    }

    [TestMethod]
    public async Task GetProfileOrDefaultAsync_ReturnsDefaultBuiltIn_WhenRequestedIdIsMissing()
    {
        await WithServiceAsync(async (service, _, _) =>
        {
            LabelProfile profile = await service.GetProfileOrDefaultAsync(
                "user.00000000-0000-0000-0000-000000000000");

            Assert.AreEqual(
                LabelProfileService.DefaultBuiltInProfileId,
                profile.Id);
        });
    }

    [TestMethod]
    public async Task GetAllAsync_RejectsCorruptedUserProfileFile()
    {
        await WithServiceAsync(async (service, _, userDirectory) =>
        {
            string filePath = Path.Combine(
                userDirectory,
                $"user.{Guid.NewGuid():D}.json");
            await File.WriteAllTextAsync(filePath, "{ niepoprawny json");

            InvalidDataException exception = await Assert.ThrowsExactlyAsync<InvalidDataException>(
                () => service.GetAllAsync());

            StringAssert.Contains(exception.Message, filePath);
        });
    }

    private static async Task WithServiceAsync(
        Func<LabelProfileService, string, string, Task> test)
    {
        string rootDirectory = CreateTemporaryDirectory();
        string builtInDirectory = Path.Combine(rootDirectory, "built-in");
        string userDirectory = Path.Combine(rootDirectory, "profiles");
        Directory.CreateDirectory(builtInDirectory);
        Directory.CreateDirectory(userDirectory);

        try
        {
            var store = new JsonFileStore();
            LabelProfile builtIn = CreateProfile(
                LabelProfileService.DefaultBuiltInProfileId,
                "Profil wbudowany");
            await store.SaveAsync(
                Path.Combine(builtInDirectory, $"{builtIn.Id}.json"),
                builtIn,
                value => value.Validate());

            var service = new LabelProfileService(
                store,
                builtInDirectory,
                userDirectory);
            await test(service, builtInDirectory, userDirectory);
        }
        finally
        {
            Directory.Delete(rootDirectory, true);
        }
    }

    private static LabelProfile CreateProfile(string id, string name)
    {
        return new LabelProfile
        {
            Id = id,
            Name = name,
            WidthMm = 89.0,
            HeightMm = 41.0,
            Columns = 2,
            Rows = 1,
            DrawCutLines = true
        };
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
