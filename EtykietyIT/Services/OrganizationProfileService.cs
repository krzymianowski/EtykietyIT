using System.Text.Json;
using EtykietyIT.Models;
using EtykietyIT.Persistence;

namespace EtykietyIT.Services;

public sealed class OrganizationProfileService
{
    private readonly JsonFileStore _jsonFileStore;
    private readonly string _organizationsDirectory;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public OrganizationProfileService(
        JsonFileStore jsonFileStore,
        string organizationsDirectory)
    {
        _jsonFileStore = jsonFileStore ??
            throw new ArgumentNullException(nameof(jsonFileStore));
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationsDirectory);
        _organizationsDirectory = Path.GetFullPath(organizationsDirectory);
    }

    public async Task<OrganizationProfileReadResult> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_organizationsDirectory))
        {
            return new OrganizationProfileReadResult();
        }

        var profiles = new List<OrganizationProfile>();
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int skippedFileCount = 0;

        foreach (string filePath in Directory.EnumerateFiles(
            _organizationsDirectory,
            $"{OrganizationProfile.IdPrefix}*.json",
            SearchOption.TopDirectoryOnly).Order(StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();

            OrganizationProfile? profile = await TryLoadProfileAsync(
                filePath,
                cancellationToken);
            if (profile is null || !names.Add(profile.Name.Trim()))
            {
                skippedFileCount++;
                continue;
            }

            profiles.Add(profile);
        }

        return new OrganizationProfileReadResult
        {
            Profiles = profiles
                .OrderBy(
                    profile => profile.Name,
                    StringComparer.CurrentCultureIgnoreCase)
                .ToArray(),
            SkippedFileCount = skippedFileCount
        };
    }

    public async Task<OrganizationProfile?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        EnsureValidId(id);
        OrganizationProfileReadResult result = await GetAllAsync(
            cancellationToken);
        return result.Profiles.FirstOrDefault(profile => string.Equals(
            profile.Id,
            id,
            StringComparison.Ordinal));
    }

    public async Task<OrganizationProfile> CreateAsync(
        OrganizationProfile profile,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(profile);

        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            OrganizationProfileReadResult current = await GetAllAsync(
                cancellationToken);
            OrganizationProfile created = profile with
            {
                SchemaVersion = OrganizationProfile.CurrentSchemaVersion,
                Id = $"{OrganizationProfile.IdPrefix}{Guid.NewGuid():D}"
            };
            created.Validate();
            EnsureUniqueName(current.Profiles, created.Name, excludedId: null);

            await SaveNewAsync(created, cancellationToken);
            return created;
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task UpdateAsync(
        OrganizationProfile profile,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(profile);
        EnsureValidId(profile.Id);

        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            OrganizationProfile updated = profile;
            updated.Validate();
            string filePath = GetFilePath(updated.Id);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    $"Nie znaleziono profilu organizacji {updated.Id}.",
                    filePath);
            }

            OrganizationProfileReadResult current = await GetAllAsync(
                cancellationToken);
            EnsureUniqueName(current.Profiles, updated.Name, updated.Id);

            await _jsonFileStore.SaveAsync(
                filePath,
                updated,
                value => value.Validate(),
                cancellationToken);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        EnsureValidId(id);

        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            OrganizationProfileReadResult current = await GetAllAsync(
                cancellationToken);
            OrganizationProfile? profile = current.Profiles.FirstOrDefault(
                item => string.Equals(item.Id, id, StringComparison.Ordinal));
            if (profile is null)
            {
                throw new KeyNotFoundException(
                    $"Nie znaleziono profilu organizacji {id}.");
            }

            if (current.Profiles.Count <= 1)
            {
                throw new InvalidOperationException(
                    "Nie można usunąć ostatniego profilu organizacji.");
            }

            File.Delete(GetFilePath(id));
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task<OrganizationProfile> DuplicateAsync(
        string sourceId,
        CancellationToken cancellationToken = default)
    {
        EnsureValidId(sourceId);

        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            OrganizationProfileReadResult current = await GetAllAsync(
                cancellationToken);
            OrganizationProfile source = current.Profiles.FirstOrDefault(
                profile => string.Equals(
                    profile.Id,
                    sourceId,
                    StringComparison.Ordinal)) ?? throw new KeyNotFoundException(
                        $"Nie znaleziono profilu organizacji {sourceId}.");

            OrganizationProfile duplicate = source with
            {
                Id = $"{OrganizationProfile.IdPrefix}{Guid.NewGuid():D}",
                Name = CreateDuplicateName(source.Name, current.Profiles)
            };
            duplicate.Validate();
            await SaveNewAsync(duplicate, cancellationToken);
            return duplicate;
        }
        finally
        {
            _writeLock.Release();
        }
    }

    internal Task DeleteForMigrationRollbackAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureValidId(id);
        string filePath = GetFilePath(id);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    private async Task<OrganizationProfile?> TryLoadProfileAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            OrganizationProfile profile =
                await _jsonFileStore.LoadAsync<OrganizationProfile>(
                    filePath,
                    cancellationToken) ?? throw new InvalidDataException(
                        "Plik nie zawiera profilu organizacji.");
            profile.Validate();

            if (!string.Equals(
                Path.GetFileName(filePath),
                $"{profile.Id}.json",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException(
                    "Nazwa pliku organizacji nie odpowiada jej identyfikatorowi.");
            }

            return profile;
        }
        catch (Exception exception) when (
            exception is JsonException or InvalidDataException or
            InvalidOperationException or NotSupportedException)
        {
            return null;
        }
    }

    private async Task SaveNewAsync(
        OrganizationProfile profile,
        CancellationToken cancellationToken)
    {
        string filePath = GetFilePath(profile.Id);
        if (File.Exists(filePath))
        {
            throw new IOException(
                $"Profil organizacji {profile.Id} już istnieje.");
        }

        await _jsonFileStore.SaveAsync(
            filePath,
            profile,
            value => value.Validate(),
            cancellationToken);
    }

    private string GetFilePath(string id)
    {
        EnsureValidId(id);
        return Path.Combine(_organizationsDirectory, $"{id}.json");
    }

    private static void EnsureUniqueName(
        IEnumerable<OrganizationProfile> profiles,
        string name,
        string? excludedId)
    {
        bool duplicateExists = profiles.Any(profile =>
            !string.Equals(profile.Id, excludedId, StringComparison.Ordinal) &&
            string.Equals(
                profile.Name.Trim(),
                name.Trim(),
                StringComparison.OrdinalIgnoreCase));
        if (duplicateExists)
        {
            throw new InvalidOperationException(
                $"Profil organizacji o nazwie „{name}” już istnieje.");
        }
    }

    private static string CreateDuplicateName(
        string sourceName,
        IEnumerable<OrganizationProfile> profiles)
    {
        var existingNames = profiles
            .Select(profile => profile.Name.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        string baseName = $"{sourceName.Trim()} — kopia";
        string candidate = baseName;
        int suffix = 2;

        while (existingNames.Contains(candidate))
        {
            candidate = $"{baseName} ({suffix})";
            suffix++;
        }

        return candidate;
    }

    private static void EnsureValidId(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        if (!OrganizationProfile.IsValidId(id))
        {
            throw new InvalidOperationException(
                "Identyfikator organizacji musi mieć format organization.<guid>.");
        }
    }
}
