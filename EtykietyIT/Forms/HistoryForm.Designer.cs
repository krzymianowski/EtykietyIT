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
        mainLayoutPanel = new TableLayoutPanel();
        filterPanel = new FlowLayoutPanel();
        searchLabel = new Label();
        searchTextBox = new TextBox();
        dateFilterCheckBox = new CheckBox();
        dateFromLabel = new Label();
        dateFromDateTimePicker = new DateTimePicker();
        dateToLabel = new Label();
        dateToDateTimePicker = new DateTimePicker();
        skippedRecordsLabel = new Label();
        contentSplitContainer = new SplitContainer();
        historyDataGridView = new DataGridView();
        detailsGroupBox = new GroupBox();
        detailsTextBox = new TextBox();
        bottomLayoutPanel = new TableLayoutPanel();
        exportButtonsPanel = new FlowLayoutPanel();
        exportCsvButton = new Button();
        exportXlsxButton = new Button();
        closeButton = new Button();
        mainLayoutPanel.SuspendLayout();
        filterPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)contentSplitContainer).BeginInit();
        contentSplitContainer.Panel1.SuspendLayout();
        contentSplitContainer.Panel2.SuspendLayout();
        contentSplitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)historyDataGridView).BeginInit();
        detailsGroupBox.SuspendLayout();
        bottomLayoutPanel.SuspendLayout();
        exportButtonsPanel.SuspendLayout();
        SuspendLayout();
        //
        // mainLayoutPanel
        //
        mainLayoutPanel.ColumnCount = 1;
        mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayoutPanel.Controls.Add(filterPanel, 0, 0);
        mainLayoutPanel.Controls.Add(contentSplitContainer, 0, 1);
        mainLayoutPanel.Controls.Add(bottomLayoutPanel, 0, 2);
        mainLayoutPanel.Dock = DockStyle.Fill;
        mainLayoutPanel.Padding = new Padding(16);
        mainLayoutPanel.RowCount = 3;
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.TabIndex = 0;
        //
        // filterPanel
        //
        filterPanel.AutoSize = true;
        filterPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        filterPanel.Controls.Add(searchLabel);
        filterPanel.Controls.Add(searchTextBox);
        filterPanel.Controls.Add(dateFilterCheckBox);
        filterPanel.Controls.Add(dateFromLabel);
        filterPanel.Controls.Add(dateFromDateTimePicker);
        filterPanel.Controls.Add(dateToLabel);
        filterPanel.Controls.Add(dateToDateTimePicker);
        filterPanel.Controls.Add(skippedRecordsLabel);
        filterPanel.Dock = DockStyle.Fill;
        filterPanel.Margin = new Padding(0, 0, 0, 12);
        filterPanel.Name = "filterPanel";
        filterPanel.WrapContents = true;
        //
        // searchLabel
        //
        searchLabel.Anchor = AnchorStyles.Left;
        searchLabel.AutoSize = true;
        searchLabel.Margin = new Padding(0, 7, 8, 0);
        searchLabel.Text = "Szukaj:";
        //
        // searchTextBox
        //
        searchTextBox.Margin = new Padding(0, 3, 16, 3);
        searchTextBox.Name = "searchTextBox";
        searchTextBox.PlaceholderText =
            "Asset ID, organizacja, drukarka, profil etykiety lub firma";
        searchTextBox.Size = new Size(330, 23);
        searchTextBox.TabIndex = 0;
        //
        // dateFilterCheckBox
        //
        dateFilterCheckBox.Anchor = AnchorStyles.Left;
        dateFilterCheckBox.AutoSize = true;
        dateFilterCheckBox.Margin = new Padding(0, 6, 14, 0);
        dateFilterCheckBox.Name = "dateFilterCheckBox";
        dateFilterCheckBox.TabIndex = 1;
        dateFilterCheckBox.Text = "Filtruj daty";
        dateFilterCheckBox.UseVisualStyleBackColor = true;
        //
        // dateFromLabel
        //
        dateFromLabel.Anchor = AnchorStyles.Left;
        dateFromLabel.AutoSize = true;
        dateFromLabel.Margin = new Padding(0, 7, 6, 0);
        dateFromLabel.Text = "Od:";
        //
        // dateFromDateTimePicker
        //
        dateFromDateTimePicker.Format = DateTimePickerFormat.Short;
        dateFromDateTimePicker.Margin = new Padding(0, 3, 12, 3);
        dateFromDateTimePicker.Name = "dateFromDateTimePicker";
        dateFromDateTimePicker.Size = new Size(110, 23);
        dateFromDateTimePicker.TabIndex = 2;
        //
        // dateToLabel
        //
        dateToLabel.Anchor = AnchorStyles.Left;
        dateToLabel.AutoSize = true;
        dateToLabel.Margin = new Padding(0, 7, 6, 0);
        dateToLabel.Text = "Do:";
        //
        // dateToDateTimePicker
        //
        dateToDateTimePicker.Format = DateTimePickerFormat.Short;
        dateToDateTimePicker.Margin = new Padding(0, 3, 12, 3);
        dateToDateTimePicker.Name = "dateToDateTimePicker";
        dateToDateTimePicker.Size = new Size(110, 23);
        dateToDateTimePicker.TabIndex = 3;
        //
        // skippedRecordsLabel
        //
        skippedRecordsLabel.Anchor = AnchorStyles.Left;
        skippedRecordsLabel.AutoSize = true;
        skippedRecordsLabel.ForeColor = Color.DarkRed;
        skippedRecordsLabel.Margin = new Padding(0, 7, 0, 0);
        skippedRecordsLabel.Name = "skippedRecordsLabel";
        skippedRecordsLabel.Text = "Pominięte uszkodzone rekordy: 0";
        skippedRecordsLabel.Visible = false;
        //
        // contentSplitContainer
        //
        contentSplitContainer.Dock = DockStyle.Fill;
        contentSplitContainer.FixedPanel = FixedPanel.Panel2;
        contentSplitContainer.Location = new Point(16, 58);
        contentSplitContainer.Name = "contentSplitContainer";
        contentSplitContainer.Panel1.Controls.Add(historyDataGridView);
        contentSplitContainer.Panel1MinSize = 520;
        contentSplitContainer.Panel2.Controls.Add(detailsGroupBox);
        contentSplitContainer.Panel2MinSize = 300;
        contentSplitContainer.Size = new Size(1188, 574);
        contentSplitContainer.SplitterDistance = 820;
        contentSplitContainer.SplitterWidth = 8;
        contentSplitContainer.TabIndex = 1;
        //
        // historyDataGridView
        //
        historyDataGridView.AllowUserToAddRows = false;
        historyDataGridView.AllowUserToDeleteRows = false;
        historyDataGridView.AllowUserToResizeRows = false;
        historyDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        historyDataGridView.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        historyDataGridView.Dock = DockStyle.Fill;
        historyDataGridView.MultiSelect = false;
        historyDataGridView.Name = "historyDataGridView";
        historyDataGridView.ReadOnly = true;
        historyDataGridView.RowHeadersVisible = false;
        historyDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        historyDataGridView.TabIndex = 4;
        //
        // detailsGroupBox
        //
        detailsGroupBox.Controls.Add(detailsTextBox);
        detailsGroupBox.Dock = DockStyle.Fill;
        detailsGroupBox.Name = "detailsGroupBox";
        detailsGroupBox.Padding = new Padding(10, 8, 10, 10);
        detailsGroupBox.TabStop = false;
        detailsGroupBox.Text = "Szczegóły wydruku";
        //
        // detailsTextBox
        //
        detailsTextBox.BackColor = SystemColors.Window;
        detailsTextBox.Dock = DockStyle.Fill;
        detailsTextBox.Font = new Font("Consolas", 9F);
        detailsTextBox.Multiline = true;
        detailsTextBox.Name = "detailsTextBox";
        detailsTextBox.ReadOnly = true;
        detailsTextBox.ScrollBars = ScrollBars.Vertical;
        detailsTextBox.TabIndex = 5;
        //
        // bottomLayoutPanel
        //
        bottomLayoutPanel.AutoSize = true;
        bottomLayoutPanel.ColumnCount = 3;
        bottomLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottomLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        bottomLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottomLayoutPanel.Controls.Add(exportButtonsPanel, 0, 0);
        bottomLayoutPanel.Controls.Add(closeButton, 2, 0);
        bottomLayoutPanel.Dock = DockStyle.Fill;
        bottomLayoutPanel.Margin = new Padding(0);
        bottomLayoutPanel.RowCount = 1;
        bottomLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //
        // exportButtonsPanel
        //
        exportButtonsPanel.AutoSize = true;
        exportButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        exportButtonsPanel.Controls.Add(exportCsvButton);
        exportButtonsPanel.Controls.Add(exportXlsxButton);
        exportButtonsPanel.Anchor = AnchorStyles.Left;
        exportButtonsPanel.Margin = new Padding(0);
        exportButtonsPanel.WrapContents = false;
        //
        // exportCsvButton
        //
        exportCsvButton.Name = "exportCsvButton";
        exportCsvButton.AutoSize = false;
        exportCsvButton.MinimumSize = new Size(150, 34);
        exportCsvButton.Size = new Size(150, 34);
        exportCsvButton.TabIndex = 6;
        exportCsvButton.Text = "Eksportuj CSV...";
        exportCsvButton.UseVisualStyleBackColor = true;
        //
        // exportXlsxButton
        //
        exportXlsxButton.Name = "exportXlsxButton";
        exportXlsxButton.AutoSize = false;
        exportXlsxButton.MinimumSize = new Size(150, 34);
        exportXlsxButton.Size = new Size(150, 34);
        exportXlsxButton.TabIndex = 7;
        exportXlsxButton.Text = "Eksportuj XLSX...";
        exportXlsxButton.UseVisualStyleBackColor = true;
        //
        // closeButton
        //
        closeButton.Anchor = AnchorStyles.Right;
        closeButton.AutoSize = false;
        closeButton.DialogResult = DialogResult.Cancel;
        closeButton.Name = "closeButton";
        closeButton.MinimumSize = new Size(100, 34);
        closeButton.Size = new Size(100, 34);
        closeButton.TabIndex = 8;
        closeButton.Text = "Zamknij";
        closeButton.UseVisualStyleBackColor = true;
        //
        // HistoryForm
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        CancelButton = closeButton;
        ClientSize = new Size(1220, 700);
        Controls.Add(mainLayoutPanel);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(1040, 620);
        Name = "HistoryForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Historia wydruków — Etykiety IT";
        mainLayoutPanel.ResumeLayout(false);
        mainLayoutPanel.PerformLayout();
        filterPanel.ResumeLayout(false);
        filterPanel.PerformLayout();
        contentSplitContainer.Panel1.ResumeLayout(false);
        contentSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)contentSplitContainer).EndInit();
        contentSplitContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)historyDataGridView).EndInit();
        detailsGroupBox.ResumeLayout(false);
        detailsGroupBox.PerformLayout();
        bottomLayoutPanel.ResumeLayout(false);
        bottomLayoutPanel.PerformLayout();
        exportButtonsPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private TableLayoutPanel mainLayoutPanel;
    private FlowLayoutPanel filterPanel;
    private Label searchLabel;
    private TextBox searchTextBox;
    private CheckBox dateFilterCheckBox;
    private Label dateFromLabel;
    private DateTimePicker dateFromDateTimePicker;
    private Label dateToLabel;
    private DateTimePicker dateToDateTimePicker;
    private Label skippedRecordsLabel;
    private SplitContainer contentSplitContainer;
    private DataGridView historyDataGridView;
    private GroupBox detailsGroupBox;
    private TextBox detailsTextBox;
    private TableLayoutPanel bottomLayoutPanel;
    private FlowLayoutPanel exportButtonsPanel;
    private Button exportCsvButton;
    private Button exportXlsxButton;
    private Button closeButton;
}
