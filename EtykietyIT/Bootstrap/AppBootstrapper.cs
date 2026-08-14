using EtykietyIT.Models;
using EtykietyIT.Persistence;
using EtykietyIT.Services;

namespace EtykietyIT.Bootstrap;

public sealed class AppBootstrapper
{
    public AppServices Build(string[] arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        string executableDirectory = AppContext.BaseDirectory;
        string localApplicationDataDirectory = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        var modeDetector = new ApplicationModeDetector();
        ApplicationMode mode = modeDetector.Detect(arguments, executableDirectory);
        AppDataPaths dataPaths = AppDataPaths.Create(
            mode,
            executableDirectory,
            localApplicationDataDirectory);

        CreateDataDirectories(dataPaths);
        EnsureDataDirectoryIsWritable(dataPaths.RootDirectory, mode);

        var jsonFileStore = new JsonFileStore();
        var settingsService = new SettingsService(
            jsonFileStore,
            dataPaths.SettingsFilePath);
        var printerCalibrationService = new PrinterCalibrationService(
            jsonFileStore,
            dataPaths.PrinterCalibrationsFilePath);
        var labelProfileService = new LabelProfileService(
            jsonFileStore,
            Path.Combine(executableDirectory, "Resources", "Profiles"),
            dataPaths.ProfilesDirectory);

        return new AppServices(
            mode,
            dataPaths,
            jsonFileStore,
            settingsService,
            printerCalibrationService,
            labelProfileService);
    }

    private static void CreateDataDirectories(AppDataPaths dataPaths)
    {
        Directory.CreateDirectory(dataPaths.RootDirectory);
        Directory.CreateDirectory(dataPaths.ProfilesDirectory);
        Directory.CreateDirectory(dataPaths.HistoryDirectory);
    }

    private static void EnsureDataDirectoryIsWritable(
        string directoryPath,
        ApplicationMode mode)
    {
        string probePath = Path.Combine(
            directoryPath,
            $".write-test-{Guid.NewGuid():N}.tmp");

        try
        {
            using var stream = new FileStream(
                probePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);
            stream.WriteByte(0);
        }
        catch (Exception exception) when (
            exception is UnauthorizedAccessException or IOException)
        {
            throw new IOException(
                $"Katalog danych trybu {mode} nie jest zapisywalny: " +
                $"{directoryPath}",
                exception);
        }
        finally
        {
            if (File.Exists(probePath))
            {
                File.Delete(probePath);
            }
        }
    }
}
