using System.Globalization;
using EtykietyIT.Models;
using EtykietyIT.Services;

namespace EtykietyIT.Forms;

public partial class ProfilesForm : Form
{
    private readonly LabelProfileService _labelProfileService;

    public ProfilesForm(LabelProfileService labelProfileService)
    {
        _labelProfileService = labelProfileService ??
            throw new ArgumentNullException(nameof(labelProfileService));

        InitializeComponent();

        Shown += ProfilesForm_Shown;
        profilesListView.SelectedIndexChanged += ProfilesListView_SelectedIndexChanged;
        newButton.Click += NewButton_Click;
        editButton.Click += EditButton_Click;
        duplicateButton.Click += DuplicateButton_Click;
        deleteButton.Click += DeleteButton_Click;
        closeButton.Click += CloseButton_Click;

        UpdateActionButtons();
    }

    private async void ProfilesForm_Shown(object? sender, EventArgs e)
    {
        await ReloadProfilesAsync();
    }

    private void ProfilesListView_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateActionButtons();
    }

    private async void NewButton_Click(object? sender, EventArgs e)
    {
        var draft = new LabelProfile
        {
            Id = $"user.{Guid.Empty:D}",
            Name = "Nowy profil"
        };

        using var editForm = new ProfileEditForm(draft, "Nowy profil etykiety");
        if (editForm.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            LabelProfile created = await _labelProfileService.CreateUserProfileAsync(
                editForm.Profile);
            await ReloadProfilesAsync(created.Id);
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się utworzyć profilu.\r\n\r\n{exception.Message}");
        }
    }

    private async void EditButton_Click(object? sender, EventArgs e)
    {
        LabelProfile? profile = GetSelectedProfile();
        if (profile is null || LabelProfileService.IsBuiltInProfile(profile))
        {
            return;
        }

        using var editForm = new ProfileEditForm(profile, "Edytuj profil etykiety");
        if (editForm.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            await _labelProfileService.UpdateUserProfileAsync(editForm.Profile);
            await ReloadProfilesAsync(editForm.Profile.Id);
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się zapisać profilu.\r\n\r\n{exception.Message}");
        }
    }

    private async void DuplicateButton_Click(object? sender, EventArgs e)
    {
        LabelProfile? profile = GetSelectedProfile();
        if (profile is null)
        {
            return;
        }

        try
        {
            LabelProfile clone = await _labelProfileService.CloneToUserProfileAsync(
                profile.Id);
            await ReloadProfilesAsync(clone.Id);
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się zduplikować profilu.\r\n\r\n{exception.Message}");
        }
    }

    private async void DeleteButton_Click(object? sender, EventArgs e)
    {
        LabelProfile? profile = GetSelectedProfile();
        if (profile is null || LabelProfileService.IsBuiltInProfile(profile))
        {
            return;
        }

        DialogResult answer = MessageBox.Show(
            this,
            $"Usunąć profil „{profile.Name}”?",
            "Usuwanie profilu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (answer != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await _labelProfileService.DeleteUserProfileAsync(profile.Id);
            await ReloadProfilesAsync();
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się usunąć profilu.\r\n\r\n{exception.Message}");
        }
    }

    private void CloseButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private async Task ReloadProfilesAsync(string? selectedId = null)
    {
        try
        {
            IReadOnlyList<LabelProfile> profiles =
                await _labelProfileService.GetAllAsync();

            profilesListView.BeginUpdate();
            try
            {
                profilesListView.Items.Clear();
                foreach (LabelProfile profile in profiles)
                {
                    bool isBuiltIn = LabelProfileService.IsBuiltInProfile(profile);
                    var item = new ListViewItem(profile.Name)
                    {
                        Tag = profile
                    };
                    item.SubItems.Add(isBuiltIn ? "Built-in" : "User");
                    item.SubItems.Add(string.Format(
                        CultureInfo.CurrentCulture,
                        "{0:0.0} × {1:0.0} mm",
                        profile.WidthMm,
                        profile.HeightMm));
                    item.SubItems.Add($"{profile.Columns} × {profile.Rows}");
                    profilesListView.Items.Add(item);

                    if (string.Equals(
                        profile.Id,
                        selectedId,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        item.Selected = true;
                    }
                }
            }
            finally
            {
                profilesListView.EndUpdate();
            }

            UpdateActionButtons();
        }
        catch (Exception exception)
        {
            ShowError($"Nie udało się wczytać profili.\r\n\r\n{exception.Message}");
        }
    }

    private LabelProfile? GetSelectedProfile()
    {
        return profilesListView.SelectedItems.Count == 1
            ? profilesListView.SelectedItems[0].Tag as LabelProfile
            : null;
    }

    private void UpdateActionButtons()
    {
        LabelProfile? profile = GetSelectedProfile();
        bool hasProfile = profile is not null;
        bool isBuiltIn = profile is not null &&
            LabelProfileService.IsBuiltInProfile(profile);

        editButton.Enabled = hasProfile && !isBuiltIn;
        duplicateButton.Enabled = hasProfile;
        deleteButton.Enabled = hasProfile && !isBuiltIn;
    }

    private void ShowError(string message)
    {
        MessageBox.Show(
            this,
            message,
            "Profile etykiet",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
