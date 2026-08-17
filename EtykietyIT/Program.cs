using EtykietyIT.Bootstrap;
using EtykietyIT.Diagnostics;
using EtykietyIT.Forms;
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
            ApplicationIconProvider.Initialize();
            var bootstrapper = new AppBootstrapper();
            AppServices services = bootstrapper.Build(arguments);
            using var uiDiagnostics = UiDiagnosticsService.StartIfRequested(
                arguments,
                services.DataPaths.DiagnosticsDirectory);
            ApplicationSettings settings = services.SettingsService
                .LoadAsync()
                .GetAwaiter()
                .GetResult();

            Application.Run(new MainForm(
                services.SettingsService,
                services.OrganizationProfileService,
                services.PrinterCalibrationService,
                services.LabelProfileService,
                services.PrintHistoryService,
                services.ApplicationVersionService,
                services.CsvHistoryExporter,
                services.XlsxHistoryExporter,
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
        finally
        {
            ApplicationIconProvider.Shutdown();
        }
    }
}
