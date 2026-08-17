using EtykietyIT.Models;
using EtykietyIT.Services;

namespace EtykietyIT.Forms;

public partial class OrganizationEditForm : Form
{
    private const string AutomaticPrinterItem = "(wybór automatyczny)";

    private readonly OrganizationProfile _originalProfile;

    public OrganizationEditForm(
        OrganizationProfile profile,
        IReadOnlyList<LabelProfile> labelProfiles,
        IEnumerable<string> installedPrinters,
        string windowTitle)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(labelProfiles);
        ArgumentNullException.ThrowIfNull(installedPrinters);
        ArgumentException.ThrowIfNullOrWhiteSpace(windowTitle);
        if (labelProfiles.Count == 0)
        {
            throw new InvalidOperationException(
                "Brak profili etykiet dostępnych dla organizacji.");
        }

        _originalProfile = profile;
        Profile = profile;

        InitializeComponent();
        Text = windowTitle;

        organizationNameTextBox.Text = profile.Name;
        companyNameTextBox.Text = profile.CompanyName;
        assetIdPrefixTextBox.Text = profile.AssetId.Prefix;
        assetIdDigitsNumericUpDown.Minimum = AssetIdSettings.MinimumDigits;
        assetIdDigitsNumericUpDown.Maximum = AssetIdSettings.MaximumDigits;
        assetIdDigitsNumericUpDown.Value = profile.AssetId.Digits;
        nextAssetNumberNumericUpDown.Value = profile.NextAssetNumber;

        defaultLabelProfileComboBox.DisplayMember = nameof(LabelProfile.Name);
        defaultLabelProfileComboBox.DataSource = labelProfiles.ToList();
        SelectLabelProfile(profile.DefaultLabelProfileId, labelProfiles);

        defaultPrinterComboBox.Items.Add(AutomaticPrinterItem);
        foreach (string printerName in installedPrinters)
        {
            defaultPrinterComboBox.Items.Add(printerName);
        }

        SelectPrinter(profile.DefaultPrinterName);
        defaultQrCheckBox.Checked = profile.DefaultQrEnabled;

        saveButton.Click += SaveButton_Click;
        cancelButton.Click += CancelButton_Click;
    }

    public OrganizationProfile Profile { get; private set; }

    private void SelectLabelProfile(
        string profileId,
        IReadOnlyList<LabelProfile> labelProfiles)
    {
        int selectedIndex = -1;
        for (int index = 0; index < labelProfiles.Count; index++)
        {
            if (string.Equals(
                labelProfiles[index].Id,
                profileId,
                StringComparison.OrdinalIgnoreCase))
            {
                selectedIndex = index;
                break;
            }
        }

        if (selectedIndex < 0)
        {
            for (int index = 0; index < labelProfiles.Count; index++)
            {
                if (string.Equals(
                    labelProfiles[index].Id,
                    LabelProfileService.DefaultBuiltInProfileId,
                    StringComparison.OrdinalIgnoreCase))
                {
                    selectedIndex = index;
                    break;
                }
            }
        }

        defaultLabelProfileComboBox.SelectedIndex = selectedIndex >= 0
            ? selectedIndex
            : 0;
    }

    private void SelectPrinter(string? printerName)
    {
        if (string.IsNullOrWhiteSpace(printerName))
        {
            defaultPrinterComboBox.SelectedIndex = 0;
            return;
        }

        for (int index = 1; index < defaultPrinterComboBox.Items.Count; index++)
        {
            if (defaultPrinterComboBox.Items[index] is string installedPrinter &&
                string.Equals(
                    installedPrinter,
                    printerName,
                    StringComparison.OrdinalIgnoreCase))
            {
                defaultPrinterComboBox.SelectedIndex = index;
                return;
            }
        }

        defaultPrinterComboBox.Items.Add(printerName);
        defaultPrinterComboBox.SelectedIndex =
            defaultPrinterComboBox.Items.Count - 1;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if (defaultLabelProfileComboBox.SelectedItem is not LabelProfile labelProfile)
        {
            ShowValidationError("Wybierz domyślny profil etykiety.");
            return;
        }

        string? defaultPrinterName = defaultPrinterComboBox.SelectedIndex > 0
            ? defaultPrinterComboBox.SelectedItem as string
            : null;
        var updatedProfile = _originalProfile with
        {
            Name = organizationNameTextBox.Text.Trim(),
            CompanyName = companyNameTextBox.Text.Trim(),
            AssetId = new AssetIdSettings
            {
                Prefix = assetIdPrefixTextBox.Text.Trim(),
                Digits = decimal.ToInt32(assetIdDigitsNumericUpDown.Value)
            },
            NextAssetNumber = decimal.ToInt32(
                nextAssetNumberNumericUpDown.Value),
            DefaultLabelProfileId = labelProfile.Id,
            DefaultPrinterName = defaultPrinterName,
            DefaultQrEnabled = defaultQrCheckBox.Checked
        };

        try
        {
            updatedProfile.Validate();
            Profile = updatedProfile;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (InvalidOperationException exception)
        {
            ShowValidationError(exception.Message);
        }
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void ShowValidationError(string message)
    {
        MessageBox.Show(
            this,
            message,
            "Nieprawidłowy profil organizacji",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}
