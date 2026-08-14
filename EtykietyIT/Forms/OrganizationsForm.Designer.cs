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
        organizationsListView = new ListView();
        nameColumnHeader = new ColumnHeader();
        companyColumnHeader = new ColumnHeader();
        assetIdColumnHeader = new ColumnHeader();
        nextNumberColumnHeader = new ColumnHeader();
        labelProfileColumnHeader = new ColumnHeader();
        printerColumnHeader = new ColumnHeader();
        skippedFilesLabel = new Label();
        newButton = new Button();
        editButton = new Button();
        duplicateButton = new Button();
        deleteButton = new Button();
        closeButton = new Button();
        SuspendLayout();
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
        organizationsListView.FullRowSelect = true;
        organizationsListView.GridLines = true;
        organizationsListView.HideSelection = false;
        organizationsListView.Location = new Point(20, 20);
        organizationsListView.MultiSelect = false;
        organizationsListView.Name = "organizationsListView";
        organizationsListView.Size = new Size(940, 330);
        organizationsListView.TabIndex = 0;
        organizationsListView.UseCompatibleStateImageBehavior = false;
        organizationsListView.View = View.Details;
        //
        // nameColumnHeader
        //
        nameColumnHeader.Text = "Profil organizacji";
        nameColumnHeader.Width = 170;
        //
        // companyColumnHeader
        //
        companyColumnHeader.Text = "Firma na etykiecie";
        companyColumnHeader.Width = 190;
        //
        // assetIdColumnHeader
        //
        assetIdColumnHeader.Text = "Asset ID";
        assetIdColumnHeader.Width = 120;
        //
        // nextNumberColumnHeader
        //
        nextNumberColumnHeader.Text = "Następny numer";
        nextNumberColumnHeader.Width = 105;
        //
        // labelProfileColumnHeader
        //
        labelProfileColumnHeader.Text = "Profil etykiety";
        labelProfileColumnHeader.Width = 170;
        //
        // printerColumnHeader
        //
        printerColumnHeader.Text = "Drukarka";
        printerColumnHeader.Width = 160;
        //
        // skippedFilesLabel
        //
        skippedFilesLabel.AutoSize = true;
        skippedFilesLabel.ForeColor = Color.DarkRed;
        skippedFilesLabel.Location = new Point(20, 366);
        skippedFilesLabel.Name = "skippedFilesLabel";
        skippedFilesLabel.Size = new Size(169, 15);
        skippedFilesLabel.TabIndex = 1;
        skippedFilesLabel.Text = "Pominięte uszkodzone pliki: 0";
        skippedFilesLabel.Visible = false;
        //
        // newButton
        //
        newButton.Location = new Point(20, 398);
        newButton.Name = "newButton";
        newButton.Size = new Size(92, 34);
        newButton.TabIndex = 2;
        newButton.Text = "Nowa";
        newButton.UseVisualStyleBackColor = true;
        //
        // editButton
        //
        editButton.Location = new Point(118, 398);
        editButton.Name = "editButton";
        editButton.Size = new Size(92, 34);
        editButton.TabIndex = 3;
        editButton.Text = "Edytuj";
        editButton.UseVisualStyleBackColor = true;
        //
        // duplicateButton
        //
        duplicateButton.Location = new Point(216, 398);
        duplicateButton.Name = "duplicateButton";
        duplicateButton.Size = new Size(92, 34);
        duplicateButton.TabIndex = 4;
        duplicateButton.Text = "Duplikuj";
        duplicateButton.UseVisualStyleBackColor = true;
        //
        // deleteButton
        //
        deleteButton.Location = new Point(314, 398);
        deleteButton.Name = "deleteButton";
        deleteButton.Size = new Size(92, 34);
        deleteButton.TabIndex = 5;
        deleteButton.Text = "Usuń";
        deleteButton.UseVisualStyleBackColor = true;
        //
        // closeButton
        //
        closeButton.DialogResult = DialogResult.Cancel;
        closeButton.Location = new Point(868, 398);
        closeButton.Name = "closeButton";
        closeButton.Size = new Size(92, 34);
        closeButton.TabIndex = 6;
        closeButton.Text = "Zamknij";
        closeButton.UseVisualStyleBackColor = true;
        //
        // OrganizationsForm
        //
        CancelButton = closeButton;
        ClientSize = new Size(980, 452);
        Controls.Add(closeButton);
        Controls.Add(deleteButton);
        Controls.Add(duplicateButton);
        Controls.Add(editButton);
        Controls.Add(newButton);
        Controls.Add(skippedFilesLabel);
        Controls.Add(organizationsListView);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "OrganizationsForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Profile organizacji";
        ResumeLayout(false);
        PerformLayout();
    }

    private ListView organizationsListView;
    private ColumnHeader nameColumnHeader;
    private ColumnHeader companyColumnHeader;
    private ColumnHeader assetIdColumnHeader;
    private ColumnHeader nextNumberColumnHeader;
    private ColumnHeader labelProfileColumnHeader;
    private ColumnHeader printerColumnHeader;
    private Label skippedFilesLabel;
    private Button newButton;
    private Button editButton;
    private Button duplicateButton;
    private Button deleteButton;
    private Button closeButton;
}
