namespace EtykietyIT.Forms;

partial class OrganizationsForm
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
        organizationsListView = new ListView();
        nameColumnHeader = new ColumnHeader();
        companyColumnHeader = new ColumnHeader();
        assetIdColumnHeader = new ColumnHeader();
        nextNumberColumnHeader = new ColumnHeader();
        labelProfileColumnHeader = new ColumnHeader();
        printerColumnHeader = new ColumnHeader();
        skippedFilesLabel = new Label();
        actionsLayoutPanel = new TableLayoutPanel();
        editButtonsPanel = new FlowLayoutPanel();
        newButton = new Button();
        editButton = new Button();
        duplicateButton = new Button();
        deleteButton = new Button();
        closeButton = new Button();
        mainLayoutPanel.SuspendLayout();
        actionsLayoutPanel.SuspendLayout();
        editButtonsPanel.SuspendLayout();
        SuspendLayout();
        //
        // mainLayoutPanel
        //
        mainLayoutPanel.ColumnCount = 1;
        mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayoutPanel.Controls.Add(organizationsListView, 0, 0);
        mainLayoutPanel.Controls.Add(skippedFilesLabel, 0, 1);
        mainLayoutPanel.Controls.Add(actionsLayoutPanel, 0, 2);
        mainLayoutPanel.Dock = DockStyle.Fill;
        mainLayoutPanel.Padding = new Padding(16);
        mainLayoutPanel.RowCount = 3;
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //
        // organizationsListView
        //
        organizationsListView.Columns.AddRange(new ColumnHeader[] {
            nameColumnHeader,
            companyColumnHeader,
            assetIdColumnHeader,
            nextNumberColumnHeader,
            labelProfileColumnHeader,
            printerColumnHeader });
        organizationsListView.Dock = DockStyle.Fill;
        organizationsListView.FullRowSelect = true;
        organizationsListView.GridLines = true;
        organizationsListView.HideSelection = false;
        organizationsListView.MultiSelect = false;
        organizationsListView.Name = "organizationsListView";
        organizationsListView.TabIndex = 0;
        organizationsListView.UseCompatibleStateImageBehavior = false;
        organizationsListView.View = View.Details;
        //
        // columns
        //
        nameColumnHeader.Text = "Profil organizacji";
        nameColumnHeader.Width = 180;
        companyColumnHeader.Text = "Firma na etykiecie";
        companyColumnHeader.Width = 190;
        assetIdColumnHeader.Text = "Asset ID";
        assetIdColumnHeader.Width = 120;
        nextNumberColumnHeader.Text = "Następny numer";
        nextNumberColumnHeader.Width = 110;
        labelProfileColumnHeader.Text = "Profil etykiety";
        labelProfileColumnHeader.Width = 180;
        printerColumnHeader.Text = "Drukarka";
        printerColumnHeader.Width = 180;
        //
        // skippedFilesLabel
        //
        skippedFilesLabel.AutoSize = true;
        skippedFilesLabel.ForeColor = Color.DarkRed;
        skippedFilesLabel.Margin = new Padding(0, 10, 0, 0);
        skippedFilesLabel.Name = "skippedFilesLabel";
        skippedFilesLabel.Text = "Pominięte uszkodzone pliki: 0";
        skippedFilesLabel.Visible = false;
        //
        // actionsLayoutPanel
        //
        actionsLayoutPanel.AutoSize = true;
        actionsLayoutPanel.ColumnCount = 3;
        actionsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        actionsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        actionsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        actionsLayoutPanel.Controls.Add(editButtonsPanel, 0, 0);
        actionsLayoutPanel.Controls.Add(closeButton, 2, 0);
        actionsLayoutPanel.Dock = DockStyle.Fill;
        actionsLayoutPanel.Margin = new Padding(0);
        actionsLayoutPanel.RowCount = 1;
        actionsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //
        // editButtonsPanel
        //
        editButtonsPanel.AutoSize = true;
        editButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        editButtonsPanel.Controls.Add(newButton);
        editButtonsPanel.Controls.Add(editButton);
        editButtonsPanel.Controls.Add(duplicateButton);
        editButtonsPanel.Controls.Add(deleteButton);
        editButtonsPanel.Anchor = AnchorStyles.Left;
        editButtonsPanel.Margin = new Padding(0);
        editButtonsPanel.WrapContents = false;
        //
        // action buttons
        //
        newButton.Name = "newButton";
        newButton.AutoSize = false;
        newButton.MinimumSize = new Size(96, 34);
        newButton.Size = new Size(96, 34);
        newButton.TabIndex = 1;
        newButton.Text = "Nowy";
        newButton.UseVisualStyleBackColor = true;
        editButton.Name = "editButton";
        editButton.AutoSize = false;
        editButton.MinimumSize = new Size(96, 34);
        editButton.Size = new Size(96, 34);
        editButton.TabIndex = 2;
        editButton.Text = "Edytuj";
        editButton.UseVisualStyleBackColor = true;
        duplicateButton.Name = "duplicateButton";
        duplicateButton.AutoSize = false;
        duplicateButton.MinimumSize = new Size(96, 34);
        duplicateButton.Size = new Size(96, 34);
        duplicateButton.TabIndex = 3;
        duplicateButton.Text = "Duplikuj";
        duplicateButton.UseVisualStyleBackColor = true;
        deleteButton.Name = "deleteButton";
        deleteButton.AutoSize = false;
        deleteButton.MinimumSize = new Size(96, 34);
        deleteButton.Size = new Size(96, 34);
        deleteButton.TabIndex = 4;
        deleteButton.Text = "Usuń";
        deleteButton.UseVisualStyleBackColor = true;
        closeButton.Anchor = AnchorStyles.Right;
        closeButton.AutoSize = false;
        closeButton.DialogResult = DialogResult.Cancel;
        closeButton.Name = "closeButton";
        closeButton.MinimumSize = new Size(100, 34);
        closeButton.Size = new Size(100, 34);
        closeButton.TabIndex = 5;
        closeButton.Text = "Zamknij";
        closeButton.UseVisualStyleBackColor = true;
        //
        // OrganizationsForm
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        CancelButton = closeButton;
        ClientSize = new Size(1060, 500);
        Controls.Add(mainLayoutPanel);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(900, 420);
        Name = "OrganizationsForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Profile organizacji";
        mainLayoutPanel.ResumeLayout(false);
        mainLayoutPanel.PerformLayout();
        actionsLayoutPanel.ResumeLayout(false);
        actionsLayoutPanel.PerformLayout();
        editButtonsPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private TableLayoutPanel mainLayoutPanel;
    private ListView organizationsListView;
    private ColumnHeader nameColumnHeader;
    private ColumnHeader companyColumnHeader;
    private ColumnHeader assetIdColumnHeader;
    private ColumnHeader nextNumberColumnHeader;
    private ColumnHeader labelProfileColumnHeader;
    private ColumnHeader printerColumnHeader;
    private Label skippedFilesLabel;
    private TableLayoutPanel actionsLayoutPanel;
    private FlowLayoutPanel editButtonsPanel;
    private Button newButton;
    private Button editButton;
    private Button duplicateButton;
    private Button deleteButton;
    private Button closeButton;
}
