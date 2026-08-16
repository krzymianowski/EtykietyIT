using System.Globalization;
using System.Text;

namespace EtykietyIT.Diagnostics;

public sealed class UiDiagnosticsService : IDisposable
{
    public const string CommandLineArgument = "--ui-diagnostics";

    private static readonly Encoding ReportEncoding = new UTF8Encoding(false);
    private static readonly HashSet<string> DetailedFormTypeNames =
        new(StringComparer.Ordinal)
        {
            "MainForm",
            "ProfilesForm",
            "AboutForm"
        };

    private readonly HashSet<Form> _scheduledForms = [];
    private readonly HashSet<Form> _reportedForms = [];
    private readonly object _fileLock = new();
    private bool _disposed;

    private UiDiagnosticsService(string diagnosticsDirectory)
    {
        Directory.CreateDirectory(diagnosticsDirectory);
        ReportFilePath = Path.Combine(
            diagnosticsDirectory,
            $"ui-diagnostics-{DateTime.Now:yyyyMMdd-HHmmss-fff}.txt");

        WriteApplicationSection();
        Application.Idle += Application_Idle;
    }

    public string ReportFilePath { get; }

    public static bool IsRequested(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        return arguments.Any(argument => string.Equals(
            argument,
            CommandLineArgument,
            StringComparison.OrdinalIgnoreCase));
    }

    public static UiDiagnosticsService? StartIfRequested(
        IEnumerable<string> arguments,
        string diagnosticsDirectory)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        ArgumentException.ThrowIfNullOrWhiteSpace(diagnosticsDirectory);

