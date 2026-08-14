using EtykietyIT.Export;
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
        OrganizationProfileService organizationProfileService,
        PrinterCalibrationService printerCalibrationService,
        LabelProfileService labelProfileService,
        PrintHistoryService printHistoryService,
        IHistoryExporter historyExporter)
    {
        ApplicationMode = applicationMode;
        DataPaths = dataPaths ?? throw new ArgumentNullException(nameof(dataPaths));
        JsonFileStore = jsonFileStore ?? throw new ArgumentNullException(nameof(jsonFileStore));
        SettingsService = settingsService ??
            throw new ArgumentNullException(nameof(settingsService));
        OrganizationProfileService = organizationProfileService ??
            throw new ArgumentNullException(nameof(organizationProfileService));
        PrinterCalibrationService = printerCalibrationService ??
            throw new ArgumentNullException(nameof(printerCalibrationService));
        LabelProfileService = labelProfileService ??
            throw new ArgumentNullException(nameof(labelProfileService));
        PrintHistoryService = printHistoryService ??
            throw new ArgumentNullException(nameof(printHistoryService));
        HistoryExporter = historyExporter ??
            throw new ArgumentNullException(nameof(historyExporter));
    }

    public ApplicationMode ApplicationMode { get; }

    public AppDataPaths DataPaths { get; }

    public JsonFileStore JsonFileStore { get; }

    public SettingsService SettingsService { get; }

    public OrganizationProfileService OrganizationProfileService { get; }

    public PrinterCalibrationService PrinterCalibrationService { get; }

    public LabelProfileService LabelProfileService { get; }

    public PrintHistoryService PrintHistoryService { get; }

    public IHistoryExporter HistoryExporter { get; }
}
