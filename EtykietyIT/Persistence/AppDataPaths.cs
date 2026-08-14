using EtykietyIT.Models;

namespace EtykietyIT.Persistence;

public sealed class AppDataPaths
{
    private AppDataPaths(string rootDirectory)
    {
        RootDirectory = rootDirectory;
        SettingsFilePath = Path.Combine(rootDirectory, "settings.json");
        SettingsV1BackupFilePath = Path.Combine(
            rootDirectory,
            "settings.v1.backup.json");
        PrinterCalibrationsFilePath = Path.Combine(
            rootDirectory,
            "printer-calibrations.json");
        ProfilesDirectory = Path.Combine(rootDirectory, "profiles");
        OrganizationsDirectory = Path.Combine(rootDirectory, "organizations");
        HistoryDirectory = Path.Combine(rootDirectory, "history");
        HistoryFilePath = Path.Combine(HistoryDirectory, "print-history.jsonl");
    }

    public string RootDirectory { get; }

    public string SettingsFilePath { get; }

    public string SettingsV1BackupFilePath { get; }

    public string PrinterCalibrationsFilePath { get; }

    public string ProfilesDirectory { get; }

    public string OrganizationsDirectory { get; }

    public string HistoryDirectory { get; }

    public string HistoryFilePath { get; }

    public static AppDataPaths Create(
        ApplicationMode mode,
        string executableDirectory,
        string localApplicationDataDirectory)
    {
        if (string.IsNullOrWhiteSpace(executableDirectory))
        {
            throw new ArgumentException(
                "Katalog aplikacji jest wymagany.",
                nameof(executableDirectory));
        }

        string rootDirectory = mode switch
        {
            ApplicationMode.Standard => CreateStandardRoot(localApplicationDataDirectory),
            ApplicationMode.Portable => Path.Combine(
                Path.GetFullPath(executableDirectory),
                "Data",
                "v3"),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        return new AppDataPaths(rootDirectory);
    }

    private static string CreateStandardRoot(string localApplicationDataDirectory)
    {
        if (string.IsNullOrWhiteSpace(localApplicationDataDirectory))
        {
            throw new ArgumentException(
                "Katalog LOCALAPPDATA jest wymagany w trybie Standard.",
                nameof(localApplicationDataDirectory));
        }

        return Path.Combine(
            Path.GetFullPath(localApplicationDataDirectory),
            "EtykietyIT",
            "v3");
    }
}
