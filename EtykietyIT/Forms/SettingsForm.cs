using EtykietyIT.Models;

namespace EtykietyIT.Forms;

public partial class SettingsForm : Form
{
    private const string AutomaticPrinterItem = "(wybór automatyczny)";

    private readonly ApplicationSettings _originalSettings;

    public SettingsForm(
        ApplicationSettings settings,
        IEnumerable<string> installedPrinters)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(installedPrinters);
        settings.Validate();

        _originalSettings = settings;
        Settings = settings;

        InitializeComponent();

        companyNameTextBox.Text = settings.CompanyName;
        assetIdPrefixTextBox.Text = settings.AssetId.Prefix;
        assetIdDigitsNumericUpDown.Minimum = AssetIdSettings.MinimumDigits;
        assetIdDigitsNumericUpDown.Maximum = AssetIdSettings.MaximumDigits;
        assetIdDigitsNumericUpDown.Value = settings.AssetId.Digits;

        defaultPrinterComboBox.Items.Add(AutomaticPrinterItem);
        foreach (string printerName in installedPrinters)
        {
            defaultPrinterComboBox.Items.Add(printerName);
        }

        SelectCurrentDefaultPrinter(settings.DefaultPrinterName);

        saveButton.Click += SaveButton_Click;
        cancelButton.Click += CancelButton_Click;
    }

    public ApplicationSettings Settings { get; private set; }

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

        var updatedSettings = _originalSettings with
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
            updatedSettings.Validate();
            Settings = updatedSettings;
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
