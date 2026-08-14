namespace EtykietyIT;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        organizationPromptLabel = new Label();
        organizationComboBox = new ComboBox();
        manageOrganizationsButton = new Button();
        printerPromptLabel = new Label();
        printerComboBox = new ComboBox();
        profilePromptLabel = new Label();
        profileComboBox = new ComboBox();
        profilesButton = new Button();
        firstNumberPromptLabel = new Label();
        firstNumberNumericUpDown = new NumericUpDown();
        quantityPromptLabel = new Label();
        quantityNumericUpDown = new NumericUpDown();
        assetRangePromptLabel = new Label();
        assetRangeLabel = new Label();
        calibrationXPromptLabel = new Label();
        calibrationXNumericUpDown = new NumericUpDown();
        calibrationYPromptLabel = new Label();
        calibrationYNumericUpDown = new NumericUpDown();
        saveCalibrationButton = new Button();
        historyButton = new Button();
        previewButton = new Button();
        printButton = new Button();
        ((System.ComponentModel.ISupportInitialize)firstNumberNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)quantityNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)calibrationXNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)calibrationYNumericUpDown).BeginInit();
        SuspendLayout();
        //
        // organizationPromptLabel
        //
        organizationPromptLabel.AutoSize = true;
        organizationPromptLabel.Location = new Point(24, 27);
        organizationPromptLabel.Name = "organizationPromptLabel";
        organizationPromptLabel.Size = new Size(105, 15);
        organizationPromptLabel.TabIndex = 0;
        organizationPromptLabel.Text = "Profil organizacji:";
        //
        // organizationComboBox
        //
        organizationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        organizationComboBox.FormattingEnabled = true;
        organizationComboBox.Location = new Point(145, 24);
        organizationComboBox.Name = "organizationComboBox";
        organizationComboBox.Size = new Size(245, 23);
        organizationComboBox.TabIndex = 1;
        //
        // manageOrganizationsButton
        //
        manageOrganizationsButton.Location = new Point(398, 21);
        manageOrganizationsButton.Name = "manageOrganizationsButton";
        manageOrganizationsButton.Size = new Size(92, 29);
        manageOrganizationsButton.TabIndex = 2;
        manageOrganizationsButton.Text = "Zarządzaj...";
        manageOrganizationsButton.UseVisualStyleBackColor = true;
        //
        // printerPromptLabel
        //
        printerPromptLabel.AutoSize = true;
        printerPromptLabel.Location = new Point(24, 66);
        printerPromptLabel.Name = "printerPromptLabel";
        printerPromptLabel.Size = new Size(57, 15);
        printerPromptLabel.TabIndex = 3;
        printerPromptLabel.Text = "Drukarka:";
        //
        // printerComboBox
        //
        printerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        printerComboBox.FormattingEnabled = true;
        printerComboBox.Location = new Point(145, 63);
        printerComboBox.Name = "printerComboBox";
        printerComboBox.Size = new Size(345, 23);
        printerComboBox.TabIndex = 4;
        //
        // profilePromptLabel
        //
        profilePromptLabel.AutoSize = true;
        profilePromptLabel.Location = new Point(24, 105);
        profilePromptLabel.Name = "profilePromptLabel";
        profilePromptLabel.Size = new Size(78, 15);
        profilePromptLabel.TabIndex = 5;
        profilePromptLabel.Text = "Profil etykiety:";
        //
        // profileComboBox
        //
        profileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        profileComboBox.FormattingEnabled = true;
        profileComboBox.Location = new Point(145, 102);
        profileComboBox.Name = "profileComboBox";
        profileComboBox.Size = new Size(245, 23);
        profileComboBox.TabIndex = 6;
        //
        // profilesButton
        //
        profilesButton.Location = new Point(398, 99);
        profilesButton.Name = "profilesButton";
        profilesButton.Size = new Size(92, 29);
        profilesButton.TabIndex = 7;
        profilesButton.Text = "Profile...";
        profilesButton.UseVisualStyleBackColor = true;
        //
        // firstNumberPromptLabel
        //
        firstNumberPromptLabel.AutoSize = true;
        firstNumberPromptLabel.Location = new Point(24, 150);
        firstNumberPromptLabel.Name = "firstNumberPromptLabel";
        firstNumberPromptLabel.Size = new Size(88, 15);
        firstNumberPromptLabel.TabIndex = 8;
        firstNumberPromptLabel.Text = "Pierwszy numer:";
        //
        // firstNumberNumericUpDown
        //
        firstNumberNumericUpDown.Font = new Font("Consolas", 9F);
        firstNumberNumericUpDown.Location = new Point(145, 146);
        firstNumberNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        firstNumberNumericUpDown.Name = "firstNumberNumericUpDown";
        firstNumberNumericUpDown.Size = new Size(130, 22);
        firstNumberNumericUpDown.TabIndex = 9;
        firstNumberNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
        //
        // quantityPromptLabel
        //
        quantityPromptLabel.AutoSize = true;
        quantityPromptLabel.Location = new Point(24, 189);
        quantityPromptLabel.Name = "quantityPromptLabel";
        quantityPromptLabel.Size = new Size(115, 15);
        quantityPromptLabel.TabIndex = 10;
        quantityPromptLabel.Text = "Liczba małych etykiet:";
        //
        // quantityNumericUpDown
        //
        quantityNumericUpDown.Location = new Point(145, 185);
        quantityNumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        quantityNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        quantityNumericUpDown.Name = "quantityNumericUpDown";
        quantityNumericUpDown.Size = new Size(130, 23);
        quantityNumericUpDown.TabIndex = 11;
        quantityNumericUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
        //
        // assetRangePromptLabel
        //
        assetRangePromptLabel.AutoSize = true;
        assetRangePromptLabel.Location = new Point(24, 232);
        assetRangePromptLabel.Name = "assetRangePromptLabel";
        assetRangePromptLabel.Size = new Size(44, 15);
        assetRangePromptLabel.TabIndex = 12;
        assetRangePromptLabel.Text = "Zakres:";
        //
        // assetRangeLabel
        //
        assetRangeLabel.AutoSize = true;
        assetRangeLabel.Font = new Font("Consolas", 10.5F, FontStyle.Bold);
        assetRangeLabel.Location = new Point(145, 230);
        assetRangeLabel.Name = "assetRangeLabel";
        assetRangeLabel.Size = new Size(176, 17);
        assetRangeLabel.TabIndex = 13;
        assetRangeLabel.Text = "—";
        //
        // calibrationXPromptLabel
        //
        calibrationXPromptLabel.AutoSize = true;
        calibrationXPromptLabel.Location = new Point(24, 277);
        calibrationXPromptLabel.Name = "calibrationXPromptLabel";
        calibrationXPromptLabel.Size = new Size(92, 15);
        calibrationXPromptLabel.TabIndex = 14;
        calibrationXPromptLabel.Text = "Korekta X [mm]:";
        //
        // calibrationXNumericUpDown
        //
        calibrationXNumericUpDown.DecimalPlaces = 1;
        calibrationXNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        calibrationXNumericUpDown.Location = new Point(145, 273);
        calibrationXNumericUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        calibrationXNumericUpDown.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
        calibrationXNumericUpDown.Name = "calibrationXNumericUpDown";
        calibrationXNumericUpDown.Size = new Size(130, 23);
        calibrationXNumericUpDown.TabIndex = 15;
        //
        // calibrationYPromptLabel
        //
        calibrationYPromptLabel.AutoSize = true;
        calibrationYPromptLabel.Location = new Point(24, 316);
        calibrationYPromptLabel.Name = "calibrationYPromptLabel";
        calibrationYPromptLabel.Size = new Size(92, 15);
        calibrationYPromptLabel.TabIndex = 16;
        calibrationYPromptLabel.Text = "Korekta Y [mm]:";
        //
        // calibrationYNumericUpDown
        //
        calibrationYNumericUpDown.DecimalPlaces = 1;
        calibrationYNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        calibrationYNumericUpDown.Location = new Point(145, 312);
        calibrationYNumericUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        calibrationYNumericUpDown.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
        calibrationYNumericUpDown.Name = "calibrationYNumericUpDown";
        calibrationYNumericUpDown.Size = new Size(130, 23);
        calibrationYNumericUpDown.TabIndex = 17;
        //
        // saveCalibrationButton
        //
        saveCalibrationButton.Location = new Point(287, 301);
        saveCalibrationButton.Name = "saveCalibrationButton";
        saveCalibrationButton.Size = new Size(203, 34);
        saveCalibrationButton.TabIndex = 18;
        saveCalibrationButton.Text = "Zapisz globalną kalibrację";
        saveCalibrationButton.UseVisualStyleBackColor = true;
        //
        // historyButton
        //
        historyButton.Location = new Point(24, 360);
        historyButton.Name = "historyButton";
        historyButton.Size = new Size(110, 34);
        historyButton.TabIndex = 19;
        historyButton.Text = "Historia...";
        historyButton.UseVisualStyleBackColor = true;
        //
        // previewButton
        //
        previewButton.Location = new Point(287, 360);
        previewButton.Name = "previewButton";
        previewButton.Size = new Size(95, 34);
        previewButton.TabIndex = 20;
        previewButton.Text = "Podgląd";
        previewButton.UseVisualStyleBackColor = true;
        //
        // printButton
        //
        printButton.Location = new Point(395, 360);
        printButton.Name = "printButton";
        printButton.Size = new Size(95, 34);
        printButton.TabIndex = 21;
        printButton.Text = "Drukuj";
        printButton.UseVisualStyleBackColor = true;
        //
        // Form1
        //
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(519, 418);
        Controls.Add(printButton);
        Controls.Add(previewButton);
        Controls.Add(historyButton);
        Controls.Add(profilesButton);
        Controls.Add(saveCalibrationButton);
        Controls.Add(calibrationYNumericUpDown);
        Controls.Add(calibrationYPromptLabel);
        Controls.Add(calibrationXNumericUpDown);
        Controls.Add(calibrationXPromptLabel);
        Controls.Add(assetRangeLabel);
        Controls.Add(assetRangePromptLabel);
        Controls.Add(quantityNumericUpDown);
        Controls.Add(quantityPromptLabel);
        Controls.Add(firstNumberNumericUpDown);
        Controls.Add(firstNumberPromptLabel);
        Controls.Add(profileComboBox);
        Controls.Add(profilePromptLabel);
        Controls.Add(printerComboBox);
        Controls.Add(printerPromptLabel);
        Controls.Add(manageOrganizationsButton);
        Controls.Add(organizationComboBox);
        Controls.Add(organizationPromptLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Etykiety IT — test drukowania";
        ((System.ComponentModel.ISupportInitialize)firstNumberNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)quantityNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)calibrationXNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)calibrationYNumericUpDown).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label organizationPromptLabel;
    private ComboBox organizationComboBox;
    private Button manageOrganizationsButton;
    private Label printerPromptLabel;
    private ComboBox printerComboBox;
    private Label profilePromptLabel;
    private ComboBox profileComboBox;
    private Button profilesButton;
    private Label firstNumberPromptLabel;
    private NumericUpDown firstNumberNumericUpDown;
    private Label quantityPromptLabel;
    private NumericUpDown quantityNumericUpDown;
    private Label assetRangePromptLabel;
    private Label assetRangeLabel;
    private Label calibrationXPromptLabel;
    private NumericUpDown calibrationXNumericUpDown;
    private Label calibrationYPromptLabel;
    private NumericUpDown calibrationYNumericUpDown;
    private Button saveCalibrationButton;
    private Button historyButton;
    private Button previewButton;
    private Button printButton;
}
