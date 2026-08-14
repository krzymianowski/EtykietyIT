using System.Globalization;
using System.Text;
using EtykietyIT.Export;
using EtykietyIT.Models;
using EtykietyIT.Services;

namespace EtykietyIT.Forms;

public partial class HistoryForm : Form
{
    private readonly PrintHistoryService _printHistoryService;
    private readonly IHistoryExporter _historyExporter;
    private IReadOnlyList<PrintHistoryEntry> _entries =
        Array.Empty<PrintHistoryEntry>();
    private IReadOnlyList<PrintHistoryEntry> _visibleEntries =
        Array.Empty<PrintHistoryEntry>();

    public HistoryForm(
        PrintHistoryService printHistoryService,
        IHistoryExporter historyExporter)
    {
        _printHistoryService = printHistoryService ??
            throw new ArgumentNullException(nameof(printHistoryService));
        _historyExporter = historyExporter ??
            throw new ArgumentNullException(nameof(historyExporter));

        InitializeComponent();
        InitializeGridColumns();

        dateFromDateTimePicker.Value = DateTime.Today.AddMonths(-1);
        dateToDateTimePicker.Value = DateTime.Today;
        UpdateDateFilterControls();

        Shown += HistoryForm_Shown;
        searchTextBox.TextChanged += FilterControl_Changed;
        dateFilterCheckBox.CheckedChanged += DateFilterCheckBox_CheckedChanged;
        dateFromDateTimePicker.ValueChanged += FilterControl_Changed;
        dateToDateTimePicker.ValueChanged += FilterControl_Changed;
        historyDataGridView.SelectionChanged += HistoryDataGridView_SelectionChanged;
        exportCsvButton.Click += ExportCsvButton_Click;
        closeButton.Click += CloseButton_Click;
    }