        return IsRequested(arguments)
            ? new UiDiagnosticsService(diagnosticsDirectory)
            : null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Application.Idle -= Application_Idle;
        AppendReport(
            $"{Environment.NewLine}=== KONIEC RAPORTU ==={Environment.NewLine}" +
            $"LocalTimestamp: {DateTimeOffset.Now:O}{Environment.NewLine}" +
            $"UtcTimestamp: {DateTimeOffset.UtcNow:O}{Environment.NewLine}");
    }

    private void Application_Idle(object? sender, EventArgs e)
    {
        if (_disposed)
        {
            return;
        }

        Form[] openForms = Application.OpenForms.Cast<Form>().ToArray();
        foreach (Form form in openForms)
        {
            if (!form.Visible || !form.IsHandleCreated ||
                _reportedForms.Contains(form) || !_scheduledForms.Add(form))
            {
                continue;
            }

            try
            {
                form.BeginInvoke(new Action(() => ReportFormAfterShown(form)));
            }
            catch (InvalidOperationException)
            {
                _scheduledForms.Remove(form);
            }
        }
    }

    private void ReportFormAfterShown(Form form)
    {
        _scheduledForms.Remove(form);
        if (_disposed || form.IsDisposed || !form.Visible ||
            !_reportedForms.Add(form))
        {
            return;
        }

        AppendReport(CreateFormSection(form));
    }

    private void WriteApplicationSection()
    {
        Font? systemFont = SystemFonts.MessageBoxFont;
        Screen? primaryScreen = Screen.PrimaryScreen;
        var report = new StringBuilder();

        report.AppendLine("=== ETYKIETY IT — UI DIAGNOSTICS ===");
        report.AppendLine($"LocalTimestamp: {DateTimeOffset.Now:O}");
        report.AppendLine($"UtcTimestamp: {DateTimeOffset.UtcNow:O}");
        report.AppendLine($"Application.HighDpiMode: {Application.HighDpiMode}");
        report.AppendLine(
            $"SystemInformation.HighContrast: {SystemInformation.HighContrast}");
        report.AppendLine($"SystemFont.Name: {ValueOrDash(systemFont?.Name)}");
        report.AppendLine(
            "SystemFont.SizeInPoints: " +
            (systemFont is null
                ? "—"
                : FormatNumber(systemFont.SizeInPoints)));
        report.AppendLine(
            $"SystemFont.Height: {systemFont?.Height.ToString(CultureInfo.InvariantCulture) ?? "—"}");
        report.AppendLine(
            $"PrimaryScreen.Bounds: {FormatRectangle(primaryScreen?.Bounds)}");
        report.AppendLine(
            "PrimaryScreen.WorkingArea: " +
            FormatRectangle(primaryScreen?.WorkingArea));
        report.AppendLine();

        AppendReport(report.ToString());
    }

    private static string CreateFormSection(Form form)
    {
        float dpiScale = form.DeviceDpi / 96F;
        var report = new StringBuilder();

        report.AppendLine("=== FORMULARZ ===");
        report.AppendLine($"CapturedAtLocal: {DateTimeOffset.Now:O}");
        report.AppendLine($"Type: {form.GetType().FullName}");
        report.AppendLine($"Name: {ValueOrDash(form.Name)}");
        report.AppendLine($"DeviceDpi: {form.DeviceDpi}");
        report.AppendLine($"DpiScaleFrom96: {FormatNumber(dpiScale)}");
        report.AppendLine($"AutoScaleMode: {form.AutoScaleMode}");
        report.AppendLine(
            $"AutoScaleDimensions: {FormatSizeF(form.AutoScaleDimensions)}");
        report.AppendLine(
            "CurrentAutoScaleDimensions: " +
            FormatSizeF(form.CurrentAutoScaleDimensions));
        report.AppendLine($"Bounds: {FormatRectangle(form.Bounds)}");
        report.AppendLine($"ClientSize: {FormatSize(form.ClientSize)}");
        report.AppendLine(
            $"LogicalBoundsAt96Dpi: {FormatLogicalRectangle(form.Bounds, dpiScale)}");
        report.AppendLine(
            "LogicalClientSizeAt96Dpi: " +
            FormatLogicalSize(form.ClientSize, dpiScale));
        report.AppendLine($"PreferredSize: {FormatSize(form.PreferredSize)}");
        report.AppendLine(
            "LogicalPreferredSizeAt96Dpi: " +
            FormatLogicalSize(form.PreferredSize, dpiScale));
        report.AppendLine($"MinimumSize: {FormatSize(form.MinimumSize)}");
        report.AppendLine($"MaximumSize: {FormatSize(form.MaximumSize)}");
        report.AppendLine($"AutoSize: {form.AutoSize}");
        report.AppendLine($"AutoSizeMode: {form.AutoSizeMode}");
        AppendFont(report, form.Font);
        report.AppendLine();

        bool detailed = DetailedFormTypeNames.Contains(form.GetType().Name) ||
            form.Owner?.GetType().Name == "AboutForm";
        report.AppendLine(detailed
            ? "--- KONTROLKI (tryb szczegółowy) ---"
            : "--- GŁÓWNE KONTENERY ---");

        AppendControls(
            report,
            form,
            form.GetType().Name,
            dpiScale,
            detailed);
        report.AppendLine();

        return report.ToString();
    }

    private static void AppendControls(
        StringBuilder report,
        Control parent,
        string parentPath,
        float dpiScale,
        bool detailed)
    {
        for (int index = 0; index < parent.Controls.Count; index++)
        {
            Control control = parent.Controls[index];
            string identity = GetControlIdentity(control, index);
            string path = $"{parentPath}/{identity}";

            if (detailed || IsLayoutContainer(control) || control is Button)
            {
                AppendControl(report, control, path, dpiScale);
            }

            if (control.HasChildren)
            {
                AppendControls(report, control, path, dpiScale, detailed);
            }
        }
    }

    private static void AppendControl(
        StringBuilder report,
        Control control,
        string path,
        float dpiScale)
    {
        Size preferredSize;
        try
        {
            preferredSize = control.PreferredSize;
        }
        catch (InvalidOperationException)
        {
            preferredSize = Size.Empty;
        }

        report.AppendLine($"[{path}] ({control.GetType().Name})");
        report.AppendLine($"  Bounds: {FormatRectangle(control.Bounds)}");
        report.AppendLine($"  ClientSize: {FormatSize(control.ClientSize)}");
        report.AppendLine(
            $"  DisplayRectangle: {FormatRectangle(control.DisplayRectangle)}");
        report.AppendLine($"  PreferredSize: {FormatSize(preferredSize)}");
        report.AppendLine(
            "  LogicalBoundsAt96Dpi: " +
            FormatLogicalRectangle(control.Bounds, dpiScale));
        report.AppendLine(
            "  LogicalPreferredSizeAt96Dpi: " +
            FormatLogicalSize(preferredSize, dpiScale));
        report.AppendLine($"  AutoSize: {control.AutoSize}");
        report.AppendLine($"  Dock: {control.Dock}");
        report.AppendLine($"  Anchor: {control.Anchor}");
        report.AppendLine($"  Margin: {FormatPadding(control.Margin)}");
        report.AppendLine($"  Padding: {FormatPadding(control.Padding)}");
        report.AppendLine($"  Visible: {control.Visible}");
        AppendFont(report, control.Font, "  ");

        if (control is TableLayoutPanel tableLayoutPanel)
        {
            AppendTableLayout(report, tableLayoutPanel, dpiScale);
        }
        else if (control is FlowLayoutPanel flowLayoutPanel)
        {
            report.AppendLine(
                $"  FlowDirection: {flowLayoutPanel.FlowDirection}");
            report.AppendLine(
                $"  WrapContents: {flowLayoutPanel.WrapContents}");
        }
        else if (control is SplitContainer splitContainer)
        {
            report.AppendLine($"  Orientation: {splitContainer.Orientation}");
            report.AppendLine(
                $"  SplitterDistance: {splitContainer.SplitterDistance}");
            report.AppendLine(
                $"  Panel1MinSize: {splitContainer.Panel1MinSize}");
            report.AppendLine(
                $"  Panel2MinSize: {splitContainer.Panel2MinSize}");
        }
    }

    private static void AppendTableLayout(
        StringBuilder report,
        TableLayoutPanel tableLayoutPanel,
        float dpiScale)
    {
        report.AppendLine(
            $"  TableLayout: columns={tableLayoutPanel.ColumnCount}, " +
            $"rows={tableLayoutPanel.RowCount}");

        for (int index = 0; index < tableLayoutPanel.ColumnStyles.Count; index++)
        {
            ColumnStyle style = tableLayoutPanel.ColumnStyles[index];
            report.AppendLine(
                $"  ColumnStyle[{index}]: {style.SizeType}, " +
                $"{FormatNumber(style.Width)}");
        }

        for (int index = 0; index < tableLayoutPanel.RowStyles.Count; index++)
        {
            RowStyle style = tableLayoutPanel.RowStyles[index];
            report.AppendLine(
                $"  RowStyle[{index}]: {style.SizeType}, " +
                $"{FormatNumber(style.Height)}");
        }

        int[] columnWidths = tableLayoutPanel.GetColumnWidths();
        int[] rowHeights = tableLayoutPanel.GetRowHeights();
        report.AppendLine(
            $"  ActualColumnWidths: [{string.Join(", ", columnWidths)}]");
        report.AppendLine(
            $"  ActualRowHeights: [{string.Join(", ", rowHeights)}]");
        report.AppendLine(
            "  LogicalColumnWidthsAt96Dpi: [" +
            string.Join(", ", columnWidths.Select(value =>
                FormatLogicalValue(value, dpiScale))) + "]");
        report.AppendLine(
            "  LogicalRowHeightsAt96Dpi: [" +
            string.Join(", ", rowHeights.Select(value =>
                FormatLogicalValue(value, dpiScale))) + "]");
    }

    private static void AppendFont(
        StringBuilder report,
        Font font,
        string prefix = "")
    {
        report.AppendLine($"{prefix}Font.Name: {font.Name}");
        report.AppendLine(
            $"{prefix}Font.SizeInPoints: {FormatNumber(font.SizeInPoints)}");
        report.AppendLine($"{prefix}Font.Height: {font.Height}");
    }

    private void AppendReport(string text)
    {
        lock (_fileLock)
        {
            File.AppendAllText(ReportFilePath, text, ReportEncoding);
        }
    }

    private static bool IsLayoutContainer(Control control)
    {
        return control is TableLayoutPanel or FlowLayoutPanel or
            SplitContainer or GroupBox or Panel or MenuStrip;
    }

    private static string GetControlIdentity(Control control, int index)
    {
        string name = string.IsNullOrWhiteSpace(control.Name)
            ? $"{control.GetType().Name}[{index}]"
            : control.Name;

        if (control.Parent is not TableLayoutPanel tableLayoutPanel)
        {
            return name;
        }

        return $"{name}(column={tableLayoutPanel.GetColumn(control)}," +
            $"row={tableLayoutPanel.GetRow(control)})";
    }

    private static string FormatRectangle(Rectangle? rectangle)
    {
        return rectangle is { } value
            ? $"X={value.X}, Y={value.Y}, Width={value.Width}, Height={value.Height}"
            : "—";
    }

    private static string FormatSize(Size size)
    {
        return $"Width={size.Width}, Height={size.Height}";
    }

    private static string FormatSizeF(SizeF size)
    {
        return $"Width={FormatNumber(size.Width)}, " +
            $"Height={FormatNumber(size.Height)}";
    }

    private static string FormatPadding(Padding padding)
    {
        return $"Left={padding.Left}, Top={padding.Top}, " +
            $"Right={padding.Right}, Bottom={padding.Bottom}";
    }

    private static string FormatLogicalRectangle(
        Rectangle rectangle,
        float dpiScale)
    {
        return $"X={FormatLogicalValue(rectangle.X, dpiScale)}, " +
            $"Y={FormatLogicalValue(rectangle.Y, dpiScale)}, " +
            $"Width={FormatLogicalValue(rectangle.Width, dpiScale)}, " +
            $"Height={FormatLogicalValue(rectangle.Height, dpiScale)}";
    }

    private static string FormatLogicalSize(Size size, float dpiScale)
    {
        return $"Width={FormatLogicalValue(size.Width, dpiScale)}, " +
            $"Height={FormatLogicalValue(size.Height, dpiScale)}";
    }

    private static string FormatLogicalValue(int value, float dpiScale)
    {
        return dpiScale <= 0F
            ? "—"
            : FormatNumber(value / dpiScale);
    }

    private static string FormatNumber(float value)
    {
        return value.ToString("0.###", CultureInfo.InvariantCulture);
    }

    private static string ValueOrDash(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "—" : value;
    }
}
