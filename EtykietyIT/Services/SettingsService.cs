using EtykietyIT.Models;
using EtykietyIT.Persistence;

namespace EtykietyIT.Services;

public sealed class SettingsService
{
    private readonly JsonFileStore _jsonFileStore;
    private readonly string _settingsFilePath;

    public SettingsService(
        JsonFileStore jsonFileStore,
        string settingsFilePath)
    {
        _jsonFileStore = jsonFileStore ??
            throw new ArgumentNullException(nameof(jsonFileStore));
        ArgumentException.ThrowIfNullOrWhiteSpace(settingsFilePath);
        _settingsFilePath = Path.GetFullPath(settingsFilePath);
    }

    public async Task<ApplicationSettings> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_settingsFilePath))
        {
            var defaultSettings = new ApplicationSettings();
            await SaveAsync(defaultSettings, cancellationToken);
            return defaultSettings;
        }

        ApplicationSettings settings =
            await _jsonFileStore.LoadAsync<ApplicationSettings>(
                _settingsFilePath,
                cancellationToken) ?? throw new InvalidDataException(
                    "Plik settings.json nie zawiera ustawień aplikacji.");

        settings.Validate();
        return settings;
    }

    public Task SaveAsync(
        ApplicationSettings settings,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings.Validate();

        return _jsonFileStore.SaveAsync(
            _settingsFilePath,
            settings,
            value => value.Validate(),
            cancellationToken);
    }
}
