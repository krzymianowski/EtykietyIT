using System.Drawing.Printing;
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

    public Form1()
    {
        InitializeComponent();

        printerComboBox.SelectedIndexChanged += PrinterComboBox_SelectedIndexChanged;
        firstNumberNumericUpDown.ValueChanged += NumberInput_ValueChanged;
        quantityNumericUpDown.ValueChanged += NumberInput_ValueChanged;
        previewButton.Click += PreviewButton_Click;
        printButton.Click += PrintButton_Click;

        LoadInstalledPrinters();
        UpdateAssetRange();
    }

    private void LoadInstalledPrinters()
    {
        foreach (string printerName in PrinterSettings.InstalledPrinters)
        {
            printerComboBox.Items.Add(printerName);
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

        UpdatePrintButtons();
    }

    private void PrinterComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdatePrintButtons();
    }

    private void NumberInput_ValueChanged(object? sender, EventArgs e)
    {
        UpdateAssetRange();
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

    private void PrintButton_Click(object? sender, EventArgs e)
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
            $"Zakres: {AssetIdFormatter.Format(startNumber)} – " +
            $"{AssetIdFormatter.Format(endNumber)}\r\n" +
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

            firstNumberNumericUpDown.Value = endNumber + 1;
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się wydrukować etykiet.\r\n\r\n{exception.Message}");
        }
    }

    private static LabelPrintJob CreatePrintJob(
        string printerName,
        int startNumber,
        int quantity,
        PrinterCalibration calibration,
        LabelRenderMode renderMode)
    {
        var options = new LabelPrintOptions(
            printerName,
            startNumber,
            quantity,
            LabelWidthMm,
            LabelHeightMm,
            LabelColumns,
            LabelRows,
            DrawCutLines,
            calibration,
            renderMode);

        return new LabelPrintJob(options);
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

    private void UpdateAssetRange()
    {
        int startNumber = decimal.ToInt32(firstNumberNumericUpDown.Value);
        int quantity = decimal.ToInt32(quantityNumericUpDown.Value);
        int endNumber = startNumber + quantity - 1;

        assetRangeLabel.Text =
            $"{AssetIdFormatter.Format(startNumber)} – " +
            AssetIdFormatter.Format(endNumber);
    }

    private void UpdatePrintButtons()
    {
        bool hasSelectedPrinter = GetSelectedPrinterName() is not null;
        previewButton.Enabled = hasSelectedPrinter;
        printButton.Enabled = hasSelectedPrinter;
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
