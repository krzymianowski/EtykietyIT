using EtykietyIT.Bootstrap;
using EtykietyIT.Models;

namespace EtykietyIT;

static class Program
{
    [STAThread]
    static void Main(string[] arguments)
    {
        ApplicationConfiguration.Initialize();

        try
        {
            var bootstrapper = new AppBootstrapper();
            AppServices services = bootstrapper.Build(arguments);
            ApplicationSettings settings = services.SettingsService
                .LoadAsync()
                .GetAwaiter()
                .GetResult();

            Application.Run(new Form1(
                services.SettingsService,
                services.PrinterCalibrationService,
                services.LabelProfileService,
                services.PrintHistoryService,
                settings));
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"Nie można uruchomić aplikacji.\r\n\r\n{exception.Message}",
                "Etykiety IT",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
