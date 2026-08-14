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
        profilesListView = new ListView();
        nameColumnHeader = new ColumnHeader();
        typeColumnHeader = new ColumnHeader();
        sizeColumnHeader = new ColumnHeader();
        layoutColumnHeader = new ColumnHeader();
        newButton = new Button();
        editButton = new Button();
        duplicateButton = new Button();
        deleteButton = new Button();
        closeButton = new Button();
        SuspendLayout();
        //
        // profilesListView
        //
        profilesListView.Columns.AddRange(new ColumnHeader[] {
            nameColumnHeader,
            typeColumnHeader,
            sizeColumnHeader,
            layoutColumnHeader });
        profilesListView.FullRowSelect = true;
        profilesListView.GridLines = true;
        profilesListView.HideSelection = false;
        profilesListView.Location = new Point(20, 20);
        profilesListView.MultiSelect = false;
        profilesListView.Name = "profilesListView";
        profilesListView.Size = new Size(632, 280);
        profilesListView.TabIndex = 0;
        profilesListView.UseCompatibleStateImageBehavior = false;
        profilesListView.View = View.Details;
        //
        // nameColumnHeader
        //
        nameColumnHeader.Text = "Nazwa";
        nameColumnHeader.Width = 270;
        //
        // typeColumnHeader
        //
        typeColumnHeader.Text = "Typ";
        typeColumnHeader.Width = 90;
        //
        // sizeColumnHeader
        //
        sizeColumnHeader.Text = "Rozmiar";
        sizeColumnHeader.Width = 150;
        //
        // layoutColumnHeader
        //
        layoutColumnHeader.Text = "Układ";
        layoutColumnHeader.Width = 80;
        //
        // newButton
        //
        newButton.Location = new Point(20, 318);
        newButton.Name = "newButton";
        newButton.Size = new Size(92, 34);
        newButton.TabIndex = 1;
        newButton.Text = "Nowy";
        newButton.UseVisualStyleBackColor = true;
        //
        // editButton
        //
        editButton.Location = new Point(118, 318);
        editButton.Name = "editButton";
        editButton.Size = new Size(92, 34);
        editButton.TabIndex = 2;
        editButton.Text = "Edytuj";
        editButton.UseVisualStyleBackColor = true;
        //
        // duplicateButton
        //
        duplicateButton.Location = new Point(216, 318);
        duplicateButton.Name = "duplicateButton";
        duplicateButton.Size = new Size(92, 34);
        duplicateButton.TabIndex = 3;
        duplicateButton.Text = "Duplikuj";
        duplicateButton.UseVisualStyleBackColor = true;
        //
        // deleteButton
        //
        deleteButton.Location = new Point(314, 318);
        deleteButton.Name = "deleteButton";
        deleteButton.Size = new Size(92, 34);
        deleteButton.TabIndex = 4;
        deleteButton.Text = "Usuń";
        deleteButton.UseVisualStyleBackColor = true;
        //
        // closeButton
        //
        closeButton.DialogResult = DialogResult.Cancel;
        closeButton.Location = new Point(560, 318);
        closeButton.Name = "closeButton";
        closeButton.Size = new Size(92, 34);
        closeButton.TabIndex = 5;
        closeButton.Text = "Zamknij";
        closeButton.UseVisualStyleBackColor = true;
        //
        // ProfilesForm
        //
        AcceptButton = editButton;
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = closeButton;
        ClientSize = new Size(672, 372);
        Controls.Add(closeButton);
        Controls.Add(deleteButton);
        Controls.Add(duplicateButton);
        Controls.Add(editButton);
        Controls.Add(newButton);
        Controls.Add(profilesListView);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ProfilesForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Profile etykiet";
        ResumeLayout(false);
    }

    private ListView profilesListView;
    private ColumnHeader nameColumnHeader;
    private ColumnHeader typeColumnHeader;
    private ColumnHeader sizeColumnHeader;
    private ColumnHeader layoutColumnHeader;
    private Button newButton;
    private Button editButton;
    private Button duplicateButton;
    private Button deleteButton;
    private Button closeButton;
}
