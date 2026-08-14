using EtykietyIT.Models;
using EtykietyIT.Services;

namespace EtykietyIT.Forms;

public partial class OrganizationsForm : Form
{
    private readonly OrganizationProfileService _organizationProfileService;
    private readonly LabelProfileService _labelProfileService;
    private readonly SettingsService _settingsService;
    private readonly string[] _installedPrinters;

    private IReadOnlyList<OrganizationProfile> _profiles =
        Array.Empty<OrganizationProfile>();
    private IReadOnlyList<LabelProfile> _labelProfiles =
        Array.Empty<LabelProfile>();

    public OrganizationsForm(
        OrganizationProfileService organizationProfileService,
        LabelProfileService labelProfileService,
        SettingsService settingsService,
        ApplicationSettings settings,
        IEnumerable<string> installedPrinters)
    {
        _organizationProfileService = organizationProfileService ??
            throw new ArgumentNullException(nameof(organizationProfileService));
        _labelProfileService = labelProfileService ??
            throw new ArgumentNullException(nameof(labelProfileService));
        _settingsService = settingsService ??
            throw new ArgumentNullException(nameof(settingsService));
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        Settings.Validate();
        ArgumentNullException.ThrowIfNull(installedPrinters);
        _installedPrinters = installedPrinters.ToArray();

        InitializeComponent();

        Shown += OrganizationsForm_Shown;
        organizationsListView.SelectedIndexChanged +=
            OrganizationsListView_SelectedIndexChanged;
        newButton.Click += NewButton_Click;
        editButton.Click += EditButton_Click;
        duplicateButton.Click += DuplicateButton_Click;
        deleteButton.Click += DeleteButton_Click;
        closeButton.Click += CloseButton_Click;
        UpdateActionButtons();
    }

    public ApplicationSettings Settings { get; private set; }

    private async void OrganizationsForm_Shown(object? sender, EventArgs e)
    {
        try
        {
            _labelProfiles = await _labelProfileService.GetAllAsync();
            await ReloadOrganizationsAsync(Settings.ActiveOrganizationProfileId);
        }
        catch (Exception exception)
        {
            ShowError(
                $"Nie udało się wczytać danych organizacji.\r\n\r\n{exception.Message}");
        }
    }

