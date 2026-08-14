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
        printerPromptLabel = new Label();
        printerComboBox = new ComboBox();
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
        previewButton = new Button();
        printButton = new Button();
        ((System.ComponentModel.ISupportInitialize)firstNumberNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)quantityNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)calibrationXNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)calibrationYNumericUpDown).BeginInit();
        SuspendLayout();
        //
        // printerPromptLabel
        //
        printerPromptLabel.AutoSize = true;
        printerPromptLabel.Location = new Point(24, 27);
        printerPromptLabel.Name = "printerPromptLabel";
        printerPromptLabel.Size = new Size(57, 15);
        printerPromptLabel.TabIndex = 0;
        printerPromptLabel.Text = "Drukarka:";
        //
        // printerComboBox
        //
        printerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        printerComboBox.FormattingEnabled = true;
        printerComboBox.Location = new Point(145, 24);
        printerComboBox.Name = "printerComboBox";
        printerComboBox.Size = new Size(345, 23);
        printerComboBox.TabIndex = 1;
        //
        // firstNumberPromptLabel
        //
        firstNumberPromptLabel.AutoSize = true;
        firstNumberPromptLabel.Location = new Point(24, 72);
        firstNumberPromptLabel.Name = "firstNumberPromptLabel";
        firstNumberPromptLabel.Size = new Size(88, 15);
        firstNumberPromptLabel.TabIndex = 2;
        firstNumberPromptLabel.Text = "Pierwszy numer:";
        //
        // firstNumberNumericUpDown
        //
        firstNumberNumericUpDown.Font = new Font("Consolas", 9F);
        firstNumberNumericUpDown.Location = new Point(145, 68);
        firstNumberNumericUpDown.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
        firstNumberNumericUpDown.Name = "firstNumberNumericUpDown";
        firstNumberNumericUpDown.Size = new Size(130, 22);
        firstNumberNumericUpDown.TabIndex = 3;
        firstNumberNumericUpDown.Value = new decimal(new int[] { 11, 0, 0, 0 });
        //
        // quantityPromptLabel
        //
        quantityPromptLabel.AutoSize = true;
        quantityPromptLabel.Location = new Point(24, 111);
        quantityPromptLabel.Name = "quantityPromptLabel";
        quantityPromptLabel.Size = new Size(115, 15);
        quantityPromptLabel.TabIndex = 4;
        quantityPromptLabel.Text = "Liczba małych etykiet:";
        //
        // quantityNumericUpDown
        //
        quantityNumericUpDown.Location = new Point(145, 107);
        quantityNumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        quantityNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        quantityNumericUpDown.Name = "quantityNumericUpDown";
        quantityNumericUpDown.Size = new Size(130, 23);
        quantityNumericUpDown.TabIndex = 5;
        quantityNumericUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
        //
        // assetRangePromptLabel
        //
        assetRangePromptLabel.AutoSize = true;
        assetRangePromptLabel.Location = new Point(24, 154);
        assetRangePromptLabel.Name = "assetRangePromptLabel";
        assetRangePromptLabel.Size = new Size(44, 15);
        assetRangePromptLabel.TabIndex = 6;
        assetRangePromptLabel.Text = "Zakres:";
        //
        // assetRangeLabel
        //
        assetRangeLabel.AutoSize = true;
        assetRangeLabel.Font = new Font("Consolas", 10.5F, FontStyle.Bold);
        assetRangeLabel.Location = new Point(145, 152);
        assetRangeLabel.Name = "assetRangeLabel";
        assetRangeLabel.Size = new Size(176, 17);
        assetRangeLabel.TabIndex = 7;
        assetRangeLabel.Text = "IT-000011 – IT-000012";
        //
        // calibrationXPromptLabel
        //
        calibrationXPromptLabel.AutoSize = true;
        calibrationXPromptLabel.Location = new Point(24, 199);
        calibrationXPromptLabel.Name = "calibrationXPromptLabel";
        calibrationXPromptLabel.Size = new Size(92, 15);
        calibrationXPromptLabel.TabIndex = 8;
        calibrationXPromptLabel.Text = "Korekta X [mm]:";
        //
        // calibrationXNumericUpDown
        //
        calibrationXNumericUpDown.DecimalPlaces = 1;
        calibrationXNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        calibrationXNumericUpDown.Location = new Point(145, 195);
        calibrationXNumericUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        calibrationXNumericUpDown.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
        calibrationXNumericUpDown.Name = "calibrationXNumericUpDown";
        calibrationXNumericUpDown.Size = new Size(130, 23);
        calibrationXNumericUpDown.TabIndex = 9;
        //
        // calibrationYPromptLabel
        //
        calibrationYPromptLabel.AutoSize = true;
        calibrationYPromptLabel.Location = new Point(24, 238);
        calibrationYPromptLabel.Name = "calibrationYPromptLabel";
        calibrationYPromptLabel.Size = new Size(92, 15);
        calibrationYPromptLabel.TabIndex = 10;
        calibrationYPromptLabel.Text = "Korekta Y [mm]:";
        //
        // calibrationYNumericUpDown
        //
        calibrationYNumericUpDown.DecimalPlaces = 1;
        calibrationYNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        calibrationYNumericUpDown.Location = new Point(145, 234);
        calibrationYNumericUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        calibrationYNumericUpDown.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
        calibrationYNumericUpDown.Name = "calibrationYNumericUpDown";
        calibrationYNumericUpDown.Size = new Size(130, 23);
        calibrationYNumericUpDown.TabIndex = 11;
        //
        // previewButton
        //
        previewButton.Location = new Point(287, 282);
        previewButton.Name = "previewButton";
        previewButton.Size = new Size(95, 34);
        previewButton.TabIndex = 12;
        previewButton.Text = "Podgląd";
        previewButton.UseVisualStyleBackColor = true;
        //
        // printButton
        //
        printButton.Location = new Point(395, 282);
        printButton.Name = "printButton";
        printButton.Size = new Size(95, 34);
        printButton.TabIndex = 13;
        printButton.Text = "Drukuj";
        printButton.UseVisualStyleBackColor = true;
        //
        // Form1
        //
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(519, 340);
        Controls.Add(printButton);
        Controls.Add(previewButton);
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
        Controls.Add(printerComboBox);
        Controls.Add(printerPromptLabel);
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

    private Label printerPromptLabel;
    private ComboBox printerComboBox;
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
    private Button previewButton;
    private Button printButton;
}
