using System.Diagnostics;
using EtykietyIT.Services;

namespace EtykietyIT.Forms;

public sealed class AboutForm : Form
{
    private const string CopyrightText = "Copyright © 2026";

    private readonly ApplicationVersionService _applicationVersionService;

    public AboutForm(ApplicationVersionService applicationVersionService)
    {
        _applicationVersionService = applicationVersionService ??
            throw new ArgumentNullException(nameof(applicationVersionService));

        InitializeLayout();
        ApplicationIconProvider.Apply(this);
    }

    private void InitializeLayout()
    {
        SuspendLayout();

        Text = "O programie — Etykiety IT";
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(560, 420);
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(600, 430);

        var layout = new TableLayoutPanel
        {
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Padding = new Padding(24, 20, 24, 20),
            RowCount = 8
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var titleLabel = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 4),
            Text = "Etykiety IT"
        };
        var versionLabel = new Label
        {
            AutoSize = true,
            Margin = new Padding(2, 0, 0, 18),
            Text = $"Wersja {_applicationVersionService.UserVersion}"
        };
        var descriptionLabel = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 18),
            Text = "Program do tworzenia i drukowania etykiet inwentarzowych " +
                "dla urządzeń i zasobów IT."
        };
        var detailsLabel = new Label
        {
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 14),
            Text = "Technologia: C# / .NET 10 / Windows Forms\r\n" +
                "Licencja: MIT\r\n" +
                CopyrightText
        };
        const string repositoryLabelPrefix = "GitHub: ";
        string repositoryLinkText =
            repositoryLabelPrefix + ApplicationLinks.RepositoryUrl;
        var repositoryLinkLabel = new LinkLabel
        {
            AutoSize = true,
            LinkArea = new LinkArea(
                repositoryLabelPrefix.Length,
                ApplicationLinks.RepositoryUrl.Length),
            Margin = new Padding(0, 0, 0, 14),
            Text = repositoryLinkText
        };
        repositoryLinkLabel.LinkClicked += (_, _) => OpenRepository();
        var librariesGroupBox = new GroupBox
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 12),
            Padding = new Padding(12, 8, 12, 10),
            Text = "Licencje bibliotek"
        };
        var librariesLabel = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            Text = "DocumentFormat.OpenXml 3.5.1 — MIT\r\n" +
                "Net.Codecrete.QrCodeGenerator 3.1.0 — MIT\r\n" +
                "Szczegółowe informacje znajdują się w THIRD-PARTY-NOTICES.md."
        };
        librariesGroupBox.Controls.Add(librariesLabel);

        var buttonsPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Margin = new Padding(0),
            WrapContents = false
        };
        var closeButton = CreateButton("Zamknij", new Size(100, 34));
        closeButton.DialogResult = DialogResult.Cancel;
        var thirdPartyButton = CreateButton(
            "Licencje bibliotek",
            new Size(150, 34));
        var licenseButton = CreateButton("Licencja", new Size(100, 34));

        licenseButton.Click += (_, _) => ShowTextFileDialog(
            "Licencja MIT",
            "LICENSE");
        thirdPartyButton.Click += (_, _) => ShowTextFileDialog(
            "Licencje bibliotek",
            "THIRD-PARTY-NOTICES.md");
        closeButton.Click += (_, _) => Close();

        buttonsPanel.Controls.Add(closeButton);
        buttonsPanel.Controls.Add(thirdPartyButton);
        buttonsPanel.Controls.Add(licenseButton);

        layout.Controls.Add(titleLabel, 0, 0);
        layout.Controls.Add(versionLabel, 0, 1);
        layout.Controls.Add(descriptionLabel, 0, 2);
        layout.Controls.Add(detailsLabel, 0, 3);
        layout.Controls.Add(repositoryLinkLabel, 0, 4);
        layout.Controls.Add(librariesGroupBox, 0, 5);
        layout.Controls.Add(buttonsPanel, 0, 7);

        layout.SizeChanged += (_, _) =>
        {
            int availableWidth = Math.Max(
                1,
                layout.ClientSize.Width - layout.Padding.Horizontal);
            descriptionLabel.MaximumSize = new Size(availableWidth, 0);
            librariesLabel.MaximumSize = new Size(availableWidth, 0);
        };

        AcceptButton = closeButton;
        CancelButton = closeButton;
        Controls.Add(layout);
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        ResumeLayout(false);
        PerformLayout();
    }

    private void OpenRepository()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = ApplicationLinks.RepositoryUrl,
                UseShellExecute = true
            });
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                $"Nie udało się otworzyć repozytorium.\r\n\r\n{exception.Message}",
                "Etykiety IT",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static Button CreateButton(string text, Size minimumSize)
    {
        return new Button
        {
            AutoSize = false,
            MinimumSize = minimumSize,
            Size = minimumSize,
            Text = text,
            UseVisualStyleBackColor = true
        };
    }

    private void ShowTextFileDialog(string title, string fileName)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        string text = File.Exists(filePath)
            ? NormalizeLineEndings(File.ReadAllText(filePath))
            : $"Nie znaleziono pliku {fileName} w katalogu aplikacji.";

        using var dialog = new Form();
        ApplicationIconProvider.Apply(dialog);
        dialog.SuspendLayout();
        dialog.Text = title;
        dialog.Font = new Font("Segoe UI", 9F);
        dialog.MaximizeBox = true;
        dialog.MinimizeBox = false;
        dialog.ShowInTaskbar = false;
        dialog.StartPosition = FormStartPosition.CenterParent;
        dialog.ClientSize = new Size(760, 560);
        dialog.MinimumSize = new Size(620, 460);
        var textBox = new RichTextBox
        {
            BackColor = SystemColors.Window,
            DetectUrls = true,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 9F),
            ReadOnly = true,
            ScrollBars = RichTextBoxScrollBars.Vertical,
            Text = text,
            WordWrap = true
        };
        var closeButton = CreateButton("Zamknij", new Size(100, 34));
        closeButton.DialogResult = DialogResult.Cancel;
        closeButton.Anchor = AnchorStyles.Right;

        var layout = new TableLayoutPanel
        {
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            RowCount = 2
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(textBox, 0, 0);
        layout.Controls.Add(closeButton, 0, 1);

        dialog.CancelButton = closeButton;
        dialog.Controls.Add(layout);
        dialog.AutoScaleDimensions = new SizeF(96F, 96F);
        dialog.AutoScaleMode = AutoScaleMode.Dpi;
        dialog.ResumeLayout(false);
        dialog.PerformLayout();
        dialog.ShowDialog(this);
    }

    private static string NormalizeLineEndings(string text)
    {
        return text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Replace("\n", Environment.NewLine, StringComparison.Ordinal);
    }
}
