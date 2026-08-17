using EtykietyIT.Models;

namespace EtykietyIT.Forms;

public partial class ProfileEditForm : Form
{
    private readonly LabelProfile _originalProfile;

    public ProfileEditForm(LabelProfile profile, string windowTitle)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentException.ThrowIfNullOrWhiteSpace(windowTitle);

        _originalProfile = profile;
        Profile = profile;

        InitializeComponent();
        Text = windowTitle;

        nameTextBox.Text = profile.Name;
        widthNumericUpDown.Value = (decimal)profile.WidthMm;
        heightNumericUpDown.Value = (decimal)profile.HeightMm;
        columnsNumericUpDown.Value = profile.Columns;
        rowsNumericUpDown.Value = profile.Rows;
        drawCutLinesCheckBox.Checked = profile.DrawCutLines;

        widthNumericUpDown.ValueChanged += GeometryValueChanged;
        heightNumericUpDown.ValueChanged += GeometryValueChanged;
        columnsNumericUpDown.ValueChanged += GeometryValueChanged;
        rowsNumericUpDown.ValueChanged += GeometryValueChanged;
        UpdateCellSizeInformation();

        saveButton.Click += SaveButton_Click;
        cancelButton.Click += CancelButton_Click;
    }

    public LabelProfile Profile { get; private set; }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        LabelProfile updatedProfile = _originalProfile with
        {
            Name = nameTextBox.Text.Trim(),
            WidthMm = decimal.ToDouble(widthNumericUpDown.Value),
            HeightMm = decimal.ToDouble(heightNumericUpDown.Value),
            Columns = decimal.ToInt32(columnsNumericUpDown.Value),
            Rows = decimal.ToInt32(rowsNumericUpDown.Value),
            DrawCutLines = drawCutLinesCheckBox.Checked
        };

        try
        {
            updatedProfile.Validate();
            Profile = updatedProfile;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (InvalidOperationException exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Nieprawidłowy profil",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void GeometryValueChanged(object? sender, EventArgs e)
    {
        UpdateCellSizeInformation();
    }

    private void UpdateCellSizeInformation()
    {
        decimal cellWidth =
            widthNumericUpDown.Value / columnsNumericUpDown.Value;
        decimal cellHeight =
            heightNumericUpDown.Value / rowsNumericUpDown.Value;
        cellSizeLabel.Text =
            $"Rozmiar pojedynczej etykiety: {cellWidth:0.0} × {cellHeight:0.0} mm";
    }
}
