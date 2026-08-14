namespace EtykietyIT.Forms;

partial class OrganizationEditForm
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
        organizationNameLabel = new Label();
        organizationNameTextBox = new TextBox();
        companyNameLabel = new Label();
        companyNameTextBox = new TextBox();
        assetIdPrefixLabel = new Label();
        assetIdPrefixTextBox = new TextBox();
        assetIdDigitsLabel = new Label();
        assetIdDigitsNumericUpDown = new NumericUpDown();
        nextAssetNumberLabel = new Label();
        nextAssetNumberNumericUpDown = new NumericUpDown();
        defaultLabelProfileLabel = new Label();
        defaultLabelProfileComboBox = new ComboBox();
        defaultPrinterLabel = new Label();
        defaultPrinterComboBox = new ComboBox();
        saveButton = new Button();
        cancelButton = new Button();
        ((System.ComponentModel.ISupportInitialize)assetIdDigitsNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nextAssetNumberNumericUpDown).BeginInit();
        SuspendLayout();
        //
        // organizationNameLabel
        //
        organizationNameLabel.AutoSize = true;
        organizationNameLabel.Location = new Point(24, 28);
        organizationNameLabel.Name = "organizationNameLabel";
        organizationNameLabel.Size = new Size(146, 15);
        organizationNameLabel.TabIndex = 0;
        organizationNameLabel.Text = "Nazwa profilu organizacji:";
        //
        // organizationNameTextBox
        //
        organizationNameTextBox.Location = new Point(205, 25);
        organizationNameTextBox.MaxLength = 200;
        organizationNameTextBox.Name = "organizationNameTextBox";
        organizationNameTextBox.Size = new Size(325, 23);
        organizationNameTextBox.TabIndex = 1;
        //
        // companyNameLabel
        //
        companyNameLabel.AutoSize = true;
        companyNameLabel.Location = new Point(24, 70);
        companyNameLabel.Name = "companyNameLabel";
        companyNameLabel.Size = new Size(132, 15);
        companyNameLabel.TabIndex = 2;
        companyNameLabel.Text = "Nazwa firmy na etykiecie:";
        //
        // companyNameTextBox
        //
        companyNameTextBox.Location = new Point(205, 67);
        companyNameTextBox.MaxLength = 200;
        companyNameTextBox.Name = "companyNameTextBox";
        companyNameTextBox.Size = new Size(325, 23);
        companyNameTextBox.TabIndex = 3;
        //
        // assetIdPrefixLabel
        //
        assetIdPrefixLabel.AutoSize = true;
        assetIdPrefixLabel.Location = new Point(24, 112);
        assetIdPrefixLabel.Name = "assetIdPrefixLabel";
        assetIdPrefixLabel.Size = new Size(93, 15);
        assetIdPrefixLabel.TabIndex = 4;
        assetIdPrefixLabel.Text = "Prefiks Asset ID:";
        //
        // assetIdPrefixTextBox
        //
        assetIdPrefixTextBox.Location = new Point(205, 109);
        assetIdPrefixTextBox.MaxLength = 32;
        assetIdPrefixTextBox.Name = "assetIdPrefixTextBox";
        assetIdPrefixTextBox.Size = new Size(130, 23);
        assetIdPrefixTextBox.TabIndex = 5;
        //
        // assetIdDigitsLabel
        //
        assetIdDigitsLabel.AutoSize = true;
        assetIdDigitsLabel.Location = new Point(24, 154);
        assetIdDigitsLabel.Name = "assetIdDigitsLabel";
        assetIdDigitsLabel.Size = new Size(71, 15);
        assetIdDigitsLabel.TabIndex = 6;
        assetIdDigitsLabel.Text = "Liczba cyfr:";
        //
        // assetIdDigitsNumericUpDown
        //
        assetIdDigitsNumericUpDown.Location = new Point(205, 151);
        assetIdDigitsNumericUpDown.Name = "assetIdDigitsNumericUpDown";
        assetIdDigitsNumericUpDown.Size = new Size(130, 23);
        assetIdDigitsNumericUpDown.TabIndex = 7;
        //
        // nextAssetNumberLabel
        //
        nextAssetNumberLabel.AutoSize = true;
        nextAssetNumberLabel.Location = new Point(24, 196);
        nextAssetNumberLabel.Name = "nextAssetNumberLabel";
        nextAssetNumberLabel.Size = new Size(142, 15);
        nextAssetNumberLabel.TabIndex = 8;
        nextAssetNumberLabel.Text = "Następny numer Asset ID:";
        //
        // nextAssetNumberNumericUpDown
        //
        nextAssetNumberNumericUpDown.Location = new Point(205, 193);
        nextAssetNumberNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        nextAssetNumberNumericUpDown.Name = "nextAssetNumberNumericUpDown";
        nextAssetNumberNumericUpDown.Size = new Size(130, 23);
        nextAssetNumberNumericUpDown.TabIndex = 9;
        //
        // defaultLabelProfileLabel
        //
        defaultLabelProfileLabel.AutoSize = true;
        defaultLabelProfileLabel.Location = new Point(24, 238);
        defaultLabelProfileLabel.Name = "defaultLabelProfileLabel";
        defaultLabelProfileLabel.Size = new Size(135, 15);
        defaultLabelProfileLabel.TabIndex = 10;
        defaultLabelProfileLabel.Text = "Domyślny profil etykiety:";
        //
        // defaultLabelProfileComboBox
        //
        defaultLabelProfileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        defaultLabelProfileComboBox.FormattingEnabled = true;
        defaultLabelProfileComboBox.Location = new Point(205, 235);
        defaultLabelProfileComboBox.Name = "defaultLabelProfileComboBox";
        defaultLabelProfileComboBox.Size = new Size(325, 23);
        defaultLabelProfileComboBox.TabIndex = 11;
        //
        // defaultPrinterLabel
        //
        defaultPrinterLabel.AutoSize = true;
        defaultPrinterLabel.Location = new Point(24, 280);
        defaultPrinterLabel.Name = "defaultPrinterLabel";
        defaultPrinterLabel.Size = new Size(111, 15);
        defaultPrinterLabel.TabIndex = 12;
        defaultPrinterLabel.Text = "Domyślna drukarka:";
        //
        // defaultPrinterComboBox
        //
        defaultPrinterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        defaultPrinterComboBox.FormattingEnabled = true;
        defaultPrinterComboBox.Location = new Point(205, 277);
        defaultPrinterComboBox.Name = "defaultPrinterComboBox";
        defaultPrinterComboBox.Size = new Size(325, 23);
        defaultPrinterComboBox.TabIndex = 13;
        //
        // saveButton
        //
        saveButton.Location = new Point(352, 332);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(86, 32);
        saveButton.TabIndex = 14;
        saveButton.Text = "Zapisz";
        saveButton.UseVisualStyleBackColor = true;
        //
        // cancelButton
        //
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(444, 332);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(86, 32);
        cancelButton.TabIndex = 15;
        cancelButton.Text = "Anuluj";
        cancelButton.UseVisualStyleBackColor = true;
        //
        // OrganizationEditForm
        //
        AcceptButton = saveButton;
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(554, 388);
        Controls.Add(cancelButton);
        Controls.Add(saveButton);
        Controls.Add(defaultPrinterComboBox);
        Controls.Add(defaultPrinterLabel);
        Controls.Add(defaultLabelProfileComboBox);
        Controls.Add(defaultLabelProfileLabel);
        Controls.Add(nextAssetNumberNumericUpDown);
        Controls.Add(nextAssetNumberLabel);
        Controls.Add(assetIdDigitsNumericUpDown);
        Controls.Add(assetIdDigitsLabel);
        Controls.Add(assetIdPrefixTextBox);
        Controls.Add(assetIdPrefixLabel);
        Controls.Add(companyNameTextBox);
        Controls.Add(companyNameLabel);
        Controls.Add(organizationNameTextBox);
        Controls.Add(organizationNameLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "OrganizationEditForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Profil organizacji";
        ((System.ComponentModel.ISupportInitialize)assetIdDigitsNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)nextAssetNumberNumericUpDown).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label organizationNameLabel;
    private TextBox organizationNameTextBox;
    private Label companyNameLabel;
    private TextBox companyNameTextBox;
    private Label assetIdPrefixLabel;
    private TextBox assetIdPrefixTextBox;
    private Label assetIdDigitsLabel;
    private NumericUpDown assetIdDigitsNumericUpDown;
    private Label nextAssetNumberLabel;
    private NumericUpDown nextAssetNumberNumericUpDown;
    private Label defaultLabelProfileLabel;
    private ComboBox defaultLabelProfileComboBox;
    private Label defaultPrinterLabel;
    private ComboBox defaultPrinterComboBox;
    private Button saveButton;
    private Button cancelButton;
}
