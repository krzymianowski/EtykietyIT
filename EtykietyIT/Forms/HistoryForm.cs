using System.Globalization;
using System.Text;
using EtykietyIT.Models;
using EtykietyIT.Services;

namespace EtykietyIT.Forms;

public partial class HistoryForm : Form
{
    private readonly PrintHistoryService _printHistoryService;
    private IReadOnlyList<PrintHistoryEntry> _entries =
        Array.Empty<PrintHistoryEntry>();

    public HistoryForm(PrintHistoryService printHistoryService)
    {
        _printHistoryService = printHistoryService ??
            throw new ArgumentNullException(nameof(printHistoryService));

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

        PrintHistoryEntry[] visibleEntries = filteredEntries
            .OrderByDescending(entry => entry.TimestampUtc)
            .ToArray();

        historyDataGridView.Rows.Clear();
        foreach (PrintHistoryEntry entry in visibleEntries)
        {
            PrintHistorySnapshot snapshot = entry.Snapshot;
            DateTimeOffset localTimestamp = entry.TimestampUtc.ToLocalTime();
            int rowIndex = historyDataGridView.Rows.Add(
                localTimestamp.ToString("g", CultureInfo.CurrentCulture),
                $"{snapshot.FirstAssetId} – {snapshot.LastAssetId}",
                snapshot.SmallLabelQuantity,
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
}
