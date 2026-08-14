using System.Drawing.Printing;
using EtykietyIT.Forms;
using EtykietyIT.Models;
using EtykietyIT.Printing;
using EtykietyIT.Services;

namespace EtykietyIT;

public partial class Form1 : Form
{
    private readonly SettingsService _settingsService;
    private readonly PrinterCalibrationService _printerCalibrationService;
    private readonly LabelProfileService _labelProfileService;

    private ApplicationSettings _settings;
    private LabelProfile? _selectedProfile;
    private bool _loadingProfiles;

    public Form1(
        SettingsService settingsService,
        PrinterCalibrationService printerCalibrationService,
        LabelProfileService labelProfileService,
        ApplicationSettings settings)
    {
        _settingsService = settingsService ??
            throw new ArgumentNullException(nameof(settingsService));
        _printerCalibrationService = printerCalibrationService ??
            throw new ArgumentNullException(nameof(printerCalibrationService));
        _labelProfileService = labelProfileService ??
            throw new ArgumentNullException(nameof(labelProfileService));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _settings.Validate();

        InitializeComponent();

        printerComboBox.SelectedIndexChanged += PrinterComboBox_SelectedIndexChanged;
        profileComboBox.SelectedIndexChanged += ProfileComboBox_SelectedIndexChanged;
        firstNumberNumericUpDown.ValueChanged += NumberInput_ValueChanged;
        quantityNumericUpDown.ValueChanged += NumberInput_ValueChanged;
        settingsButton.Click += SettingsButton_Click;
        profilesButton.Click += ProfilesButton_Click;
        saveCalibrationButton.Click += SaveCalibrationButton_Click;
        previewButton.Click += PreviewButton_Click;
        printButton.Click += PrintButton_Click;
        Shown += Form1_Shown;

        firstNumberNumericUpDown.Value = _settings.NextAssetNumber;
        LoadInstalledPrinters();
        UpdateAssetRange();
    }

    private async void Form1_Shown(object? sender, EventArgs e)
    {
        await ReloadProfilesAsync(_settings.DefaultProfileId);
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

    private async void ProfileComboBox_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        _selectedProfile = profileComboBox.SelectedItem as LabelProfile;
        UpdatePrintButtons();

        if (_loadingProfiles || _selectedProfile is null || string.Equals(
            _settings.DefaultProfileId,
            _selectedProfile.Id,
            StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        ApplicationSettings updatedSettings = _settings with
        {
            DefaultProfileId = _selectedProfile.Id
        };

        profileComboBox.Enabled = false;
        try
        {
            await _settingsService.SaveAsync(updatedSettings);
            _settings = updatedSettings;
        }
        catch (Exception exception)
        {
            ShowError(
                $"Nie udało się zapisać domyślnego profilu.\r\n\r\n{exception.Message}");
        }
        finally
        {
            profileComboBox.Enabled = true;
        }
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

    private async void ProfilesButton_Click(object? sender, EventArgs e)
    {
        string preferredProfileId = _selectedProfile?.Id ??
            _settings.DefaultProfileId;

        using var profilesForm = new ProfilesForm(_labelProfileService);
        profilesForm.ShowDialog(this);

        await ReloadProfilesAsync(preferredProfileId);
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
        LabelProfile profile = GetSelectedProfile();
        int slotsPerPhysicalLabel = profile.Columns * profile.Rows;
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
        LabelProfile profile = GetSelectedProfile();
        var content = new LabelContentOptions(
            _settings.CompanyName,
            _settings.AssetId.Prefix,
            _settings.AssetId.Digits);
        var options = new LabelPrintOptions(
            printerName,
            startNumber,
            quantity,
            profile.WidthMm,
            profile.HeightMm,
            profile.Columns,
            profile.Rows,
            profile.DrawCutLines,
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

    private LabelProfile GetSelectedProfile()
    {
        return _selectedProfile ?? throw new InvalidOperationException(
            "Nie wybrano profilu etykiety.");
    }

    private async Task ReloadProfilesAsync(string preferredProfileId)
    {
        try
        {
            IReadOnlyList<LabelProfile> profiles =
                await _labelProfileService.GetAllAsync();
            LabelProfile selectedProfile =
                await _labelProfileService.GetProfileOrDefaultAsync(
                    preferredProfileId);

            _loadingProfiles = true;
            try
            {
                profileComboBox.DataSource = null;
                profileComboBox.DisplayMember = nameof(LabelProfile.Name);
                profileComboBox.DataSource = profiles.ToList();

                for (int index = 0; index < profiles.Count; index++)
                {
                    if (string.Equals(
                        profiles[index].Id,
                        selectedProfile.Id,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        profileComboBox.SelectedIndex = index;
                        break;
                    }
                }

                _selectedProfile = selectedProfile;
            }
            finally
            {
                _loadingProfiles = false;
            }

            if (!string.Equals(
                _settings.DefaultProfileId,
                selectedProfile.Id,
                StringComparison.OrdinalIgnoreCase))
            {
                ApplicationSettings updatedSettings = _settings with
                {
                    DefaultProfileId = selectedProfile.Id
                };
                await _settingsService.SaveAsync(updatedSettings);
                _settings = updatedSettings;
            }

            UpdatePrintButtons();
        }
        catch (Exception exception)
        {
            _selectedProfile = null;
            profileComboBox.DataSource = null;
            UpdatePrintButtons();
            ShowError($"Nie udało się wczytać profili.\r\n\r\n{exception.Message}");
        }
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
        bool hasSelectedProfile = _selectedProfile is not null;
        previewButton.Enabled = hasSelectedPrinter && hasSelectedProfile;
        printButton.Enabled = hasSelectedPrinter && hasSelectedProfile;
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
