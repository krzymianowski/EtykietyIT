using EtykietyIT.Models;
using EtykietyIT.Persistence;

namespace EtykietyIT.Services;

public sealed class LabelProfileService
{
    public const string DefaultBuiltInProfileId = "builtin.89x41.2up";

    private const string BuiltInPrefix = "builtin.";
    private const string UserPrefix = "user.";

    private readonly JsonFileStore _jsonFileStore;
    private readonly string _builtInProfilesDirectory;
    private readonly string _userProfilesDirectory;

    public LabelProfileService(
        JsonFileStore jsonFileStore,
        string builtInProfilesDirectory,
        string userProfilesDirectory)
    {
        _jsonFileStore = jsonFileStore ??
            throw new ArgumentNullException(nameof(jsonFileStore));
        ArgumentException.ThrowIfNullOrWhiteSpace(builtInProfilesDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(userProfilesDirectory);

        _builtInProfilesDirectory = Path.GetFullPath(builtInProfilesDirectory);
        _userProfilesDirectory = Path.GetFullPath(userProfilesDirectory);
    }

    public async Task<IReadOnlyList<LabelProfile>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<LabelProfile> builtInProfiles = await LoadProfilesAsync(
            _builtInProfilesDirectory,
            $"{BuiltInPrefix}*.json",
            isBuiltIn: true,
            cancellationToken);
        IReadOnlyList<LabelProfile> userProfiles = await LoadProfilesAsync(
            _userProfilesDirectory,
            $"{UserPrefix}*.json",
            isBuiltIn: false,
            cancellationToken);

        var duplicateId = builtInProfiles
            .Concat(userProfiles)
            .GroupBy(profile => profile.Id, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateId is not null)
        {
            throw new InvalidDataException(
                $"Identyfikator profilu występuje więcej niż raz: {duplicateId.Key}.");
        }

        return builtInProfiles
            .OrderBy(profile => profile.Name, StringComparer.CurrentCultureIgnoreCase)
            .Concat(userProfiles.OrderBy(
                profile => profile.Name,
                StringComparer.CurrentCultureIgnoreCase))
            .ToArray();
    }