    private async void HistoryForm_Shown(object? sender, EventArgs e)
    {
        try
        {
            PrintHistoryReadResult result =
                await _printHistoryService.ReadAllAsync();
            _entries = result.Entries;

            skippedRecordsLabel.Visible = result.SkippedRecordCount > 0;
            skippedRecordsLabel.Text =
                $"Pominięte uszkodzone rekordy: {result.SkippedRecordCount}";
            ApplyFilters();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                $"Nie udało się odczytać historii.\r\n\r\n{exception.Message}",
                "Historia wydruków",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void InitializeGridColumns()
    {
        historyDataGridView.Columns.Add(
            CreateTextColumn("localTimeColumn", "Data i czas lokalny", 125));
        historyDataGridView.Columns.Add(
            CreateTextColumn("assetRangeColumn", "Zakres Asset ID", 140));
        historyDataGridView.Columns.Add(
            CreateTextColumn("quantityColumn", "Liczba", 55));
        historyDataGridView.Columns.Add(
            CreateTextColumn("organizationColumn", "Organizacja", 130));
        historyDataGridView.Columns.Add(
            CreateTextColumn("profileColumn", "Profil", 145));
        historyDataGridView.Columns.Add(
            CreateTextColumn("printerColumn", "Drukarka", 160));
        historyDataGridView.Columns.Add(
            CreateTextColumn("companyColumn", "Firma", 160));
    }

    private static DataGridViewTextBoxColumn CreateTextColumn(
        string name,
        string headerText,
        int fillWeight)
    {
        return new DataGridViewTextBoxColumn
        {
            Name = name,
            HeaderText = headerText,
            FillWeight = fillWeight,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.NotSortable
        };
    }

    private void DateFilterCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        UpdateDateFilterControls();
        ApplyFilters();
    }

    private void FilterControl_Changed(object? sender, EventArgs e)
    {
        ApplyFilters();
    }

    private void HistoryDataGridView_SelectionChanged(
        object? sender,
        EventArgs e)
    {
        PrintHistoryEntry? entry = historyDataGridView.SelectedRows.Count > 0
            ? historyDataGridView.SelectedRows[0].Tag as PrintHistoryEntry
            : null;
        detailsTextBox.Text = entry is null
            ? string.Empty
            : FormatDetails(entry);
    }

    private void CloseButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private async void ExportCsvButton_Click(object? sender, EventArgs e)
    {
        if (_visibleEntries.Count == 0)
        {
            MessageBox.Show(
                this,
                "Brak widocznych rekordów do wyeksportowania.",
                "Eksport CSV",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        PrintHistoryEntry[] entriesToExport = _visibleEntries.ToArray();
        using var saveFileDialog = new SaveFileDialog
        {
            AddExtension = true,
            DefaultExt = "csv",
            FileName = $"EtykietyIT_Historia_{DateTime.Now:yyyy-MM-dd_HHmm}.csv",
            Filter = "Pliki CSV (*.csv)|*.csv",
            OverwritePrompt = true,
            Title = "Eksportuj historię do CSV"
        };

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        exportCsvButton.Enabled = false;
        try
        {
            await _historyExporter.ExportAsync(
                entriesToExport,
                saveFileDialog.FileName);
            string fullPath = Path.GetFullPath(saveFileDialog.FileName);

            MessageBox.Show(
                this,
                $"Wyeksportowano rekordów: {entriesToExport.Length}\r\n\r\n{fullPath}",
                "Eksport CSV zakończony",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                $"Nie udało się zapisać pliku CSV.\r\n\r\n{exception.Message}",
                "Błąd eksportu CSV",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            exportCsvButton.Enabled = true;
        }
    }

    private void ApplyFilters()
    {
        string query = searchTextBox.Text;
        DateTime fromDate = dateFromDateTimePicker.Value.Date;
        DateTime toDate = dateToDateTimePicker.Value.Date;

        IEnumerable<PrintHistoryEntry> filteredEntries = _entries.Where(
            entry => PrintHistorySearch.Matches(entry, query));

        if (dateFilterCheckBox.Checked)
        {
            filteredEntries = filteredEntries.Where(entry =>
            {
                DateTime localDate = entry.TimestampUtc.ToLocalTime().Date;
                return localDate >= fromDate && localDate <= toDate;
            });
        }

        _visibleEntries = filteredEntries
            .OrderByDescending(entry => entry.TimestampUtc)
            .ToArray();

        historyDataGridView.Rows.Clear();
        foreach (PrintHistoryEntry entry in _visibleEntries)
        {
            PrintHistorySnapshot snapshot = entry.Snapshot;
            DateTimeOffset localTimestamp = entry.TimestampUtc.ToLocalTime();
            int rowIndex = historyDataGridView.Rows.Add(
                localTimestamp.ToString("g", CultureInfo.CurrentCulture),
                $"{snapshot.FirstAssetId} – {snapshot.LastAssetId}",
                snapshot.SmallLabelQuantity,
                FormatOrganizationName(snapshot.OrganizationProfileName),
                snapshot.ProfileName,
                snapshot.PrinterName,
                snapshot.CompanyName);
            historyDataGridView.Rows[rowIndex].Tag = entry;
        }

        if (historyDataGridView.Rows.Count > 0)
        {
            historyDataGridView.Rows[0].Selected = true;
            historyDataGridView.CurrentCell = historyDataGridView.Rows[0].Cells[0];
        }
        else
        {
            detailsTextBox.Clear();
        }
    }

    private void UpdateDateFilterControls()
    {
        bool enabled = dateFilterCheckBox.Checked;
        dateFromLabel.Enabled = enabled;
        dateFromDateTimePicker.Enabled = enabled;
        dateToLabel.Enabled = enabled;
        dateToDateTimePicker.Enabled = enabled;
    }

    private static string FormatDetails(PrintHistoryEntry entry)
    {
        PrintHistorySnapshot snapshot = entry.Snapshot;
        DateTimeOffset localTimestamp = entry.TimestampUtc.ToLocalTime();
        var details = new StringBuilder();

        details.AppendLine("ZADANIE DRUKOWANIA");
        details.AppendLine($"ID: {entry.Id}");
        details.AppendLine($"Czas lokalny: {localTimestamp:yyyy-MM-dd HH:mm:ss zzz}");
        details.AppendLine($"Czas UTC: {entry.TimestampUtc:O}");
        details.AppendLine($"Wersja aplikacji: {entry.ApplicationVersion}");
        details.AppendLine(
            $"Organizacja: {FormatOrganizationName(snapshot.OrganizationProfileName)}");
        details.AppendLine(
            $"ID organizacji: {FormatOrganizationName(snapshot.OrganizationProfileId)}");
        details.AppendLine();
        details.AppendLine("ASSET ID");
        details.AppendLine($"Zakres: {snapshot.FirstAssetId} – {snapshot.LastAssetId}");
        details.AppendLine($"Numery: {snapshot.StartNumber} – {snapshot.EndNumber}");
        details.AppendLine($"Prefiks: {snapshot.Prefix}");
        details.AppendLine($"Liczba cyfr: {snapshot.Digits}");
        details.AppendLine($"Małe etykiety: {snapshot.SmallLabelQuantity}");
        details.AppendLine($"Fizyczne etykiety: {snapshot.PhysicalLabelQuantity}");
        details.AppendLine();
        details.AppendLine("TREŚĆ I DRUKARKA");
        details.AppendLine($"Firma: {snapshot.CompanyName}");
        details.AppendLine($"Drukarka: {snapshot.PrinterName}");
        details.AppendLine($"Korekta X: {snapshot.OffsetXmm:0.0} mm");
        details.AppendLine($"Korekta Y: {snapshot.OffsetYmm:0.0} mm");
        details.AppendLine();
        details.AppendLine("PROFIL");
        details.AppendLine($"Nazwa: {snapshot.ProfileName}");
        details.AppendLine($"ID: {snapshot.ProfileId}");
        details.AppendLine($"Rozmiar: {snapshot.WidthMm:0.0} × {snapshot.HeightMm:0.0} mm");
        details.AppendLine($"Układ: {snapshot.Columns} × {snapshot.Rows}");
        details.AppendLine($"Linie cięcia: {FormatBoolean(snapshot.DrawCutLines)}");
        details.AppendLine($"QR: {FormatBoolean(snapshot.QrEnabled)}");

        return details.ToString();
    }

    private static string FormatBoolean(bool value)
    {
        return value ? "Tak" : "Nie";
    }

    private static string FormatOrganizationName(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "—" : value;
    }
}
