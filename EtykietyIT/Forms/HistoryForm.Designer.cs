namespace EtykietyIT.Forms;

partial class HistoryForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        searchLabel = new Label();
        searchTextBox = new TextBox();
        dateFilterCheckBox = new CheckBox();
        dateFromLabel = new Label();
        dateFromDateTimePicker = new DateTimePicker();
        dateToLabel = new Label();
        dateToDateTimePicker = new DateTimePicker();
        skippedRecordsLabel = new Label();
        historyDataGridView = new DataGridView();
        detailsGroupBox = new GroupBox();
        detailsTextBox = new TextBox();
        exportCsvButton = new Button();
        closeButton = new Button();
        ((System.ComponentModel.ISupportInitialize)historyDataGridView).BeginInit();
        detailsGroupBox.SuspendLayout();
        SuspendLayout();
        //
        // searchLabel
        //
        searchLabel.AutoSize = true;
        searchLabel.Location = new Point(20, 24);
        searchLabel.Name = "searchLabel";
        searchLabel.Size = new Size(45, 15);
        searchLabel.TabIndex = 0;
        searchLabel.Text = "Szukaj:";
        //
        // searchTextBox
        //
        searchTextBox.Location = new Point(75, 20);
        searchTextBox.Name = "searchTextBox";
        searchTextBox.PlaceholderText = "Asset ID, drukarka, profil lub firma";
        searchTextBox.Size = new Size(315, 23);
        searchTextBox.TabIndex = 1;
        //
        // dateFilterCheckBox
        //
        dateFilterCheckBox.AutoSize = true;
        dateFilterCheckBox.Location = new Point(420, 22);
        dateFilterCheckBox.Name = "dateFilterCheckBox";
        dateFilterCheckBox.Size = new Size(92, 19);
        dateFilterCheckBox.TabIndex = 2;
        dateFilterCheckBox.Text = "Filtruj daty";
        dateFilterCheckBox.UseVisualStyleBackColor = true;
        //
        // dateFromLabel
        //
        dateFromLabel.AutoSize = true;
        dateFromLabel.Location = new Point(527, 24);
        dateFromLabel.Name = "dateFromLabel";
        dateFromLabel.Size = new Size(25, 15);
        dateFromLabel.TabIndex = 3;
        dateFromLabel.Text = "Od:";
        //
        // dateFromDateTimePicker
        //
        dateFromDateTimePicker.Format = DateTimePickerFormat.Short;
        dateFromDateTimePicker.Location = new Point(558, 20);
        dateFromDateTimePicker.Name = "dateFromDateTimePicker";
        dateFromDateTimePicker.Size = new Size(110, 23);
        dateFromDateTimePicker.TabIndex = 4;
        //
        // dateToLabel
        //
        dateToLabel.AutoSize = true;
        dateToLabel.Location = new Point(680, 24);
        dateToLabel.Name = "dateToLabel";
        dateToLabel.Size = new Size(24, 15);
        dateToLabel.TabIndex = 5;
        dateToLabel.Text = "Do:";
        //
        // dateToDateTimePicker
        //
        dateToDateTimePicker.Format = DateTimePickerFormat.Short;
        dateToDateTimePicker.Location = new Point(710, 20);
        dateToDateTimePicker.Name = "dateToDateTimePicker";
        dateToDateTimePicker.Size = new Size(110, 23);
        dateToDateTimePicker.TabIndex = 6;
        //
        // skippedRecordsLabel
        //
        skippedRecordsLabel.AutoSize = true;
        skippedRecordsLabel.ForeColor = Color.DarkRed;
        skippedRecordsLabel.Location = new Point(840, 24);
        skippedRecordsLabel.Name = "skippedRecordsLabel";
        skippedRecordsLabel.Size = new Size(181, 15);
        skippedRecordsLabel.TabIndex = 7;
        skippedRecordsLabel.Text = "Pominięte uszkodzone rekordy: 0";
        skippedRecordsLabel.Visible = false;
        //
        // historyDataGridView
        //
        historyDataGridView.AllowUserToAddRows = false;
        historyDataGridView.AllowUserToDeleteRows = false;
        historyDataGridView.AllowUserToResizeRows = false;
        historyDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        historyDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        historyDataGridView.Location = new Point(20, 60);
        historyDataGridView.MultiSelect = false;
        historyDataGridView.Name = "historyDataGridView";
        historyDataGridView.ReadOnly = true;
        historyDataGridView.RowHeadersVisible = false;
        historyDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        historyDataGridView.Size = new Size(790, 540);
        historyDataGridView.TabIndex = 8;
        //
        // detailsGroupBox
        //
        detailsGroupBox.Controls.Add(detailsTextBox);
        detailsGroupBox.Location = new Point(830, 60);
        detailsGroupBox.Name = "detailsGroupBox";
        detailsGroupBox.Size = new Size(360, 540);
        detailsGroupBox.TabIndex = 9;
        detailsGroupBox.TabStop = false;
        detailsGroupBox.Text = "Szczegóły snapshotu";
        //
        // detailsTextBox
        //
        detailsTextBox.BackColor = SystemColors.Window;
        detailsTextBox.Font = new Font("Consolas", 9F);
        detailsTextBox.Location = new Point(12, 25);
        detailsTextBox.Multiline = true;
        detailsTextBox.Name = "detailsTextBox";
        detailsTextBox.ReadOnly = true;
        detailsTextBox.ScrollBars = ScrollBars.Vertical;
        detailsTextBox.Size = new Size(336, 500);
        detailsTextBox.TabIndex = 0;
        //
        // exportCsvButton
        //
        exportCsvButton.Location = new Point(830, 616);
        exportCsvButton.Name = "exportCsvButton";
        exportCsvButton.Size = new Size(150, 34);
        exportCsvButton.TabIndex = 10;
        exportCsvButton.Text = "Eksportuj CSV...";
        exportCsvButton.UseVisualStyleBackColor = true;
        //
        // closeButton
        //
        closeButton.DialogResult = DialogResult.Cancel;
        closeButton.Location = new Point(1098, 616);
        closeButton.Name = "closeButton";
        closeButton.Size = new Size(92, 34);
        closeButton.TabIndex = 11;
        closeButton.Text = "Zamknij";
        closeButton.UseVisualStyleBackColor = true;
        //
        // HistoryForm
        //
        CancelButton = closeButton;
        ClientSize = new Size(1210, 670);
        Controls.Add(closeButton);
        Controls.Add(exportCsvButton);
        Controls.Add(detailsGroupBox);
        Controls.Add(historyDataGridView);
        Controls.Add(skippedRecordsLabel);
        Controls.Add(dateToDateTimePicker);
        Controls.Add(dateToLabel);
        Controls.Add(dateFromDateTimePicker);
        Controls.Add(dateFromLabel);
        Controls.Add(dateFilterCheckBox);
        Controls.Add(searchTextBox);
        Controls.Add(searchLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "HistoryForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Historia wydruków — Etykiety IT";
        ((System.ComponentModel.ISupportInitialize)historyDataGridView).EndInit();
        detailsGroupBox.ResumeLayout(false);
        detailsGroupBox.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label searchLabel;
    private TextBox searchTextBox;
    private CheckBox dateFilterCheckBox;
    private Label dateFromLabel;
    private DateTimePicker dateFromDateTimePicker;
    private Label dateToLabel;
    private DateTimePicker dateToDateTimePicker;
    private Label skippedRecordsLabel;
    private DataGridView historyDataGridView;
    private GroupBox detailsGroupBox;
    private TextBox detailsTextBox;
    private Button exportCsvButton;
    private Button closeButton;
}
