namespace EtykietyIT.Forms;

partial class ProfilesForm
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
        profilesListView = new ListView();
        nameColumnHeader = new ColumnHeader();
        typeColumnHeader = new ColumnHeader();
        sizeColumnHeader = new ColumnHeader();
        layoutColumnHeader = new ColumnHeader();
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
        mainLayoutPanel.Controls.Add(profilesListView, 0, 0);
        mainLayoutPanel.Controls.Add(actionsLayoutPanel, 0, 1);
        mainLayoutPanel.Dock = DockStyle.Fill;
        mainLayoutPanel.Padding = new Padding(16);
        mainLayoutPanel.RowCount = 2;
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //
        // profilesListView
        //
        profilesListView.Columns.AddRange(new ColumnHeader[] {
            nameColumnHeader,
            typeColumnHeader,
            sizeColumnHeader,
            layoutColumnHeader });
        profilesListView.Dock = DockStyle.Fill;
        profilesListView.FullRowSelect = true;
        profilesListView.GridLines = true;
        profilesListView.HideSelection = false;
        profilesListView.MultiSelect = false;
        profilesListView.Name = "profilesListView";
        profilesListView.TabIndex = 0;
        profilesListView.UseCompatibleStateImageBehavior = false;
        profilesListView.View = View.Details;
        //
        // columns
        //
        nameColumnHeader.Text = "Nazwa profilu etykiety";
        nameColumnHeader.Width = 320;
        typeColumnHeader.Text = "Typ";
        typeColumnHeader.Width = 100;
        sizeColumnHeader.Text = "Rozmiar";
        sizeColumnHeader.Width = 150;
        layoutColumnHeader.Text = "Układ";
        layoutColumnHeader.Width = 90;
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
        // ProfilesForm
        //
        AcceptButton = editButton;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        CancelButton = closeButton;
        ClientSize = new Size(720, 430);
        Controls.Add(mainLayoutPanel);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(620, 360);
        Name = "ProfilesForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Profile etykiet";
        mainLayoutPanel.ResumeLayout(false);
        mainLayoutPanel.PerformLayout();
        actionsLayoutPanel.ResumeLayout(false);
        actionsLayoutPanel.PerformLayout();
        editButtonsPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private TableLayoutPanel mainLayoutPanel;
    private ListView profilesListView;
    private ColumnHeader nameColumnHeader;
    private ColumnHeader typeColumnHeader;
    private ColumnHeader sizeColumnHeader;
    private ColumnHeader layoutColumnHeader;
    private TableLayoutPanel actionsLayoutPanel;
    private FlowLayoutPanel editButtonsPanel;
    private Button newButton;
    private Button editButton;
    private Button duplicateButton;
    private Button deleteButton;
    private Button closeButton;
}
