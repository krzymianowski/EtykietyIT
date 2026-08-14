namespace EtykietyIT.Forms;

partial class ProfileEditForm
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
        nameLabel = new Label();
        nameTextBox = new TextBox();
        widthLabel = new Label();
        widthNumericUpDown = new NumericUpDown();
        heightLabel = new Label();
        heightNumericUpDown = new NumericUpDown();
        columnsLabel = new Label();
        columnsNumericUpDown = new NumericUpDown();
        rowsLabel = new Label();
        rowsNumericUpDown = new NumericUpDown();
        drawCutLinesCheckBox = new CheckBox();
        saveButton = new Button();
        cancelButton = new Button();
        ((System.ComponentModel.ISupportInitialize)widthNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)heightNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)columnsNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)rowsNumericUpDown).BeginInit();
        SuspendLayout();
        //
        // nameLabel
        //
        nameLabel.AutoSize = true;
        nameLabel.Location = new Point(24, 28);
        nameLabel.Name = "nameLabel";
        nameLabel.Size = new Size(43, 15);
        nameLabel.TabIndex = 0;
        nameLabel.Text = "Nazwa:";
        //
        // nameTextBox
        //
        nameTextBox.Location = new Point(161, 25);
        nameTextBox.MaxLength = 200;
        nameTextBox.Name = "nameTextBox";
        nameTextBox.Size = new Size(267, 23);
        nameTextBox.TabIndex = 1;
        //
        // widthLabel
        //
        widthLabel.AutoSize = true;
        widthLabel.Location = new Point(24, 70);
        widthLabel.Name = "widthLabel";
        widthLabel.Size = new Size(89, 15);
        widthLabel.TabIndex = 2;
        widthLabel.Text = "Szerokość [mm]:";
        //
        // widthNumericUpDown
        //
        widthNumericUpDown.DecimalPlaces = 1;
        widthNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        widthNumericUpDown.Location = new Point(161, 67);
        widthNumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        widthNumericUpDown.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
        widthNumericUpDown.Name = "widthNumericUpDown";
        widthNumericUpDown.Size = new Size(130, 23);
        widthNumericUpDown.TabIndex = 3;
        widthNumericUpDown.Value = new decimal(new int[] { 89, 0, 0, 0 });
        //
        // heightLabel
        //
        heightLabel.AutoSize = true;
        heightLabel.Location = new Point(24, 112);
        heightLabel.Name = "heightLabel";
        heightLabel.Size = new Size(91, 15);
        heightLabel.TabIndex = 4;
        heightLabel.Text = "Wysokość [mm]:";
        //
        // heightNumericUpDown
        //
        heightNumericUpDown.DecimalPlaces = 1;
        heightNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        heightNumericUpDown.Location = new Point(161, 109);
        heightNumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        heightNumericUpDown.Minimum = new decimal(new int[] { 15, 0, 0, 0 });
        heightNumericUpDown.Name = "heightNumericUpDown";
        heightNumericUpDown.Size = new Size(130, 23);
        heightNumericUpDown.TabIndex = 5;
        heightNumericUpDown.Value = new decimal(new int[] { 41, 0, 0, 0 });
        //
        // columnsLabel
        //
        columnsLabel.AutoSize = true;
        columnsLabel.Location = new Point(24, 154);
        columnsLabel.Name = "columnsLabel";
        columnsLabel.Size = new Size(56, 15);
        columnsLabel.TabIndex = 6;
        columnsLabel.Text = "Kolumny:";
        //
        // columnsNumericUpDown
        //
        columnsNumericUpDown.Location = new Point(161, 151);
        columnsNumericUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
        columnsNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        columnsNumericUpDown.Name = "columnsNumericUpDown";
        columnsNumericUpDown.Size = new Size(130, 23);
        columnsNumericUpDown.TabIndex = 7;
        columnsNumericUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
        //
        // rowsLabel
        //
        rowsLabel.AutoSize = true;
        rowsLabel.Location = new Point(24, 196);
        rowsLabel.Name = "rowsLabel";
        rowsLabel.Size = new Size(49, 15);
        rowsLabel.TabIndex = 8;
        rowsLabel.Text = "Wiersze:";
        //
        // rowsNumericUpDown
        //
        rowsNumericUpDown.Location = new Point(161, 193);
        rowsNumericUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
        rowsNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        rowsNumericUpDown.Name = "rowsNumericUpDown";
        rowsNumericUpDown.Size = new Size(130, 23);
        rowsNumericUpDown.TabIndex = 9;
        rowsNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
        //
        // drawCutLinesCheckBox
        //
        drawCutLinesCheckBox.AutoSize = true;
        drawCutLinesCheckBox.Location = new Point(161, 235);
        drawCutLinesCheckBox.Name = "drawCutLinesCheckBox";
        drawCutLinesCheckBox.Size = new Size(87, 19);
        drawCutLinesCheckBox.TabIndex = 10;
        drawCutLinesCheckBox.Text = "Linie cięcia";
        drawCutLinesCheckBox.UseVisualStyleBackColor = true;
        //
        // saveButton
        //
        saveButton.Location = new Point(250, 285);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(86, 32);
        saveButton.TabIndex = 11;
        saveButton.Text = "Zapisz";
        saveButton.UseVisualStyleBackColor = true;
        //
        // cancelButton
        //
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(342, 285);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(86, 32);
        cancelButton.TabIndex = 12;
        cancelButton.Text = "Anuluj";
        cancelButton.UseVisualStyleBackColor = true;
        //
        // ProfileEditForm
        //
        AcceptButton = saveButton;
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(452, 341);
        Controls.Add(cancelButton);
        Controls.Add(saveButton);
        Controls.Add(drawCutLinesCheckBox);
        Controls.Add(rowsNumericUpDown);
        Controls.Add(rowsLabel);
        Controls.Add(columnsNumericUpDown);
        Controls.Add(columnsLabel);
        Controls.Add(heightNumericUpDown);
        Controls.Add(heightLabel);
        Controls.Add(widthNumericUpDown);
        Controls.Add(widthLabel);
        Controls.Add(nameTextBox);
        Controls.Add(nameLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ProfileEditForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Profil etykiety";
        ((System.ComponentModel.ISupportInitialize)widthNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)heightNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)columnsNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)rowsNumericUpDown).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label nameLabel;
    private TextBox nameTextBox;
    private Label widthLabel;
    private NumericUpDown widthNumericUpDown;
    private Label heightLabel;
    private NumericUpDown heightNumericUpDown;
    private Label columnsLabel;
    private NumericUpDown columnsNumericUpDown;
    private Label rowsLabel;
    private NumericUpDown rowsNumericUpDown;
    private CheckBox drawCutLinesCheckBox;
    private Button saveButton;
    private Button cancelButton;
}
