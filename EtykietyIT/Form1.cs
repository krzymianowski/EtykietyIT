using System.Drawing.Printing;
using EtykietyIT.Forms;
using EtykietyIT.Models;
using EtykietyIT.Printing;
using EtykietyIT.Services;

namespace EtykietyIT;

public partial class Form1 : Form
{
    private const double LabelWidthMm = 89.0;
    private const double LabelHeightMm = 41.0;
    private const int LabelColumns = 2;
    private const int LabelRows = 1;
    private const bool DrawCutLines = true;

    private readonly SettingsService _settingsService;
    private readonly PrinterCalibrationService _printerCalibrationService;

    private ApplicationSettings _settings;

    public Form1(
        SettingsService settingsService,
        PrinterCalibrationService printerCalibrationService,
        ApplicationSettings settings)
    {
        _settingsService = settingsService ??
            throw new ArgumentNullException(nameof(settingsService));
        _printerCalibrationService = printerCalibrationService ??
            throw new ArgumentNullException(nameof(printerCalibrationService));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _settings.Validate();

        InitializeComponent();

        printerComboBox.SelectedIndexChanged += PrinterComboBox_SelectedIndexChanged;
        firstNumberNumericUpDown.ValueChanged += NumberInput_ValueChanged;
        quantityNumericUpDown.ValueChanged += NumberInput_ValueChanged;
        settingsButton.Click += SettingsButton_Click;
        saveCalibrationButton.Click += SaveCalibrationButton_Click;
        previewButton.Click += PreviewButton_Click;
        printButton.Click += PrintButton_Click;

        firstNumberNumericUpDown.Value = _settings.NextAssetNumber;
        LoadInstalledPrinters();
        UpdateAssetRange();
    }

    private void LoadInstalledPrinters()
    {
        printerComboBox.Items.Clear();
        foreach (string printerName in PrinterSettings.InstalledPrinters)
        {
            printerComboBox.Items.Add(printerName);
        }

        SelectPreferredPrinter(_settings.DefaultPrinterName);
        UpdatePrintButtons();
    }

    private void SelectPreferredPrinter(string? preferredPrinterName)
    {
        int preferredIndex = FindPrinterIndex(preferredPrinterName);
        if (preferredIndex >= 0)
        {
            printerComboBox.SelectedIndex = preferredIndex;
            return;
        }

        int dymoIndex = -1;
        for (int index = 0; index < printerComboBox.Items.Count; index++)
        {
            if (printerComboBox.Items[index] is string printerName &&
                printerName.Contains("DYMO", StringComparison.OrdinalIgnoreCase))
            {
                dymoIndex = index;
                break;
            }
        }

        if (printerComboBox.Items.Count > 0)
        {
            printerComboBox.SelectedIndex = dymoIndex >= 0 ? dymoIndex : 0;
        }
        else
        {
            SetCalibrationControls(new PrinterCalibration());
        }
    }

