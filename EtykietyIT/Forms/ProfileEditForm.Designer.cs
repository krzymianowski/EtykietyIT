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
        mainLayoutPanel = new TableLayoutPanel();
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
        cellSizeLabel = new Label();
        buttonsPanel = new FlowLayoutPanel();
        saveButton = new Button();
        cancelButton = new Button();
        mainLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)widthNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)heightNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)columnsNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)rowsNumericUpDown).BeginInit();
        buttonsPanel.SuspendLayout();
        SuspendLayout();
        //
        // mainLayoutPanel
        //
        mainLayoutPanel.ColumnCount = 2;
        mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayoutPanel.Controls.Add(nameLabel, 0, 0);
        mainLayoutPanel.Controls.Add(nameTextBox, 1, 0);
        mainLayoutPanel.Controls.Add(widthLabel, 0, 1);
        mainLayoutPanel.Controls.Add(widthNumericUpDown, 1, 1);
        mainLayoutPanel.Controls.Add(heightLabel, 0, 2);
        mainLayoutPanel.Controls.Add(heightNumericUpDown, 1, 2);
        mainLayoutPanel.Controls.Add(columnsLabel, 0, 3);
        mainLayoutPanel.Controls.Add(columnsNumericUpDown, 1, 3);
        mainLayoutPanel.Controls.Add(rowsLabel, 0, 4);
        mainLayoutPanel.Controls.Add(rowsNumericUpDown, 1, 4);
        mainLayoutPanel.Controls.Add(drawCutLinesCheckBox, 1, 5);
        mainLayoutPanel.Controls.Add(cellSizeLabel, 0, 6);
        mainLayoutPanel.SetColumnSpan(cellSizeLabel, 2);
        mainLayoutPanel.Controls.Add(buttonsPanel, 0, 8);
        mainLayoutPanel.SetColumnSpan(buttonsPanel, 2);
        mainLayoutPanel.Dock = DockStyle.Fill;
        mainLayoutPanel.Padding = new Padding(20);
        mainLayoutPanel.RowCount = 9;
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //
        // labels
        //
        nameLabel.Anchor = AnchorStyles.Left;
        nameLabel.AutoSize = true;
        nameLabel.Margin = new Padding(0, 7, 18, 13);
        nameLabel.Text = "Nazwa:";
        widthLabel.Anchor = AnchorStyles.Left;
        widthLabel.AutoSize = true;
        widthLabel.Margin = new Padding(0, 7, 18, 13);
        widthLabel.Text = "Szerokość [mm]:";
        heightLabel.Anchor = AnchorStyles.Left;
        heightLabel.AutoSize = true;
        heightLabel.Margin = new Padding(0, 7, 18, 13);
        heightLabel.Text = "Wysokość [mm]:";
        columnsLabel.Anchor = AnchorStyles.Left;
        columnsLabel.AutoSize = true;
        columnsLabel.Margin = new Padding(0, 7, 18, 13);
        columnsLabel.Text = "Kolumny:";
        rowsLabel.Anchor = AnchorStyles.Left;
        rowsLabel.AutoSize = true;
        rowsLabel.Margin = new Padding(0, 7, 18, 13);
        rowsLabel.Text = "Wiersze:";
        //
        // nameTextBox
        //
        nameTextBox.Dock = DockStyle.Fill;
        nameTextBox.Margin = new Padding(0, 3, 0, 10);
        nameTextBox.MaxLength = 200;
        nameTextBox.Name = "nameTextBox";
        nameTextBox.TabIndex = 0;
        //
        // widthNumericUpDown
        //
        widthNumericUpDown.DecimalPlaces = 1;
        widthNumericUpDown.Dock = DockStyle.Fill;
        widthNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        widthNumericUpDown.Margin = new Padding(0, 3, 0, 10);
        widthNumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        widthNumericUpDown.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
        widthNumericUpDown.Name = "widthNumericUpDown";
        widthNumericUpDown.TabIndex = 1;
        widthNumericUpDown.Value = new decimal(new int[] { 89, 0, 0, 0 });
        //
        // heightNumericUpDown
        //
        heightNumericUpDown.DecimalPlaces = 1;
        heightNumericUpDown.Dock = DockStyle.Fill;
        heightNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        heightNumericUpDown.Margin = new Padding(0, 3, 0, 10);
        heightNumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        heightNumericUpDown.Minimum = new decimal(new int[] { 15, 0, 0, 0 });
        heightNumericUpDown.Name = "heightNumericUpDown";
        heightNumericUpDown.TabIndex = 2;
        heightNumericUpDown.Value = new decimal(new int[] { 41, 0, 0, 0 });
        //
        // columnsNumericUpDown
        //
        columnsNumericUpDown.Dock = DockStyle.Fill;
        columnsNumericUpDown.Margin = new Padding(0, 3, 0, 10);
        columnsNumericUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
        columnsNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        columnsNumericUpDown.Name = "columnsNumericUpDown";
        columnsNumericUpDown.TabIndex = 3;
        columnsNumericUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
        //
        // rowsNumericUpDown
        //
        rowsNumericUpDown.Dock = DockStyle.Fill;
        rowsNumericUpDown.Margin = new Padding(0, 3, 0, 10);
        rowsNumericUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
        rowsNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        rowsNumericUpDown.Name = "rowsNumericUpDown";
        rowsNumericUpDown.TabIndex = 4;
        rowsNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
        //
        // drawCutLinesCheckBox
        //
        drawCutLinesCheckBox.Anchor = AnchorStyles.Left;
        drawCutLinesCheckBox.AutoSize = true;
        drawCutLinesCheckBox.Margin = new Padding(0, 4, 0, 10);
        drawCutLinesCheckBox.Name = "drawCutLinesCheckBox";
        drawCutLinesCheckBox.TabIndex = 5;
        drawCutLinesCheckBox.Text = "Linie cięcia";
        drawCutLinesCheckBox.UseVisualStyleBackColor = true;
        //
        // cellSizeLabel
        //
        cellSizeLabel.Anchor = AnchorStyles.Left;
        cellSizeLabel.AutoSize = true;
        cellSizeLabel.Margin = new Padding(0, 4, 0, 10);
        cellSizeLabel.Name = "cellSizeLabel";
        cellSizeLabel.TabIndex = 6;
        cellSizeLabel.Text = "Rozmiar pojedynczej etykiety: —";
        //
        // buttonsPanel
        //
        buttonsPanel.AutoSize = true;
        buttonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Controls.Add(saveButton);
        buttonsPanel.Dock = DockStyle.Fill;
        buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonsPanel.Margin = new Padding(0, 12, 0, 0);
        buttonsPanel.WrapContents = false;
        //
        // saveButton
        //
        saveButton.AutoSize = false;
        saveButton.MinimumSize = new Size(96, 34);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(96, 34);
        saveButton.TabIndex = 6;
        saveButton.Text = "Zapisz";
        saveButton.UseVisualStyleBackColor = true;
        //
        // cancelButton
        //
        cancelButton.AutoSize = false;
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.MinimumSize = new Size(96, 34);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(96, 34);
        cancelButton.TabIndex = 7;
        cancelButton.Text = "Anuluj";
        cancelButton.UseVisualStyleBackColor = true;
        //
        // ProfileEditForm
        //
        AcceptButton = saveButton;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        CancelButton = cancelButton;
        ClientSize = new Size(500, 390);
        Controls.Add(mainLayoutPanel);
        Font = new Font("Segoe UI", 9F);
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(460, 360);
        Name = "ProfileEditForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Profil etykiety";
        mainLayoutPanel.ResumeLayout(false);
        mainLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)widthNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)heightNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)columnsNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)rowsNumericUpDown).EndInit();
        buttonsPanel.ResumeLayout(false);
        buttonsPanel.PerformLayout();
        ResumeLayout(false);
    }

    private TableLayoutPanel mainLayoutPanel;
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
    private Label cellSizeLabel;
    private FlowLayoutPanel buttonsPanel;
    private Button saveButton;
    private Button cancelButton;
}
