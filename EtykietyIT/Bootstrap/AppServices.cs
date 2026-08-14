using EtykietyIT.Models;
using EtykietyIT.Persistence;
using EtykietyIT.Services;

namespace EtykietyIT.Bootstrap;

public sealed class AppServices
{
    public AppServices(
        ApplicationMode applicationMode,
        AppDataPaths dataPaths,
        JsonFileStore jsonFileStore,
        SettingsService settingsService,
        PrinterCalibrationService printerCalibrationService,
        LabelProfileService labelProfileService)
    {
        ApplicationMode = applicationMode;
        DataPaths = dataPaths ?? throw new ArgumentNullException(nameof(dataPaths));
        JsonFileStore = jsonFileStore ?? throw new ArgumentNullException(nameof(jsonFileStore));
        SettingsService = settingsService ??
            throw new ArgumentNullException(nameof(settingsService));
        PrinterCalibrationService = printerCalibrationService ??
            throw new ArgumentNullException(nameof(printerCalibrationService));
        LabelProfileService = labelProfileService ??
            throw new ArgumentNullException(nameof(labelProfileService));
    }

    public ApplicationMode ApplicationMode { get; }

    public AppDataPaths DataPaths { get; }

    public JsonFileStore JsonFileStore { get; }

    public SettingsService SettingsService { get; }

    public PrinterCalibrationService PrinterCalibrationService { get; }

    public LabelProfileService LabelProfileService { get; }
}
