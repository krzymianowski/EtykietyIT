namespace EtykietyIT;

partial class MainForm
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
        menuStrip = new MenuStrip();
        fileMenuItem = new ToolStripMenuItem();
        exitMenuItem = new ToolStripMenuItem();
        toolsMenuItem = new ToolStripMenuItem();
        organizationProfilesMenuItem = new ToolStripMenuItem();
        labelProfilesMenuItem = new ToolStripMenuItem();
        helpMenuItem = new ToolStripMenuItem();
        aboutMenuItem = new ToolStripMenuItem();
        mainLayoutPanel = new TableLayoutPanel();
        organizationGroupBox = new GroupBox();
        organizationLayoutPanel = new TableLayoutPanel();
        organizationPromptLabel = new Label();
        organizationComboBox = new ComboBox();
        manageOrganizationsButton = new Button();
        printingGroupBox = new GroupBox();
        printingLayoutPanel = new TableLayoutPanel();
        printerPromptLabel = new Label();
        printerComboBox = new ComboBox();
        profilePromptLabel = new Label();
        profileComboBox = new ComboBox();
        profilesButton = new Button();
        numberingGroupBox = new GroupBox();
        numberingLayoutPanel = new TableLayoutPanel();
        firstNumberPromptLabel = new Label();
        firstNumberNumericUpDown = new NumericUpDown();
        quantityPromptLabel = new Label();
        quantityNumericUpDown = new NumericUpDown();
        assetRangePromptLabel = new Label();
        assetRangeLabel = new Label();
        calibrationGroupBox = new GroupBox();
        calibrationLayoutPanel = new TableLayoutPanel();
        calibrationXPromptLabel = new Label();
        calibrationXNumericUpDown = new NumericUpDown();
        calibrationYPromptLabel = new Label();
        calibrationYNumericUpDown = new NumericUpDown();
        saveCalibrationButton = new Button();
        actionsGroupBox = new GroupBox();
        actionsLayoutPanel = new TableLayoutPanel();
        historyButton = new Button();
        previewButton = new Button();
        printButton = new Button();
        calibrationToolTip = new ToolTip(components);
        menuStrip.SuspendLayout();
        mainLayoutPanel.SuspendLayout();
        organizationGroupBox.SuspendLayout();
        organizationLayoutPanel.SuspendLayout();
        printingGroupBox.SuspendLayout();
        printingLayoutPanel.SuspendLayout();
        numberingGroupBox.SuspendLayout();
        numberingLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)firstNumberNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)quantityNumericUpDown).BeginInit();
        calibrationGroupBox.SuspendLayout();
        calibrationLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)calibrationXNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)calibrationYNumericUpDown).BeginInit();
        actionsGroupBox.SuspendLayout();
        actionsLayoutPanel.SuspendLayout();
        SuspendLayout();
        //
        // menuStrip
        //
        menuStrip.Items.AddRange(new ToolStripItem[] {
            fileMenuItem,
            toolsMenuItem,
            helpMenuItem });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(664, 24);
        menuStrip.TabIndex = 0;
        //
        // fileMenuItem
        //
        fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            exitMenuItem });
        fileMenuItem.Name = "fileMenuItem";
        fileMenuItem.Text = "Plik";
        //
        // exitMenuItem
        //
        exitMenuItem.Name = "exitMenuItem";
        exitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
        exitMenuItem.Text = "Zakończ";
        //
        // toolsMenuItem
        //
        toolsMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            organizationProfilesMenuItem,
            labelProfilesMenuItem });
        toolsMenuItem.Name = "toolsMenuItem";
        toolsMenuItem.Text = "Narzędzia";
        //
        // organizationProfilesMenuItem
        //
        organizationProfilesMenuItem.Name = "organizationProfilesMenuItem";
        organizationProfilesMenuItem.Text = "Profile organizacji...";
        //
        // labelProfilesMenuItem
        //
        labelProfilesMenuItem.Name = "labelProfilesMenuItem";
        labelProfilesMenuItem.Text = "Profile etykiet...";
        //
        // helpMenuItem
        //
        helpMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            aboutMenuItem });
        helpMenuItem.Name = "helpMenuItem";
        helpMenuItem.Text = "Pomoc";
        //
        // aboutMenuItem
        //
        aboutMenuItem.Name = "aboutMenuItem";
        aboutMenuItem.Text = "O programie...";
        //
        // mainLayoutPanel
        //
        mainLayoutPanel.ColumnCount = 1;
        mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayoutPanel.Controls.Add(organizationGroupBox, 0, 0);
        mainLayoutPanel.Controls.Add(printingGroupBox, 0, 1);
        mainLayoutPanel.Controls.Add(numberingGroupBox, 0, 2);
        mainLayoutPanel.Controls.Add(calibrationGroupBox, 0, 3);
        mainLayoutPanel.Controls.Add(actionsGroupBox, 0, 4);
        mainLayoutPanel.AutoScroll = true;
        mainLayoutPanel.Dock = DockStyle.Fill;
        mainLayoutPanel.Location = new Point(0, 24);
        mainLayoutPanel.Name = "mainLayoutPanel";
        mainLayoutPanel.Padding = new Padding(16, 12, 16, 16);
        mainLayoutPanel.RowCount = 5;
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.Size = new Size(664, 646);
        mainLayoutPanel.TabIndex = 1;
        //
        // organizationGroupBox
        //
        organizationGroupBox.Controls.Add(organizationLayoutPanel);
        organizationGroupBox.AutoSize = true;
        organizationGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        organizationGroupBox.Dock = DockStyle.Top;
        organizationGroupBox.Name = "organizationGroupBox";
        organizationGroupBox.Padding = new Padding(12, 8, 12, 10);
        organizationGroupBox.TabIndex = 0;
        organizationGroupBox.TabStop = false;
        organizationGroupBox.Text = "ORGANIZACJA";
        //
        // organizationLayoutPanel
        //
        organizationLayoutPanel.ColumnCount = 3;
        organizationLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        organizationLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        organizationLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        organizationLayoutPanel.Controls.Add(organizationPromptLabel, 0, 0);
        organizationLayoutPanel.Controls.Add(organizationComboBox, 1, 0);
        organizationLayoutPanel.Controls.Add(manageOrganizationsButton, 2, 0);
        organizationLayoutPanel.AutoSize = true;
        organizationLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        organizationLayoutPanel.Dock = DockStyle.Top;
        organizationLayoutPanel.Name = "organizationLayoutPanel";
        organizationLayoutPanel.RowCount = 1;
        organizationLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        organizationLayoutPanel.TabIndex = 0;
        //
        // organizationPromptLabel
        //
        organizationPromptLabel.Anchor = AnchorStyles.Left;
        organizationPromptLabel.AutoSize = true;
        organizationPromptLabel.Name = "organizationPromptLabel";
        organizationPromptLabel.Text = "Profil organizacji:";
        //
        // organizationComboBox
        //
        organizationComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        organizationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        organizationComboBox.FormattingEnabled = true;
        organizationComboBox.Name = "organizationComboBox";
        organizationComboBox.TabIndex = 0;
        //
        // manageOrganizationsButton
        //
        manageOrganizationsButton.Anchor = AnchorStyles.Right;
        manageOrganizationsButton.AutoSize = false;
        manageOrganizationsButton.MinimumSize = new Size(132, 30);
        manageOrganizationsButton.Name = "manageOrganizationsButton";
        manageOrganizationsButton.Size = new Size(132, 30);
        manageOrganizationsButton.TabIndex = 1;
        manageOrganizationsButton.Text = "Zarządzaj...";
        manageOrganizationsButton.UseVisualStyleBackColor = true;
        //
        // printingGroupBox
        //
        printingGroupBox.Controls.Add(printingLayoutPanel);
        printingGroupBox.AutoSize = true;
        printingGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        printingGroupBox.Dock = DockStyle.Top;
        printingGroupBox.Name = "printingGroupBox";
        printingGroupBox.Padding = new Padding(12, 8, 12, 10);
        printingGroupBox.TabIndex = 1;
        printingGroupBox.TabStop = false;
        printingGroupBox.Text = "DRUKOWANIE";
        //
        // printingLayoutPanel
        //
        printingLayoutPanel.ColumnCount = 3;
        printingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        printingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        printingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        printingLayoutPanel.Controls.Add(printerPromptLabel, 0, 0);
        printingLayoutPanel.Controls.Add(printerComboBox, 1, 0);
        printingLayoutPanel.SetColumnSpan(printerComboBox, 2);
        printingLayoutPanel.Controls.Add(profilePromptLabel, 0, 1);
        printingLayoutPanel.Controls.Add(profileComboBox, 1, 1);
        printingLayoutPanel.Controls.Add(profilesButton, 2, 1);
        printingLayoutPanel.AutoSize = true;
        printingLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        printingLayoutPanel.Dock = DockStyle.Top;
        printingLayoutPanel.Name = "printingLayoutPanel";
        printingLayoutPanel.RowCount = 2;
        printingLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        printingLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        printingLayoutPanel.TabIndex = 0;
        //
        // printerPromptLabel
        //
        printerPromptLabel.Anchor = AnchorStyles.Left;
        printerPromptLabel.AutoSize = true;
        printerPromptLabel.Name = "printerPromptLabel";
        printerPromptLabel.Text = "Drukarka:";
        //
        // printerComboBox
        //
        printerComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        printerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        printerComboBox.FormattingEnabled = true;
        printerComboBox.Margin = new Padding(3, 3, 10, 3);
        printerComboBox.Name = "printerComboBox";
        printerComboBox.TabIndex = 2;
        //
        // profilePromptLabel
        //
        profilePromptLabel.Anchor = AnchorStyles.Left;
        profilePromptLabel.AutoSize = true;
        profilePromptLabel.Name = "profilePromptLabel";
        profilePromptLabel.Text = "Profil etykiety:";
        //
        // profileComboBox
        //
        profileComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        profileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        profileComboBox.FormattingEnabled = true;
        profileComboBox.Name = "profileComboBox";
        profileComboBox.TabIndex = 3;
        //
        // profilesButton
        //
        profilesButton.Anchor = AnchorStyles.Right;
        profilesButton.AutoSize = false;
        profilesButton.MinimumSize = new Size(132, 30);
        profilesButton.Name = "profilesButton";
        profilesButton.Size = new Size(132, 30);
        profilesButton.TabIndex = 4;
        profilesButton.Text = "Profile...";
        profilesButton.UseVisualStyleBackColor = true;
        //
        // numberingGroupBox
        //
        numberingGroupBox.Controls.Add(numberingLayoutPanel);
        numberingGroupBox.AutoSize = true;
        numberingGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        numberingGroupBox.Dock = DockStyle.Top;
        numberingGroupBox.Name = "numberingGroupBox";
        numberingGroupBox.Padding = new Padding(12, 8, 12, 10);
        numberingGroupBox.TabIndex = 2;
        numberingGroupBox.TabStop = false;
        numberingGroupBox.Text = "NUMERACJA";
        //
        // numberingLayoutPanel
        //
        numberingLayoutPanel.ColumnCount = 2;
        numberingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        numberingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        numberingLayoutPanel.Controls.Add(firstNumberPromptLabel, 0, 0);
        numberingLayoutPanel.Controls.Add(firstNumberNumericUpDown, 1, 0);
        numberingLayoutPanel.Controls.Add(quantityPromptLabel, 0, 1);
        numberingLayoutPanel.Controls.Add(quantityNumericUpDown, 1, 1);
        numberingLayoutPanel.Controls.Add(assetRangePromptLabel, 0, 2);
        numberingLayoutPanel.Controls.Add(assetRangeLabel, 1, 2);
        numberingLayoutPanel.AutoSize = true;
        numberingLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        numberingLayoutPanel.Dock = DockStyle.Top;
        numberingLayoutPanel.Name = "numberingLayoutPanel";
        numberingLayoutPanel.RowCount = 3;
        numberingLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        numberingLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        numberingLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        numberingLayoutPanel.TabIndex = 0;
        //
        // firstNumberPromptLabel
        //
        firstNumberPromptLabel.Anchor = AnchorStyles.Left;
        firstNumberPromptLabel.AutoSize = true;
        firstNumberPromptLabel.Name = "firstNumberPromptLabel";
        firstNumberPromptLabel.Text = "Pierwszy numer:";
        //
        // firstNumberNumericUpDown
        //
        firstNumberNumericUpDown.Anchor = AnchorStyles.Left;
        firstNumberNumericUpDown.Font = new Font("Consolas", 9F);
        firstNumberNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        firstNumberNumericUpDown.Name = "firstNumberNumericUpDown";
        firstNumberNumericUpDown.Size = new Size(170, 22);
        firstNumberNumericUpDown.TabIndex = 5;
        firstNumberNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
        //
        // quantityPromptLabel
        //
        quantityPromptLabel.Anchor = AnchorStyles.Left;
        quantityPromptLabel.AutoSize = true;
        quantityPromptLabel.Name = "quantityPromptLabel";
        quantityPromptLabel.Text = "Liczba małych etykiet:";
        //
        // quantityNumericUpDown
        //
        quantityNumericUpDown.Anchor = AnchorStyles.Left;
        quantityNumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        quantityNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        quantityNumericUpDown.Name = "quantityNumericUpDown";
        quantityNumericUpDown.Size = new Size(170, 23);
        quantityNumericUpDown.TabIndex = 6;
        quantityNumericUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
        //
        // assetRangePromptLabel
        //
        assetRangePromptLabel.Anchor = AnchorStyles.Left;
        assetRangePromptLabel.AutoSize = true;
        assetRangePromptLabel.Name = "assetRangePromptLabel";
        assetRangePromptLabel.Text = "Zakres Asset ID:";
        //
        // assetRangeLabel
        //
        assetRangeLabel.Anchor = AnchorStyles.Left;
        assetRangeLabel.AutoSize = true;
        assetRangeLabel.Font = new Font("Consolas", 10.5F, FontStyle.Bold);
        assetRangeLabel.Name = "assetRangeLabel";
        assetRangeLabel.Text = "—";
        //
        // calibrationGroupBox
        //
        calibrationGroupBox.Controls.Add(calibrationLayoutPanel);
        calibrationGroupBox.AutoSize = true;
        calibrationGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        calibrationGroupBox.Dock = DockStyle.Top;
        calibrationGroupBox.Name = "calibrationGroupBox";
        calibrationGroupBox.Padding = new Padding(12, 8, 12, 10);
        calibrationGroupBox.TabIndex = 3;
        calibrationGroupBox.TabStop = false;
        calibrationGroupBox.Text = "KALIBRACJA DRUKARKI";
        //
        // calibrationLayoutPanel
        //
        calibrationLayoutPanel.ColumnCount = 2;
        calibrationLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        calibrationLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        calibrationLayoutPanel.Controls.Add(calibrationXPromptLabel, 0, 0);
        calibrationLayoutPanel.Controls.Add(calibrationXNumericUpDown, 1, 0);
        calibrationLayoutPanel.Controls.Add(calibrationYPromptLabel, 0, 1);
        calibrationLayoutPanel.Controls.Add(calibrationYNumericUpDown, 1, 1);
        calibrationLayoutPanel.Controls.Add(saveCalibrationButton, 1, 2);
        calibrationLayoutPanel.AutoSize = true;
        calibrationLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        calibrationLayoutPanel.Dock = DockStyle.Top;
        calibrationLayoutPanel.Name = "calibrationLayoutPanel";
        calibrationLayoutPanel.RowCount = 3;
        calibrationLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        calibrationLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        calibrationLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        calibrationLayoutPanel.TabIndex = 0;
        //
        // calibrationXPromptLabel
        //
        calibrationXPromptLabel.Anchor = AnchorStyles.Left;
        calibrationXPromptLabel.AutoSize = true;
        calibrationXPromptLabel.Name = "calibrationXPromptLabel";
        calibrationXPromptLabel.Text = "Korekta X [mm]:";
        //
        // calibrationXNumericUpDown
        //
        calibrationXNumericUpDown.Anchor = AnchorStyles.Left;
        calibrationXNumericUpDown.DecimalPlaces = 1;
        calibrationXNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        calibrationXNumericUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        calibrationXNumericUpDown.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
        calibrationXNumericUpDown.Name = "calibrationXNumericUpDown";
        calibrationXNumericUpDown.Size = new Size(170, 23);
        calibrationXNumericUpDown.TabIndex = 7;
        //
        // calibrationYPromptLabel
        //
        calibrationYPromptLabel.Anchor = AnchorStyles.Left;
        calibrationYPromptLabel.AutoSize = true;
        calibrationYPromptLabel.Name = "calibrationYPromptLabel";
        calibrationYPromptLabel.Text = "Korekta Y [mm]:";
        //
        // calibrationYNumericUpDown
        //
        calibrationYNumericUpDown.Anchor = AnchorStyles.Left;
        calibrationYNumericUpDown.DecimalPlaces = 1;
        calibrationYNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        calibrationYNumericUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        calibrationYNumericUpDown.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
        calibrationYNumericUpDown.Name = "calibrationYNumericUpDown";
        calibrationYNumericUpDown.Size = new Size(170, 23);
        calibrationYNumericUpDown.TabIndex = 8;
        //
        // saveCalibrationButton
        //
        saveCalibrationButton.Anchor = AnchorStyles.Left;
        saveCalibrationButton.AutoSize = false;
        saveCalibrationButton.MinimumSize = new Size(220, 30);
        saveCalibrationButton.Name = "saveCalibrationButton";
        saveCalibrationButton.Size = new Size(220, 30);
        saveCalibrationButton.TabIndex = 9;
        saveCalibrationButton.Text = "Zapisz kalibrację drukarki";
        calibrationToolTip.SetToolTip(saveCalibrationButton,
            "Kalibracja jest przypisana do wybranej drukarki i używana we wszystkich profilach organizacji.");
        saveCalibrationButton.UseVisualStyleBackColor = true;
        //
        // actionsGroupBox
        //
        actionsGroupBox.Controls.Add(actionsLayoutPanel);
        actionsGroupBox.AutoSize = true;
        actionsGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        actionsGroupBox.Dock = DockStyle.Top;
        actionsGroupBox.Name = "actionsGroupBox";
        actionsGroupBox.Padding = new Padding(12, 8, 12, 10);
        actionsGroupBox.TabIndex = 4;
        actionsGroupBox.TabStop = false;
        actionsGroupBox.Text = "AKCJE";
        //
        // actionsLayoutPanel
        //
        actionsLayoutPanel.ColumnCount = 4;
        actionsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        actionsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        actionsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        actionsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        actionsLayoutPanel.Controls.Add(historyButton, 0, 0);
        actionsLayoutPanel.Controls.Add(previewButton, 2, 0);
        actionsLayoutPanel.Controls.Add(printButton, 3, 0);
        actionsLayoutPanel.AutoSize = true;
        actionsLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        actionsLayoutPanel.Dock = DockStyle.Top;
        actionsLayoutPanel.Name = "actionsLayoutPanel";
        actionsLayoutPanel.RowCount = 1;
        actionsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        actionsLayoutPanel.TabIndex = 0;
        //
        // historyButton
        //
        historyButton.Anchor = AnchorStyles.Left;
        historyButton.AutoSize = false;
        historyButton.MinimumSize = new Size(110, 32);
        historyButton.Name = "historyButton";
        historyButton.Size = new Size(110, 32);
        historyButton.TabIndex = 10;
        historyButton.Text = "Historia...";
        historyButton.UseVisualStyleBackColor = true;
        //
        // previewButton
        //
        previewButton.Anchor = AnchorStyles.Right;
        previewButton.AutoSize = false;
        previewButton.MinimumSize = new Size(104, 32);
        previewButton.Name = "previewButton";
        previewButton.Size = new Size(104, 32);
        previewButton.TabIndex = 11;
        previewButton.Text = "Podgląd";
        previewButton.UseVisualStyleBackColor = true;
        //
        // printButton
        //
        printButton.Anchor = AnchorStyles.Right;
        printButton.AutoSize = false;
        printButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        printButton.Name = "printButton";
        printButton.MinimumSize = new Size(104, 32);
        printButton.Size = new Size(104, 32);
        printButton.TabIndex = 12;
        printButton.Text = "Drukuj";
        printButton.UseVisualStyleBackColor = true;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(664, 670);
        Controls.Add(mainLayoutPanel);
        Controls.Add(menuStrip);
        Font = new Font("Segoe UI", 9F);
        MainMenuStrip = menuStrip;
        MinimumSize = new Size(680, 710);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Etykiety IT";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        mainLayoutPanel.ResumeLayout(false);
        organizationGroupBox.ResumeLayout(false);
        organizationLayoutPanel.ResumeLayout(false);
        organizationLayoutPanel.PerformLayout();
        printingGroupBox.ResumeLayout(false);
        printingLayoutPanel.ResumeLayout(false);
        printingLayoutPanel.PerformLayout();
        numberingGroupBox.ResumeLayout(false);
        numberingLayoutPanel.ResumeLayout(false);
        numberingLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)firstNumberNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)quantityNumericUpDown).EndInit();
        calibrationGroupBox.ResumeLayout(false);
        calibrationLayoutPanel.ResumeLayout(false);
        calibrationLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)calibrationXNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)calibrationYNumericUpDown).EndInit();
        actionsGroupBox.ResumeLayout(false);
        actionsLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private MenuStrip menuStrip;
    private ToolStripMenuItem fileMenuItem;
    private ToolStripMenuItem exitMenuItem;
    private ToolStripMenuItem toolsMenuItem;
    private ToolStripMenuItem organizationProfilesMenuItem;
    private ToolStripMenuItem labelProfilesMenuItem;
    private ToolStripMenuItem helpMenuItem;
    private ToolStripMenuItem aboutMenuItem;
    private TableLayoutPanel mainLayoutPanel;
    private GroupBox organizationGroupBox;
    private TableLayoutPanel organizationLayoutPanel;
    private Label organizationPromptLabel;
    private ComboBox organizationComboBox;
    private Button manageOrganizationsButton;
    private GroupBox printingGroupBox;
    private TableLayoutPanel printingLayoutPanel;
    private Label printerPromptLabel;
    private ComboBox printerComboBox;
    private Label profilePromptLabel;
    private ComboBox profileComboBox;
    private Button profilesButton;
    private GroupBox numberingGroupBox;
    private TableLayoutPanel numberingLayoutPanel;
    private Label firstNumberPromptLabel;
    private NumericUpDown firstNumberNumericUpDown;
    private Label quantityPromptLabel;
    private NumericUpDown quantityNumericUpDown;
    private Label assetRangePromptLabel;
    private Label assetRangeLabel;
    private GroupBox calibrationGroupBox;
    private TableLayoutPanel calibrationLayoutPanel;
    private Label calibrationXPromptLabel;
    private NumericUpDown calibrationXNumericUpDown;
    private Label calibrationYPromptLabel;
    private NumericUpDown calibrationYNumericUpDown;
    private Button saveCalibrationButton;
    private GroupBox actionsGroupBox;
    private TableLayoutPanel actionsLayoutPanel;
    private Button historyButton;
    private Button previewButton;
    private Button printButton;
    private ToolTip calibrationToolTip;
}