    public async Task<LabelProfile?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        IReadOnlyList<LabelProfile> profiles = await GetAllAsync(cancellationToken);
        return profiles.FirstOrDefault(profile => string.Equals(
            profile.Id,
            id,
            StringComparison.OrdinalIgnoreCase));
    }

    public async Task<LabelProfile> GetProfileOrDefaultAsync(
        string? id,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<LabelProfile> profiles = await GetAllAsync(cancellationToken);
        LabelProfile? requestedProfile = string.IsNullOrWhiteSpace(id)
            ? null
            : profiles.FirstOrDefault(profile => string.Equals(
                profile.Id,
                id,
                StringComparison.OrdinalIgnoreCase));

        if (requestedProfile is not null)
        {
            return requestedProfile;
        }

        return profiles.FirstOrDefault(profile => string.Equals(
            profile.Id,
            DefaultBuiltInProfileId,
            StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidDataException(
                $"Brak wymaganego profilu wbudowanego {DefaultBuiltInProfileId}.");
    }

    public Task<LabelProfile> CreateUserProfileAsync(
        LabelProfile profile,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(profile);

        LabelProfile userProfile = profile with
        {
            SchemaVersion = LabelProfile.CurrentSchemaVersion,
            Id = $"{UserPrefix}{Guid.NewGuid():D}"
        };

        return SaveNewUserProfileAsync(userProfile, cancellationToken);
    }

    public async Task UpdateUserProfileAsync(
        LabelProfile profile,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(profile);
        EnsureUserProfileId(profile.Id);
        profile.Validate();

        string filePath = GetUserProfilePath(profile.Id);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Nie znaleziono profilu użytkownika {profile.Id}.",
                filePath);
        }

        await _jsonFileStore.SaveAsync(
            filePath,
            profile,
            ValidateUserProfile,
            cancellationToken);
    }

    public Task DeleteUserProfileAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureUserProfileId(id);

        string filePath = GetUserProfilePath(id);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Nie znaleziono profilu użytkownika {id}.",
                filePath);
        }

        File.Delete(filePath);
        return Task.CompletedTask;
    }

    public async Task<LabelProfile> CloneToUserProfileAsync(
        string sourceId,
        CancellationToken cancellationToken = default)
    {
        LabelProfile sourceProfile = await GetByIdAsync(
            sourceId,
            cancellationToken) ?? throw new KeyNotFoundException(
                $"Nie znaleziono profilu {sourceId}.");

        return await CreateUserProfileAsync(
            sourceProfile with { Name = $"{sourceProfile.Name} — kopia" },
            cancellationToken);
    }

    public static bool IsBuiltInProfile(LabelProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        return profile.Id.StartsWith(BuiltInPrefix, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<LabelProfile> SaveNewUserProfileAsync(
        LabelProfile profile,
        CancellationToken cancellationToken)
    {
        ValidateUserProfile(profile);
        string filePath = GetUserProfilePath(profile.Id);

        if (File.Exists(filePath))
        {
            throw new IOException($"Profil {profile.Id} już istnieje.");
        }

        await _jsonFileStore.SaveAsync(
            filePath,
            profile,
            ValidateUserProfile,
            cancellationToken);
        return profile;
    }

    private async Task<IReadOnlyList<LabelProfile>> LoadProfilesAsync(
        string directoryPath,
        string searchPattern,
        bool isBuiltIn,
        CancellationToken cancellationToken)
    {
        if (!Directory.Exists(directoryPath))
        {
            if (isBuiltIn)
            {
                throw new DirectoryNotFoundException(
                    $"Nie znaleziono katalogu profili wbudowanych: {directoryPath}");
            }

            return Array.Empty<LabelProfile>();
        }

        var profiles = new List<LabelProfile>();
        foreach (string filePath in Directory.EnumerateFiles(
            directoryPath,
            searchPattern,
            SearchOption.TopDirectoryOnly))
        {
            cancellationToken.ThrowIfCancellationRequested();
            profiles.Add(await LoadProfileAsync(
                filePath,
                isBuiltIn,
                cancellationToken));
        }

        return profiles;
    }

    private async Task<LabelProfile> LoadProfileAsync(
        string filePath,
        bool isBuiltIn,
        CancellationToken cancellationToken)
    {
        try
        {
            LabelProfile profile = await _jsonFileStore.LoadAsync<LabelProfile>(
                filePath,
                cancellationToken) ?? throw new InvalidDataException(
                    "Plik profilu nie zawiera dokumentu JSON.");
            profile.Validate();

            if (isBuiltIn)
            {
                if (!profile.Id.StartsWith(
                    BuiltInPrefix,
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException(
                        "Profil wbudowany musi mieć identyfikator builtin.*.");
                }
            }
            else
            {
                ValidateUserProfile(profile);
            }

            string expectedFileName = $"{profile.Id}.json";
            if (!string.Equals(
                Path.GetFileName(filePath),
                expectedFileName,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException(
                    $"Nazwa pliku profilu musi być zgodna z Id: {expectedFileName}.");
            }

            return profile;
        }
        catch (Exception exception) when (
            exception is InvalidOperationException or InvalidDataException or
            System.Text.Json.JsonException)
        {
            throw new InvalidDataException(
                $"Uszkodzony lub nieprawidłowy plik profilu: {filePath}",
                exception);
        }
    }

    private string GetUserProfilePath(string id)
    {
        EnsureUserProfileId(id);
        return Path.Combine(_userProfilesDirectory, $"{id}.json");
    }

    private static void ValidateUserProfile(LabelProfile profile)
    {
        profile.Validate();
        EnsureUserProfileId(profile.Id);
    }

    private static void EnsureUserProfileId(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        if (id.StartsWith(BuiltInPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Profili wbudowanych nie można edytować ani usuwać.");
        }

        if (!id.StartsWith(UserPrefix, StringComparison.OrdinalIgnoreCase) ||
            !Guid.TryParseExact(id[UserPrefix.Length..], "D", out _))
        {
            throw new InvalidOperationException(
                "Profil użytkownika musi mieć identyfikator user.<guid>.");
        }
    }
}