    private void OrganizationsListView_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        UpdateActionButtons();
    }

    private async void NewButton_Click(object? sender, EventArgs e)
    {
        var draft = new OrganizationProfile
        {
            Id = $"{OrganizationProfile.IdPrefix}{Guid.Empty:D}"
        };
        using var editForm = new OrganizationEditForm(
            draft,
            _labelProfiles,
            _installedPrinters,
            "Nowy profil organizacji");
        if (editForm.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            OrganizationProfile created =
                await _organizationProfileService.CreateAsync(editForm.Profile);
            await ReloadOrganizationsAsync(created.Id);
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się utworzyć organizacji.\r\n\r\n{exception.Message}");
        }
    }

    private async void EditButton_Click(object? sender, EventArgs e)
    {
        OrganizationProfile? profile = GetSelectedProfile();
        if (profile is null)
        {
            return;
        }

        using var editForm = new OrganizationEditForm(
            profile,
            _labelProfiles,
            _installedPrinters,
            "Edytuj profil organizacji");
        if (editForm.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            await _organizationProfileService.UpdateAsync(editForm.Profile);
            await ReloadOrganizationsAsync(editForm.Profile.Id);
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się zapisać organizacji.\r\n\r\n{exception.Message}");
        }
    }

    private async void DuplicateButton_Click(object? sender, EventArgs e)
    {
        OrganizationProfile? profile = GetSelectedProfile();
        if (profile is null)
        {
            return;
        }

        try
        {
            OrganizationProfile duplicate =
                await _organizationProfileService.DuplicateAsync(profile.Id);
            await ReloadOrganizationsAsync(duplicate.Id);
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się zduplikować organizacji.\r\n\r\n{exception.Message}");
        }
    }

    private async void DeleteButton_Click(object? sender, EventArgs e)
    {
        OrganizationProfile? profile = GetSelectedProfile();
        if (profile is null || _profiles.Count <= 1)
        {
            return;
        }

        DialogResult answer = MessageBox.Show(
            this,
            $"Usunąć profil organizacji „{profile.Name}”?",
            "Usuwanie organizacji",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (answer != DialogResult.Yes)
        {
            return;
        }

        ApplicationSettings previousSettings = Settings;
        bool changedActiveOrganization = string.Equals(
            profile.Id,
            Settings.ActiveOrganizationProfileId,
            StringComparison.Ordinal);

        try
        {
            if (changedActiveOrganization)
            {
                OrganizationProfile replacement = _profiles.First(item =>
                    !string.Equals(item.Id, profile.Id, StringComparison.Ordinal));
                ApplicationSettings updatedSettings = Settings with
                {
                    ActiveOrganizationProfileId = replacement.Id
                };
                await _settingsService.SaveAsync(updatedSettings);
                Settings = updatedSettings;
            }

            await _organizationProfileService.DeleteAsync(profile.Id);
            await ReloadOrganizationsAsync(Settings.ActiveOrganizationProfileId);
        }
        catch (Exception exception)
        {
            if (changedActiveOrganization)
            {
                try
                {
                    await _settingsService.SaveAsync(previousSettings);
                    Settings = previousSettings;
                }
                catch (Exception rollbackException)
                {
                    ShowError(
                        "Usuwanie organizacji nie powiodło się, a przywrócenie " +
                        "poprzedniego wyboru również zakończyło się błędem.\r\n\r\n" +
                        $"{exception.Message}\r\n\r\n{rollbackException.Message}");
                    return;
                }
            }

            ShowError($"Nie udało się usunąć organizacji.\r\n\r\n{exception.Message}");
        }
    }

    private void CloseButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private async Task ReloadOrganizationsAsync(string? selectedId)
    {
        OrganizationProfileReadResult result =
            await _organizationProfileService.GetAllAsync();
        _profiles = result.Profiles;

        skippedFilesLabel.Visible = result.SkippedFileCount > 0;
        skippedFilesLabel.Text =
            $"Pominięte uszkodzone pliki: {result.SkippedFileCount}";

        organizationsListView.BeginUpdate();
        try
        {
            organizationsListView.Items.Clear();
            foreach (OrganizationProfile profile in _profiles)
            {
                var item = new ListViewItem(profile.Name)
                {
                    Tag = profile
                };
                item.SubItems.Add(profile.CompanyName);
                item.SubItems.Add(
                    $"{profile.AssetId.Prefix} / {profile.AssetId.Digits} cyfr");
                item.SubItems.Add(profile.NextAssetNumber.ToString());
                item.SubItems.Add(GetLabelProfileName(profile.DefaultLabelProfileId));
                item.SubItems.Add(profile.DefaultPrinterName ?? "Automatyczna");
                organizationsListView.Items.Add(item);

                if (string.Equals(
                    profile.Id,
                    selectedId,
                    StringComparison.Ordinal))
                {
                    item.Selected = true;
                }
            }
        }
        finally
        {
            organizationsListView.EndUpdate();
        }

        UpdateActionButtons();
    }

    private string GetLabelProfileName(string profileId)
    {
        return _labelProfiles.FirstOrDefault(profile => string.Equals(
            profile.Id,
            profileId,
            StringComparison.OrdinalIgnoreCase))?.Name ?? profileId;
    }

    private OrganizationProfile? GetSelectedProfile()
    {
        return organizationsListView.SelectedItems.Count == 1
            ? organizationsListView.SelectedItems[0].Tag as OrganizationProfile
            : null;
    }

    private void UpdateActionButtons()
    {
        bool hasSelection = GetSelectedProfile() is not null;
        editButton.Enabled = hasSelection;
        duplicateButton.Enabled = hasSelection;
        deleteButton.Enabled = hasSelection && _profiles.Count > 1;
    }

    private void ShowError(string message)
    {
        MessageBox.Show(
            this,
            message,
            "Profile organizacji",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