    private int FindPrinterIndex(string? printerName)
    {
        if (string.IsNullOrWhiteSpace(printerName))
        {
            return -1;
        }

        for (int index = 0; index < printerComboBox.Items.Count; index++)
        {
            if (printerComboBox.Items[index] is string installedPrinterName &&
                string.Equals(
                    installedPrinterName,
                    printerName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        return -1;
    }

    private async void PrinterComboBox_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        UpdatePrintButtons();
        await LoadCalibrationForSelectedPrinterAsync();
    }

    private void NumberInput_ValueChanged(object? sender, EventArgs e)
    {
        UpdateAssetRange();
    }

    private async void SettingsButton_Click(object? sender, EventArgs e)
    {
        string[] installedPrinters = printerComboBox.Items
            .Cast<string>()
            .ToArray();

        using var settingsForm = new SettingsForm(_settings, installedPrinters);
        if (settingsForm.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            await _settingsService.SaveAsync(settingsForm.Settings);
            _settings = settingsForm.Settings;
            UpdateAssetRange();
            SelectPreferredPrinter(_settings.DefaultPrinterName);
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się zapisać ustawień.\r\n\r\n{exception.Message}");
        }
    }

    private async void SaveCalibrationButton_Click(object? sender, EventArgs e)
    {
        string? printerName = GetSelectedPrinterName();
        if (printerName is null)
        {
            ShowError("Wybierz drukarkę.");
            return;
        }

        try
        {
            await _printerCalibrationService.SaveCalibrationAsync(
                printerName,
                GetPrinterCalibration());

            MessageBox.Show(
                this,
                $"Zapisano kalibrację drukarki:\r\n{printerName}",
                "Kalibracja drukarki",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się zapisać kalibracji.\r\n\r\n{exception.Message}");
        }
    }

    private void PreviewButton_Click(object? sender, EventArgs e)
    {
        string? printerName = GetSelectedPrinterName();
        if (printerName is null)
        {
            ShowError("Wybierz drukarkę.");
            return;
        }

        int startNumber = decimal.ToInt32(firstNumberNumericUpDown.Value);
        int quantity = decimal.ToInt32(quantityNumericUpDown.Value);
        PrinterCalibration calibration = GetPrinterCalibration();

        try
        {
            using var printJob = CreatePrintJob(
                printerName,
                startNumber,
                quantity,
                calibration,
                LabelRenderMode.Preview);
            using var previewDialog = new PrintPreviewDialog
            {
                Document = printJob.Document,
                Width = 1100,
                Height = 720,
                StartPosition = FormStartPosition.CenterParent
            };

            previewDialog.ShowDialog(this);
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się wygenerować podglądu.\r\n\r\n{exception.Message}");
        }
    }

    private async void PrintButton_Click(object? sender, EventArgs e)
    {
        string? printerName = GetSelectedPrinterName();
        if (printerName is null)
        {
            ShowError("Wybierz drukarkę.");
            return;
        }

        int startNumber = decimal.ToInt32(firstNumberNumericUpDown.Value);
        int quantity = decimal.ToInt32(quantityNumericUpDown.Value);
        int endNumber = startNumber + quantity - 1;
        int slotsPerPhysicalLabel = LabelColumns * LabelRows;
        PrinterCalibration calibration = GetPrinterCalibration();
        int physicalLabels = (int)Math.Ceiling(
            quantity / (double)slotsPerPhysicalLabel);

        string confirmation =
            $"Drukarka: {printerName}\r\n" +
            $"Zakres: {FormatAssetId(startNumber)} – {FormatAssetId(endNumber)}\r\n" +
            $"Małych etykiet: {quantity}\r\n" +
            $"Fizycznych etykiet: {physicalLabels}\r\n\r\n" +
            "Wysłać do drukarki?";

        DialogResult answer = MessageBox.Show(
            this,
            confirmation,
            "Potwierdzenie wydruku",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        try
        {
            using var printJob = CreatePrintJob(
                printerName,
                startNumber,
                quantity,
                calibration,
                LabelRenderMode.Print);
            printJob.Document.Print();
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się wydrukować etykiet.\r\n\r\n{exception.Message}");
            return;
        }

        int nextAssetNumber = endNumber + 1;
        _settings = _settings with { NextAssetNumber = nextAssetNumber };
        firstNumberNumericUpDown.Value = nextAssetNumber;

        try
        {
            await _settingsService.SaveAsync(_settings);
        }
        catch (Exception exception)
        {
            ShowError(
                "Zadanie zostało przekazane do systemu drukowania, ale nie udało się " +
                $"zapisać następnego numeru.\r\n\r\n{exception.Message}");
        }
    }

    private LabelPrintJob CreatePrintJob(
        string printerName,
        int startNumber,
        int quantity,
        PrinterCalibration calibration,
        LabelRenderMode renderMode)
    {
        var content = new LabelContentOptions(
            _settings.CompanyName,
            _settings.AssetId.Prefix,
            _settings.AssetId.Digits);
        var options = new LabelPrintOptions(
            printerName,
            startNumber,
            quantity,
            LabelWidthMm,
            LabelHeightMm,
            LabelColumns,
            LabelRows,
            DrawCutLines,
            content,
            calibration,
            renderMode);

        return new LabelPrintJob(options);
    }

    private async Task LoadCalibrationForSelectedPrinterAsync()
    {
        string? printerName = GetSelectedPrinterName();
        if (printerName is null)
        {
            SetCalibrationControls(new PrinterCalibration());
            return;
        }

        try
        {
            PrinterCalibration calibration =
                await _printerCalibrationService.GetCalibrationAsync(printerName);

            if (string.Equals(
                GetSelectedPrinterName(),
                printerName,
                StringComparison.OrdinalIgnoreCase))
            {
                SetCalibrationControls(calibration);
            }
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się odczytać kalibracji.\r\n\r\n{exception.Message}");
        }
    }

    private void SetCalibrationControls(PrinterCalibration calibration)
    {
        calibrationXNumericUpDown.Value = (decimal)calibration.OffsetXmm;
        calibrationYNumericUpDown.Value = (decimal)calibration.OffsetYmm;
    }

    private PrinterCalibration GetPrinterCalibration()
    {
        return new PrinterCalibration(
            decimal.ToDouble(calibrationXNumericUpDown.Value),
            decimal.ToDouble(calibrationYNumericUpDown.Value));
    }

    private string? GetSelectedPrinterName()
    {
        return printerComboBox.SelectedItem as string;
    }

    private string FormatAssetId(int number)
    {
        return AssetIdFormatter.Format(
            number,
            _settings.AssetId.Prefix,
            _settings.AssetId.Digits);
    }

    private void UpdateAssetRange()
    {
        int startNumber = decimal.ToInt32(firstNumberNumericUpDown.Value);
        int quantity = decimal.ToInt32(quantityNumericUpDown.Value);
        int endNumber = startNumber + quantity - 1;

        assetRangeLabel.Text =
            $"{FormatAssetId(startNumber)} – {FormatAssetId(endNumber)}";
    }

    private void UpdatePrintButtons()
    {
        bool hasSelectedPrinter = GetSelectedPrinterName() is not null;
        previewButton.Enabled = hasSelectedPrinter;
        printButton.Enabled = hasSelectedPrinter;
        saveCalibrationButton.Enabled = hasSelectedPrinter;
        calibrationXNumericUpDown.Enabled = hasSelectedPrinter;
        calibrationYNumericUpDown.Enabled = hasSelectedPrinter;
    }

    private void ShowError(string message)
    {
        MessageBox.Show(
            this,
            message,
            "Etykiety IT",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
