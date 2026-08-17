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
        mainLayoutPanel = new TableLayoutPanel();
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
        defaultQrCheckBox = new CheckBox();
        buttonsPanel = new FlowLayoutPanel();
        saveButton = new Button();
        cancelButton = new Button();
        mainLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)assetIdDigitsNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nextAssetNumberNumericUpDown).BeginInit();
        buttonsPanel.SuspendLayout();
        SuspendLayout();
        //
        // mainLayoutPanel
        //
        mainLayoutPanel.ColumnCount = 2;
        mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayoutPanel.Controls.Add(organizationNameLabel, 0, 0);
        mainLayoutPanel.Controls.Add(organizationNameTextBox, 1, 0);
        mainLayoutPanel.Controls.Add(companyNameLabel, 0, 1);
        mainLayoutPanel.Controls.Add(companyNameTextBox, 1, 1);
        mainLayoutPanel.Controls.Add(assetIdPrefixLabel, 0, 2);
        mainLayoutPanel.Controls.Add(assetIdPrefixTextBox, 1, 2);
        mainLayoutPanel.Controls.Add(assetIdDigitsLabel, 0, 3);
        mainLayoutPanel.Controls.Add(assetIdDigitsNumericUpDown, 1, 3);
        mainLayoutPanel.Controls.Add(nextAssetNumberLabel, 0, 4);
        mainLayoutPanel.Controls.Add(nextAssetNumberNumericUpDown, 1, 4);
        mainLayoutPanel.Controls.Add(defaultLabelProfileLabel, 0, 5);
        mainLayoutPanel.Controls.Add(defaultLabelProfileComboBox, 1, 5);
        mainLayoutPanel.Controls.Add(defaultPrinterLabel, 0, 6);
        mainLayoutPanel.Controls.Add(defaultPrinterComboBox, 1, 6);
        mainLayoutPanel.Controls.Add(defaultQrCheckBox, 1, 7);
        mainLayoutPanel.Controls.Add(buttonsPanel, 0, 9);
        mainLayoutPanel.SetColumnSpan(buttonsPanel, 2);
        mainLayoutPanel.Dock = DockStyle.Fill;
        mainLayoutPanel.Padding = new Padding(20);
        mainLayoutPanel.RowCount = 10;
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
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
        organizationNameLabel.Anchor = AnchorStyles.Left;
        organizationNameLabel.AutoSize = true;
        organizationNameLabel.Margin = new Padding(0, 7, 18, 13);
        organizationNameLabel.Text = "Nazwa profilu organizacji:";
        companyNameLabel.Anchor = AnchorStyles.Left;
        companyNameLabel.AutoSize = true;
        companyNameLabel.Margin = new Padding(0, 7, 18, 13);
        companyNameLabel.Text = "Nazwa firmy na etykiecie:";
        assetIdPrefixLabel.Anchor = AnchorStyles.Left;
        assetIdPrefixLabel.AutoSize = true;
        assetIdPrefixLabel.Margin = new Padding(0, 7, 18, 13);
        assetIdPrefixLabel.Text = "Prefiks Asset ID:";
        assetIdDigitsLabel.Anchor = AnchorStyles.Left;
        assetIdDigitsLabel.AutoSize = true;
        assetIdDigitsLabel.Margin = new Padding(0, 7, 18, 13);
        assetIdDigitsLabel.Text = "Liczba cyfr:";
        nextAssetNumberLabel.Anchor = AnchorStyles.Left;
        nextAssetNumberLabel.AutoSize = true;
        nextAssetNumberLabel.Margin = new Padding(0, 7, 18, 13);
        nextAssetNumberLabel.Text = "Następny numer Asset ID:";
        defaultLabelProfileLabel.Anchor = AnchorStyles.Left;
        defaultLabelProfileLabel.AutoSize = true;
        defaultLabelProfileLabel.Margin = new Padding(0, 7, 18, 13);
        defaultLabelProfileLabel.Text = "Domyślny profil etykiety:";
        defaultPrinterLabel.Anchor = AnchorStyles.Left;
        defaultPrinterLabel.AutoSize = true;
        defaultPrinterLabel.Margin = new Padding(0, 7, 18, 13);
        defaultPrinterLabel.Text = "Domyślna drukarka:";
        //
        // organizationNameTextBox
        //
        organizationNameTextBox.Dock = DockStyle.Fill;
        organizationNameTextBox.Margin = new Padding(0, 3, 0, 10);
        organizationNameTextBox.MaxLength = 200;
        organizationNameTextBox.Name = "organizationNameTextBox";
        organizationNameTextBox.TabIndex = 0;
        //
        // companyNameTextBox
        //
        companyNameTextBox.Dock = DockStyle.Fill;
        companyNameTextBox.Margin = new Padding(0, 3, 0, 10);
        companyNameTextBox.MaxLength = 200;
        companyNameTextBox.Name = "companyNameTextBox";
        companyNameTextBox.TabIndex = 1;
        //
        // assetIdPrefixTextBox
        //
        assetIdPrefixTextBox.Dock = DockStyle.Fill;
        assetIdPrefixTextBox.Margin = new Padding(0, 3, 0, 10);
        assetIdPrefixTextBox.MaxLength = 32;
        assetIdPrefixTextBox.Name = "assetIdPrefixTextBox";
        assetIdPrefixTextBox.TabIndex = 2;
        //
        // assetIdDigitsNumericUpDown
        //
        assetIdDigitsNumericUpDown.Dock = DockStyle.Fill;
        assetIdDigitsNumericUpDown.Margin = new Padding(0, 3, 0, 10);
        assetIdDigitsNumericUpDown.Name = "assetIdDigitsNumericUpDown";
        assetIdDigitsNumericUpDown.TabIndex = 3;
        //
        // nextAssetNumberNumericUpDown
        //
        nextAssetNumberNumericUpDown.Dock = DockStyle.Fill;
        nextAssetNumberNumericUpDown.Margin = new Padding(0, 3, 0, 10);
        nextAssetNumberNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        nextAssetNumberNumericUpDown.Name = "nextAssetNumberNumericUpDown";
        nextAssetNumberNumericUpDown.TabIndex = 4;
        //
        // defaultLabelProfileComboBox
        //
        defaultLabelProfileComboBox.Dock = DockStyle.Fill;
        defaultLabelProfileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        defaultLabelProfileComboBox.FormattingEnabled = true;
        defaultLabelProfileComboBox.Margin = new Padding(0, 3, 0, 10);
        defaultLabelProfileComboBox.Name = "defaultLabelProfileComboBox";
        defaultLabelProfileComboBox.TabIndex = 5;
        //
        // defaultPrinterComboBox
        //
        defaultPrinterComboBox.Dock = DockStyle.Fill;
        defaultPrinterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        defaultPrinterComboBox.FormattingEnabled = true;
        defaultPrinterComboBox.Margin = new Padding(0, 3, 0, 10);
        defaultPrinterComboBox.Name = "defaultPrinterComboBox";
        defaultPrinterComboBox.TabIndex = 6;
        //
        // defaultQrCheckBox
        //
        defaultQrCheckBox.Anchor = AnchorStyles.Left;
        defaultQrCheckBox.AutoSize = true;
        defaultQrCheckBox.Margin = new Padding(0, 3, 0, 10);
        defaultQrCheckBox.Name = "defaultQrCheckBox";
        defaultQrCheckBox.TabIndex = 7;
        defaultQrCheckBox.Text = "Domyślnie drukuj QR z Asset ID";
        defaultQrCheckBox.UseVisualStyleBackColor = true;
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
        saveButton.TabIndex = 7;
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
        cancelButton.TabIndex = 8;
        cancelButton.Text = "Anuluj";
        cancelButton.UseVisualStyleBackColor = true;
        //
        // OrganizationEditForm
        //
        AcceptButton = saveButton;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        CancelButton = cancelButton;
        ClientSize = new Size(620, 450);
        Controls.Add(mainLayoutPanel);
        Font = new Font("Segoe UI", 9F);
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(560, 430);
        Name = "OrganizationEditForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Profil organizacji";
        mainLayoutPanel.ResumeLayout(false);
        mainLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)assetIdDigitsNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)nextAssetNumberNumericUpDown).EndInit();
        buttonsPanel.ResumeLayout(false);
        buttonsPanel.PerformLayout();
        ResumeLayout(false);
    }

    private TableLayoutPanel mainLayoutPanel;
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
    private CheckBox defaultQrCheckBox;
    private FlowLayoutPanel buttonsPanel;
    private Button saveButton;
    private Button cancelButton;
}
