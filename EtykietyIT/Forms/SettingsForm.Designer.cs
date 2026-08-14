namespace EtykietyIT.Forms;

partial class SettingsForm
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
        companyNameLabel = new Label();
        companyNameTextBox = new TextBox();
        assetIdPrefixLabel = new Label();
        assetIdPrefixTextBox = new TextBox();
        assetIdDigitsLabel = new Label();
        assetIdDigitsNumericUpDown = new NumericUpDown();
        defaultPrinterLabel = new Label();
        defaultPrinterComboBox = new ComboBox();
        saveButton = new Button();
        cancelButton = new Button();
        ((System.ComponentModel.ISupportInitialize)assetIdDigitsNumericUpDown).BeginInit();
        SuspendLayout();
        //
        // companyNameLabel
        //
        companyNameLabel.AutoSize = true;
        companyNameLabel.Location = new Point(24, 28);
        companyNameLabel.Name = "companyNameLabel";
        companyNameLabel.Size = new Size(76, 15);
        companyNameLabel.TabIndex = 0;
        companyNameLabel.Text = "Nazwa firmy:";
        //
        // companyNameTextBox
        //
        companyNameTextBox.Location = new Point(159, 25);
        companyNameTextBox.MaxLength = 200;
        companyNameTextBox.Name = "companyNameTextBox";
        companyNameTextBox.Size = new Size(285, 23);
        companyNameTextBox.TabIndex = 1;
        //
        // assetIdPrefixLabel
        //
        assetIdPrefixLabel.AutoSize = true;
        assetIdPrefixLabel.Location = new Point(24, 70);
        assetIdPrefixLabel.Name = "assetIdPrefixLabel";
        assetIdPrefixLabel.Size = new Size(93, 15);
        assetIdPrefixLabel.TabIndex = 2;
        assetIdPrefixLabel.Text = "Prefiks Asset ID:";
        //
        // assetIdPrefixTextBox
        //
        assetIdPrefixTextBox.Location = new Point(159, 67);
        assetIdPrefixTextBox.MaxLength = 32;
        assetIdPrefixTextBox.Name = "assetIdPrefixTextBox";
        assetIdPrefixTextBox.Size = new Size(130, 23);
        assetIdPrefixTextBox.TabIndex = 3;
        //
        // assetIdDigitsLabel
        //
        assetIdDigitsLabel.AutoSize = true;
        assetIdDigitsLabel.Location = new Point(24, 112);
        assetIdDigitsLabel.Name = "assetIdDigitsLabel";
        assetIdDigitsLabel.Size = new Size(71, 15);
        assetIdDigitsLabel.TabIndex = 4;
        assetIdDigitsLabel.Text = "Liczba cyfr:";
        //
        // assetIdDigitsNumericUpDown
        //
        assetIdDigitsNumericUpDown.Location = new Point(159, 109);
        assetIdDigitsNumericUpDown.Name = "assetIdDigitsNumericUpDown";
        assetIdDigitsNumericUpDown.Size = new Size(130, 23);
        assetIdDigitsNumericUpDown.TabIndex = 5;
        //
        // defaultPrinterLabel
        //
        defaultPrinterLabel.AutoSize = true;
        defaultPrinterLabel.Location = new Point(24, 154);
        defaultPrinterLabel.Name = "defaultPrinterLabel";
        defaultPrinterLabel.Size = new Size(111, 15);
        defaultPrinterLabel.TabIndex = 6;
        defaultPrinterLabel.Text = "Domyślna drukarka:";
        //
        // defaultPrinterComboBox
        //
        defaultPrinterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        defaultPrinterComboBox.FormattingEnabled = true;
        defaultPrinterComboBox.Location = new Point(159, 151);
        defaultPrinterComboBox.Name = "defaultPrinterComboBox";
        defaultPrinterComboBox.Size = new Size(285, 23);
        defaultPrinterComboBox.TabIndex = 7;
        //
        // saveButton
        //
        saveButton.Location = new Point(266, 207);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(86, 32);
        saveButton.TabIndex = 8;
        saveButton.Text = "Zapisz";
        saveButton.UseVisualStyleBackColor = true;
        //
        // cancelButton
        //
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(358, 207);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(86, 32);
        cancelButton.TabIndex = 9;
        cancelButton.Text = "Anuluj";
        cancelButton.UseVisualStyleBackColor = true;
        //
        // SettingsForm
        //
        AcceptButton = saveButton;
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(468, 263);
        Controls.Add(cancelButton);
        Controls.Add(saveButton);
        Controls.Add(defaultPrinterComboBox);
        Controls.Add(defaultPrinterLabel);
        Controls.Add(assetIdDigitsNumericUpDown);
        Controls.Add(assetIdDigitsLabel);
        Controls.Add(assetIdPrefixTextBox);
        Controls.Add(assetIdPrefixLabel);
        Controls.Add(companyNameTextBox);
        Controls.Add(companyNameLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SettingsForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Ustawienia — Etykiety IT";
        ((System.ComponentModel.ISupportInitialize)assetIdDigitsNumericUpDown).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label companyNameLabel;
    private TextBox companyNameTextBox;
    private Label assetIdPrefixLabel;
    private TextBox assetIdPrefixTextBox;
    private Label assetIdDigitsLabel;
    private NumericUpDown assetIdDigitsNumericUpDown;
    private Label defaultPrinterLabel;
    private ComboBox defaultPrinterComboBox;
    private Button saveButton;
    private Button cancelButton;
}
