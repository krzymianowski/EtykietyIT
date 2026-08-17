using EtykietyIT.Models;

namespace EtykietyIT.Forms;

internal partial class SettingsForm : Form
{
    private const string AutomaticPrinterItem = "(wybór automatyczny)";

    private readonly OrganizationProfile _originalProfile;

    public SettingsForm(
        OrganizationProfile profile,
        IEnumerable<string> installedPrinters)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(installedPrinters);
        profile.Validate();

        _originalProfile = profile;
        Profile = profile;

        InitializeComponent();
        ApplicationIconProvider.Apply(this);

        companyNameTextBox.Text = profile.CompanyName;
        assetIdPrefixTextBox.Text = profile.AssetId.Prefix;
        assetIdDigitsNumericUpDown.Minimum = AssetIdSettings.MinimumDigits;
        assetIdDigitsNumericUpDown.Maximum = AssetIdSettings.MaximumDigits;
        assetIdDigitsNumericUpDown.Value = profile.AssetId.Digits;

        defaultPrinterComboBox.Items.Add(AutomaticPrinterItem);
        foreach (string printerName in installedPrinters)
        {
            defaultPrinterComboBox.Items.Add(printerName);
        }

        SelectCurrentDefaultPrinter(profile.DefaultPrinterName);

        saveButton.Click += SaveButton_Click;
        cancelButton.Click += CancelButton_Click;
    }

    public OrganizationProfile Profile { get; private set; }

    private void SelectCurrentDefaultPrinter(string? defaultPrinterName)
    {
        if (string.IsNullOrWhiteSpace(defaultPrinterName))
        {
            defaultPrinterComboBox.SelectedIndex = 0;
            return;
        }

        for (int index = 1; index < defaultPrinterComboBox.Items.Count; index++)
        {
            if (defaultPrinterComboBox.Items[index] is string printerName &&
                string.Equals(
                    printerName,
                    defaultPrinterName,
                    StringComparison.OrdinalIgnoreCase))
            {
                defaultPrinterComboBox.SelectedIndex = index;
                return;
            }
        }

        defaultPrinterComboBox.Items.Add(defaultPrinterName);
        defaultPrinterComboBox.SelectedIndex = defaultPrinterComboBox.Items.Count - 1;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        string? defaultPrinterName = defaultPrinterComboBox.SelectedIndex > 0
            ? defaultPrinterComboBox.SelectedItem as string
            : null;

        var updatedProfile = _originalProfile with
        {
            CompanyName = companyNameTextBox.Text.Trim(),
            AssetId = new AssetIdSettings
            {
                Prefix = assetIdPrefixTextBox.Text.Trim(),
                Digits = decimal.ToInt32(assetIdDigitsNumericUpDown.Value)
            },
            DefaultPrinterName = defaultPrinterName
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
            MessageBox.Show(
                this,
                exception.Message,
                "Nieprawidłowe ustawienia",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
