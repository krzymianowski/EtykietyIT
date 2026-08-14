using EtykietyIT.Models;
using EtykietyIT.Persistence;

namespace EtykietyIT.Services;

public sealed class SettingsService
{
    private readonly JsonFileStore _jsonFileStore;
    private readonly string _settingsFilePath;
    private readonly string _settingsV1BackupFilePath;
    private readonly OrganizationProfileService _organizationProfileService;

    public SettingsService(
        JsonFileStore jsonFileStore,
        string settingsFilePath,
        string settingsV1BackupFilePath,
        OrganizationProfileService organizationProfileService)
    {
        _jsonFileStore = jsonFileStore ??
            throw new ArgumentNullException(nameof(jsonFileStore));
        ArgumentException.ThrowIfNullOrWhiteSpace(settingsFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(settingsV1BackupFilePath);
        _settingsFilePath = Path.GetFullPath(settingsFilePath);
        _settingsV1BackupFilePath = Path.GetFullPath(settingsV1BackupFilePath);
        _organizationProfileService = organizationProfileService ??
            throw new ArgumentNullException(nameof(organizationProfileService));
    }

    public async Task<ApplicationSettings> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_settingsFilePath))
        {
            return await InitializeFreshInstallationAsync(cancellationToken);
        }

        SettingsSchemaEnvelope envelope =
            await _jsonFileStore.LoadAsync<SettingsSchemaEnvelope>(
                _settingsFilePath,
                cancellationToken) ?? throw new InvalidDataException(
                    "Plik settings.json nie zawiera ustawień aplikacji.");

        return envelope.SchemaVersion switch
        {
            1 => await MigrateV1Async(cancellationToken),
            ApplicationSettings.CurrentSchemaVersion =>
                await LoadCurrentAsync(cancellationToken),
            _ => throw new InvalidDataException(
                $"Nieobsługiwana wersja settings.json: {envelope.SchemaVersion}.")
        };
    }

    public async Task SaveAsync(
        ApplicationSettings settings,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings.Validate();

        OrganizationProfile? activeProfile =
            await _organizationProfileService.GetByIdAsync(
                settings.ActiveOrganizationProfileId,
                cancellationToken);
        if (activeProfile is null)
        {
            throw new InvalidOperationException(
                "Aktywny profil organizacji nie istnieje.");
        }

        await _jsonFileStore.SaveAsync(
            _settingsFilePath,
            settings,
            value => value.Validate(),
            cancellationToken);
    }

    private async Task<ApplicationSettings> LoadCurrentAsync(
        CancellationToken cancellationToken)
    {
        ApplicationSettings settings =
            await _jsonFileStore.LoadAsync<ApplicationSettings>(
                _settingsFilePath,
                cancellationToken) ?? throw new InvalidDataException(
                    "Plik settings.json nie zawiera ustawień aplikacji.");
        settings.Validate();

        if (await _organizationProfileService.GetByIdAsync(
            settings.ActiveOrganizationProfileId,
            cancellationToken) is null)
        {
            throw new InvalidDataException(
                "Aktywny profil organizacji wskazany w settings.json nie istnieje.");
        }

        return settings;
    }

    private async Task<ApplicationSettings> InitializeFreshInstallationAsync(
        CancellationToken cancellationToken)
    {
        OrganizationProfileReadResult existingProfiles =
            await _organizationProfileService.GetAllAsync(cancellationToken);
        OrganizationProfile activeProfile;
        bool createdProfile = false;

        if (existingProfiles.Profiles.Count > 0)
        {
            activeProfile = existingProfiles.Profiles[0];
        }
        else
        {
            activeProfile = await _organizationProfileService.CreateAsync(
                new OrganizationProfile(),
                cancellationToken);
            createdProfile = true;
        }

        var settings = new ApplicationSettings
        {
            ActiveOrganizationProfileId = activeProfile.Id
        };

        try
        {
            await SaveAsync(settings, cancellationToken);
            return settings;
        }
        catch
        {
            if (createdProfile)
            {
                await _organizationProfileService.DeleteForMigrationRollbackAsync(
                    activeProfile.Id,
                    CancellationToken.None);
            }

            throw;
        }
    }

    private async Task<ApplicationSettings> MigrateV1Async(
        CancellationToken cancellationToken)
    {
        LegacyApplicationSettings legacySettings =
            await _jsonFileStore.LoadAsync<LegacyApplicationSettings>(
                _settingsFilePath,
                cancellationToken) ?? throw new InvalidDataException(
                    "Plik settings.json v1 nie zawiera ustawień aplikacji.");
        legacySettings.Validate();

        bool createdBackup = await CreateOrValidateBackupAsync(
            cancellationToken);
        OrganizationProfile? migrationProfile = null;
        bool createdProfile = false;

        try
        {
            OrganizationProfileReadResult existingProfiles =
                await _organizationProfileService.GetAllAsync(cancellationToken);
            migrationProfile = existingProfiles.Profiles.FirstOrDefault(
                profile => MatchesLegacySettings(profile, legacySettings));

            if (migrationProfile is null)
            {
                migrationProfile = await _organizationProfileService.CreateAsync(
                    CreateMigratedProfile(legacySettings),
                    cancellationToken);
                createdProfile = true;
            }

            var settings = new ApplicationSettings
            {
                ActiveOrganizationProfileId = migrationProfile.Id
            };
            await SaveAsync(settings, cancellationToken);
            return settings;
        }
        catch (Exception migrationException)
        {
            var cleanupExceptions = new List<Exception>();

            if (createdProfile && migrationProfile is not null)
            {
                try
                {
                    await _organizationProfileService.DeleteForMigrationRollbackAsync(
                        migrationProfile.Id,
                        CancellationToken.None);
                }
                catch (Exception cleanupException)
                {
                    cleanupExceptions.Add(cleanupException);
                }
            }

            if (createdBackup)
            {
                try
                {
                    File.Delete(_settingsV1BackupFilePath);
                }
                catch (Exception cleanupException)
                {
                    cleanupExceptions.Add(cleanupException);
                }
            }

            if (cleanupExceptions.Count > 0)
            {
                cleanupExceptions.Insert(0, migrationException);
                throw new InvalidOperationException(
                    "Migracja settings.json v1 nie powiodła się, a pełne " +
                    "wycofanie zmian również zakończyło się błędem.",
                    new AggregateException(cleanupExceptions));
            }

            throw;
        }
    }

    private async Task<bool> CreateOrValidateBackupAsync(
        CancellationToken cancellationToken)
    {
        byte[] settingsBytes = await File.ReadAllBytesAsync(
            _settingsFilePath,
            cancellationToken);

        if (File.Exists(_settingsV1BackupFilePath))
        {
            byte[] backupBytes = await File.ReadAllBytesAsync(
                _settingsV1BackupFilePath,
                cancellationToken);
            if (!settingsBytes.AsSpan().SequenceEqual(backupBytes))
            {
                throw new InvalidDataException(
                    "Istniejący settings.v1.backup.json nie odpowiada settings.json v1.");
            }

            return false;
        }

        string? directoryPath = Path.GetDirectoryName(
            _settingsV1BackupFilePath);
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new InvalidOperationException(
                "Nie można ustalić katalogu kopii settings.json v1.");
        }

        Directory.CreateDirectory(directoryPath);
        string temporaryPath = Path.Combine(
            directoryPath,
            $".{Path.GetFileName(_settingsV1BackupFilePath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            await File.WriteAllBytesAsync(
                temporaryPath,
                settingsBytes,
                cancellationToken);
            File.Move(temporaryPath, _settingsV1BackupFilePath);
            return true;
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private static OrganizationProfile CreateMigratedProfile(
        LegacyApplicationSettings settings)
    {
        return new OrganizationProfile
        {
            Name = settings.CompanyName,
            CompanyName = settings.CompanyName,
            AssetId = settings.AssetId!,
            NextAssetNumber = settings.NextAssetNumber,
            DefaultLabelProfileId = settings.DefaultProfileId,
            DefaultPrinterName = settings.DefaultPrinterName
        };
    }

    private static bool MatchesLegacySettings(
        OrganizationProfile profile,
        LegacyApplicationSettings settings)
    {
        return string.Equals(
                profile.Name,
                settings.CompanyName,
                StringComparison.Ordinal) &&
            string.Equals(
                profile.CompanyName,
                settings.CompanyName,
                StringComparison.Ordinal) &&
            string.Equals(
                profile.AssetId.Prefix,
                settings.AssetId!.Prefix,
                StringComparison.Ordinal) &&
            profile.AssetId.Digits == settings.AssetId.Digits &&
            profile.NextAssetNumber == settings.NextAssetNumber &&
            string.Equals(
                profile.DefaultLabelProfileId,
                settings.DefaultProfileId,
                StringComparison.Ordinal) &&
            string.Equals(
                profile.DefaultPrinterName,
                settings.DefaultPrinterName,
                StringComparison.OrdinalIgnoreCase);
    }

    private sealed record SettingsSchemaEnvelope
    {
        public int SchemaVersion { get; init; }
    }

    private sealed record LegacyApplicationSettings
    {
        public int SchemaVersion { get; init; }

        public string CompanyName { get; init; } = string.Empty;

        public AssetIdSettings? AssetId { get; init; }

        public string? DefaultPrinterName { get; init; }

        public string DefaultProfileId { get; init; } = string.Empty;

        public int NextAssetNumber { get; init; }

        public void Validate()
        {
            if (SchemaVersion != 1)
            {
                throw new InvalidOperationException(
                    $"Nieobsługiwana wersja ustawień legacy: {SchemaVersion}.");
            }

            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                throw new InvalidOperationException(
                    "Nazwa firmy w settings.json v1 nie może być pusta.");
            }

            if (AssetId is null)
            {
                throw new InvalidOperationException(
                    "Brak ustawień Asset ID w settings.json v1.");
            }

            AssetId.Validate();

            if (NextAssetNumber < 0)
            {
                throw new InvalidOperationException(
                    "Następny numer w settings.json v1 nie może być ujemny.");
            }

            if (string.IsNullOrWhiteSpace(DefaultProfileId))
            {
                throw new InvalidOperationException(
                    "Domyślny profil etykiety w settings.json v1 jest wymagany.");
            }
        }
    }
}
