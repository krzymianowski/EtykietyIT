using System.Drawing.Printing;
using EtykietyIT.Export;
using EtykietyIT.Forms;
using EtykietyIT.Models;
using EtykietyIT.Printing;
using EtykietyIT.Services;

namespace EtykietyIT;

public partial class MainForm : Form
{
    private readonly SettingsService _settingsService;
    private readonly OrganizationProfileService _organizationProfileService;
    private readonly PrinterCalibrationService _printerCalibrationService;
    private readonly LabelProfileService _labelProfileService;
    private readonly PrintHistoryService _printHistoryService;
    private readonly ApplicationVersionService _applicationVersionService;
    private readonly IHistoryExporter _csvHistoryExporter;
    private readonly IHistoryExporter _xlsxHistoryExporter;

    private ApplicationSettings _settings;
    private OrganizationProfile? _activeOrganization;
    private LabelProfile? _selectedProfile;
    private bool _loadingOrganizations;
    private bool _applyingOrganization;
    private bool _loadingProfiles;

    public MainForm(
        SettingsService settingsService,
        OrganizationProfileService organizationProfileService,
        PrinterCalibrationService printerCalibrationService,
        LabelProfileService labelProfileService,
        PrintHistoryService printHistoryService,
        ApplicationVersionService applicationVersionService,
        IHistoryExporter csvHistoryExporter,
        IHistoryExporter xlsxHistoryExporter,
        ApplicationSettings settings)
    {
        _settingsService = settingsService ??
            throw new ArgumentNullException(nameof(settingsService));
        _organizationProfileService = organizationProfileService ??
            throw new ArgumentNullException(nameof(organizationProfileService));
        _printerCalibrationService = printerCalibrationService ??
            throw new ArgumentNullException(nameof(printerCalibrationService));
        _labelProfileService = labelProfileService ??
            throw new ArgumentNullException(nameof(labelProfileService));
        _printHistoryService = printHistoryService ??
            throw new ArgumentNullException(nameof(printHistoryService));
        _applicationVersionService = applicationVersionService ??
            throw new ArgumentNullException(nameof(applicationVersionService));
        _csvHistoryExporter = csvHistoryExporter ??
            throw new ArgumentNullException(nameof(csvHistoryExporter));
        _xlsxHistoryExporter = xlsxHistoryExporter ??
            throw new ArgumentNullException(nameof(xlsxHistoryExporter));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _settings.Validate();

        InitializeComponent();

        organizationComboBox.SelectedIndexChanged +=
            OrganizationComboBox_SelectedIndexChanged;
        manageOrganizationsButton.Click += ManageOrganizationsButton_Click;
        printerComboBox.SelectedIndexChanged += PrinterComboBox_SelectedIndexChanged;
        profileComboBox.SelectedIndexChanged += ProfileComboBox_SelectedIndexChanged;
        firstNumberNumericUpDown.ValueChanged += NumberInput_ValueChanged;
        quantityNumericUpDown.ValueChanged += NumberInput_ValueChanged;
        profilesButton.Click += ProfilesButton_Click;
        historyButton.Click += HistoryButton_Click;
        saveCalibrationButton.Click += SaveCalibrationButton_Click;
        previewButton.Click += PreviewButton_Click;
        printButton.Click += PrintButton_Click;
        exitMenuItem.Click += ExitMenuItem_Click;
        organizationProfilesMenuItem.Click += ManageOrganizationsButton_Click;
        labelProfilesMenuItem.Click += ProfilesButton_Click;
        aboutMenuItem.Click += AboutMenuItem_Click;
        Shown += MainForm_Shown;

        firstNumberNumericUpDown.Value = 1;
        LoadInstalledPrinters();
        UpdateAssetRange();
    }

    private async void MainForm_Shown(object? sender, EventArgs e)
    {
        await ReloadOrganizationsAsync(_settings.ActiveOrganizationProfileId);
    }

    private void LoadInstalledPrinters()
    {
        printerComboBox.Items.Clear();
        foreach (string printerName in PrinterSettings.InstalledPrinters)
        {
            printerComboBox.Items.Add(printerName);
        }

        printerComboBox.SelectedIndex = -1;
        UpdatePrintButtons();
    }

    private async void OrganizationComboBox_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        if (_loadingOrganizations ||
            organizationComboBox.SelectedItem is not OrganizationProfile selected)
        {
            return;
        }

        string previousOrganizationId = _settings.ActiveOrganizationProfileId;
        var updatedSettings = _settings with
        {
            ActiveOrganizationProfileId = selected.Id
        };

        organizationComboBox.Enabled = false;
        try
        {
            await _settingsService.SaveAsync(updatedSettings);
            _settings = updatedSettings;
            await ApplyOrganizationAsync(selected);
        }
        catch (Exception exception)
        {
            ShowError(
                $"Nie udało się przełączyć organizacji.\r\n\r\n{exception.Message}");
            await ReloadOrganizationsAsync(previousOrganizationId);
        }
        finally
        {
            organizationComboBox.Enabled = true;
        }
    }

    private async void ManageOrganizationsButton_Click(
        object? sender,
        EventArgs e)
    {
        string[] installedPrinters = printerComboBox.Items
            .Cast<string>()
            .ToArray();
        using var organizationsForm = new OrganizationsForm(
            _organizationProfileService,
            _labelProfileService,
            _settingsService,
            _settings,
            installedPrinters);
        organizationsForm.ShowDialog(this);

        try
        {
            _settings = await _settingsService.LoadAsync();
            await ReloadOrganizationsAsync(
                _settings.ActiveOrganizationProfileId);
        }
        catch (Exception exception)
        {
            ShowError(
                $"Nie udało się odświeżyć organizacji.\r\n\r\n{exception.Message}");
        }
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

        printerComboBox.SelectedIndex = printerComboBox.Items.Count > 0
            ? dymoIndex >= 0 ? dymoIndex : 0
            : -1;
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
        if (_applyingOrganization)
        {
            return;
        }

        printerComboBox.Enabled = false;
        try
        {
            string? printerName = GetSelectedPrinterName();
            OrganizationProfile? organization = _activeOrganization;
            if (organization is not null && !string.Equals(
                organization.DefaultPrinterName,
                printerName,
                StringComparison.OrdinalIgnoreCase))
            {
                OrganizationProfile updated = organization with
                {
                    DefaultPrinterName = printerName
                };
                try
                {
                    await _organizationProfileService.UpdateAsync(updated);
                    if (IsActiveOrganization(organization.Id))
                    {
                        _activeOrganization = updated;
                    }
                }
                catch (Exception exception)
                {
                    ShowError(
                        "Nie udało się zapisać domyślnej drukarki organizacji." +
                        $"\r\n\r\n{exception.Message}");
                }
            }

            await LoadCalibrationForSelectedPrinterAsync();
        }
        finally
        {
            printerComboBox.Enabled = true;
        }
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

        OrganizationProfile? organization = _activeOrganization;
        if (_loadingProfiles || organization is null ||
            _selectedProfile is null || string.Equals(
                organization.DefaultLabelProfileId,
                _selectedProfile.Id,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        profileComboBox.Enabled = false;
        try
        {
            OrganizationProfile updated = organization with
            {
                DefaultLabelProfileId = _selectedProfile.Id
            };
            await _organizationProfileService.UpdateAsync(updated);
            if (IsActiveOrganization(organization.Id))
            {
                _activeOrganization = updated;
            }
        }
        catch (Exception exception)
        {
            ShowError(
                "Nie udało się zapisać domyślnego profilu etykiety " +
                $"organizacji.\r\n\r\n{exception.Message}");
        }
        finally
        {
            profileComboBox.Enabled = true;
        }
    }

    private async void ProfilesButton_Click(object? sender, EventArgs e)
    {
        string preferredProfileId = _selectedProfile?.Id ??
            GetActiveOrganization().DefaultLabelProfileId;

        using var profilesForm = new ProfilesForm(_labelProfileService);
        profilesForm.ShowDialog(this);

        try
        {
            await ReloadLabelProfilesAsync(preferredProfileId);
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się odświeżyć profili.\r\n\r\n{exception.Message}");
        }
    }

    private void HistoryButton_Click(object? sender, EventArgs e)
    {
        using var historyForm = new HistoryForm(
            _printHistoryService,
            _csvHistoryExporter,
            _xlsxHistoryExporter);
        historyForm.ShowDialog(this);
    }

    private void ExitMenuItem_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void AboutMenuItem_Click(object? sender, EventArgs e)
    {
        using var aboutForm = new AboutForm(_applicationVersionService);
        aboutForm.ShowDialog(this);
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

        if (!TryGetPrintRange(out int startNumber, out int quantity, out _))
        {
            return;
        }

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

        if (!TryGetPrintRange(
            out int startNumber,
            out int quantity,
            out int endNumber))
        {
            return;
        }

        OrganizationProfile organization = GetActiveOrganization();
        LabelProfile profile = GetSelectedProfile();
        int slotsPerPhysicalLabel = profile.Columns * profile.Rows;
        PrinterCalibration calibration = GetPrinterCalibration();
        int physicalLabels = (int)Math.Ceiling(
            quantity / (double)slotsPerPhysicalLabel);

        string confirmation =
            $"Organizacja: {organization.Name}\r\n" +
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

        var historyEntry = new PrintHistoryEntry
        {
            Id = Guid.NewGuid(),
            TimestampUtc = DateTimeOffset.UtcNow,
            ApplicationVersion = _applicationVersionService.UserVersion,
            Snapshot = new PrintHistorySnapshot
            {
                OrganizationProfileId = organization.Id,
                OrganizationProfileName = organization.Name,
                StartNumber = startNumber,
                EndNumber = endNumber,
                FirstAssetId = FormatAssetId(startNumber, organization),
                LastAssetId = FormatAssetId(endNumber, organization),
                Prefix = organization.AssetId.Prefix,
                Digits = organization.AssetId.Digits,
                CompanyName = organization.CompanyName,
                PrinterName = printerName,
                OffsetXmm = calibration.OffsetXmm,
                OffsetYmm = calibration.OffsetYmm,
                ProfileId = profile.Id,
                ProfileName = profile.Name,
                WidthMm = profile.WidthMm,
                HeightMm = profile.HeightMm,
                Columns = profile.Columns,
                Rows = profile.Rows,
                DrawCutLines = profile.DrawCutLines,
                SmallLabelQuantity = quantity,
                PhysicalLabelQuantity = physicalLabels,
                QrEnabled = false
            }
        };

        try
        {
            await _printHistoryService.AppendAsync(historyEntry);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                "Zadanie zostało przekazane do systemu drukowania Windows, " +
                "ale nie udało się zapisać historii. Wydruk nie zostanie wysłany " +
                "ponownie. Numeracja organizacji zostanie przesunięta, aby nie " +
                $"powtórzyć wysłanego zakresu.\r\n\r\n{exception.Message}",
                "Błąd zapisu historii",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        OrganizationProfile updatedOrganization = organization with
        {
            NextAssetNumber = endNumber + 1
        };
        try
        {
            await _organizationProfileService.UpdateAsync(updatedOrganization);
            if (IsActiveOrganization(organization.Id))
            {
                _activeOrganization = updatedOrganization;
                firstNumberNumericUpDown.Value =
                    updatedOrganization.NextAssetNumber;
            }
        }
        catch (Exception exception)
        {
            ShowError(
                "Zadanie zostało przekazane do systemu drukowania, ale nie udało " +
                "się zapisać następnego numeru aktywnej organizacji." +
                $"\r\n\r\n{exception.Message}");
        }
    }

    private LabelPrintJob CreatePrintJob(
        string printerName,
        int startNumber,
        int quantity,
        PrinterCalibration calibration,
        LabelRenderMode renderMode)
    {
        OrganizationProfile organization = GetActiveOrganization();
        LabelProfile profile = GetSelectedProfile();
        var content = new LabelContentOptions(
            organization.CompanyName,
            organization.AssetId.Prefix,
            organization.AssetId.Digits);
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

    private async Task ReloadOrganizationsAsync(string preferredOrganizationId)
    {
        try
        {
            OrganizationProfileReadResult result =
                await _organizationProfileService.GetAllAsync();
            OrganizationProfile selected = result.Profiles.FirstOrDefault(
                profile => string.Equals(
                    profile.Id,
                    preferredOrganizationId,
                    StringComparison.Ordinal)) ?? result.Profiles.FirstOrDefault()
                    ?? throw new InvalidOperationException(
                        "Brak dostępnych profili organizacji.");

            _loadingOrganizations = true;
            try
            {
                organizationComboBox.DataSource = null;
                organizationComboBox.DisplayMember = nameof(OrganizationProfile.Name);
                organizationComboBox.DataSource = result.Profiles.ToList();
                organizationComboBox.SelectedItem = result.Profiles.First(
                    profile => string.Equals(
                        profile.Id,
                        selected.Id,
                        StringComparison.Ordinal));
            }
            finally
            {
                _loadingOrganizations = false;
            }

            if (!string.Equals(
                _settings.ActiveOrganizationProfileId,
                selected.Id,
                StringComparison.Ordinal))
            {
                ApplicationSettings updatedSettings = _settings with
                {
                    ActiveOrganizationProfileId = selected.Id
                };
                await _settingsService.SaveAsync(updatedSettings);
                _settings = updatedSettings;
            }

            await ApplyOrganizationAsync(selected);
        }
        catch (Exception exception)
        {
            _activeOrganization = null;
            _selectedProfile = null;
            organizationComboBox.DataSource = null;
            profileComboBox.DataSource = null;
            printerComboBox.SelectedIndex = -1;
            UpdateAssetRange();
            UpdatePrintButtons();
            ShowError($"Nie udało się wczytać organizacji.\r\n\r\n{exception.Message}");
        }
    }

    private async Task ApplyOrganizationAsync(OrganizationProfile organization)
    {
        _activeOrganization = organization;
        _applyingOrganization = true;
        try
        {
            firstNumberNumericUpDown.Value = organization.NextAssetNumber;
            await ReloadLabelProfilesAsync(
                organization.DefaultLabelProfileId);
            SelectPreferredPrinter(organization.DefaultPrinterName);
        }
        finally
        {
            _applyingOrganization = false;
        }

        await LoadCalibrationForSelectedPrinterAsync();
        UpdateAssetRange();
        UpdatePrintButtons();
    }

    private async Task ReloadLabelProfilesAsync(string preferredProfileId)
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
            profileComboBox.SelectedItem = profiles.First(profile =>
                string.Equals(
                    profile.Id,
                    selectedProfile.Id,
                    StringComparison.OrdinalIgnoreCase));
            _selectedProfile = selectedProfile;
        }
        finally
        {
            _loadingProfiles = false;
        }

        OrganizationProfile? organization = _activeOrganization;
        if (organization is not null && !string.Equals(
            organization.DefaultLabelProfileId,
            selectedProfile.Id,
            StringComparison.OrdinalIgnoreCase))
        {
            OrganizationProfile updated = organization with
            {
                DefaultLabelProfileId = selectedProfile.Id
            };
            await _organizationProfileService.UpdateAsync(updated);
            if (IsActiveOrganization(organization.Id))
            {
                _activeOrganization = updated;
            }
        }

        UpdatePrintButtons();
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

    private OrganizationProfile GetActiveOrganization()
    {
        return _activeOrganization ?? throw new InvalidOperationException(
            "Nie wybrano profilu organizacji.");
    }

    private LabelProfile GetSelectedProfile()
    {
        return _selectedProfile ?? throw new InvalidOperationException(
            "Nie wybrano profilu etykiety.");
    }

    private bool IsActiveOrganization(string organizationId)
    {
        return string.Equals(
            _activeOrganization?.Id,
            organizationId,
            StringComparison.Ordinal);
    }

    private string FormatAssetId(int number)
    {
        return FormatAssetId(number, GetActiveOrganization());
    }

    private static string FormatAssetId(
        int number,
        OrganizationProfile organization)
    {
        return AssetIdFormatter.Format(
            number,
            organization.AssetId.Prefix,
            organization.AssetId.Digits);
    }

    private void UpdateAssetRange()
    {
        if (_activeOrganization is null)
        {
            assetRangeLabel.Text = "—";
            return;
        }

        int startNumber = decimal.ToInt32(firstNumberNumericUpDown.Value);
        int quantity = decimal.ToInt32(quantityNumericUpDown.Value);
        long endNumber = (long)startNumber + quantity - 1;
        assetRangeLabel.Text = endNumber <= int.MaxValue
            ? $"{FormatAssetId(startNumber)} – {FormatAssetId((int)endNumber)}"
            : "Zakres przekracza maksymalny numer";
    }

    private bool TryGetPrintRange(
        out int startNumber,
        out int quantity,
        out int endNumber)
    {
        startNumber = decimal.ToInt32(firstNumberNumericUpDown.Value);
        quantity = decimal.ToInt32(quantityNumericUpDown.Value);
        long calculatedEndNumber = (long)startNumber + quantity - 1;
        if (calculatedEndNumber >= int.MaxValue)
        {
            endNumber = default;
            ShowError(
                "Zakres Asset ID nie pozostawia prawidłowego następnego numeru.");
            return false;
        }

        endNumber = (int)calculatedEndNumber;
        return true;
    }

    private void UpdatePrintButtons()
    {
        bool hasSelectedPrinter = GetSelectedPrinterName() is not null;
        bool hasSelectedProfile = _selectedProfile is not null;
        bool hasOrganization = _activeOrganization is not null;
        previewButton.Enabled =
            hasSelectedPrinter && hasSelectedProfile && hasOrganization;
        printButton.Enabled =
            hasSelectedPrinter && hasSelectedProfile && hasOrganization;
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
