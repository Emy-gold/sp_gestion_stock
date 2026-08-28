using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;
using System.Collections.ObjectModel;

namespace GestionStock.Mobile.Pages;

public partial class UserFormPage : ContentPage
{
    private readonly UserApiService _userApiService;
    private readonly RoleApiService _roleApiService;
    private int? _userId;

    public string FormTitle => _userId.HasValue ? "Modifier l'utilisateur" : "Nouvel utilisateur";
    public ObservableCollection<RoleDto> Roles { get; set; } = new();

    public UserFormPage(UserApiService userApiService, RoleApiService roleApiService)
    {
        InitializeComponent();
        _userApiService = userApiService;
        _roleApiService = roleApiService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRolesAsync();
    }

    private async Task LoadRolesAsync()
    {
        try
        {
            var roles = await _roleApiService.GetRolesAsync();
            Roles.Clear();
            foreach (var role in roles)
            {
                Roles.Add(role);
            }
            RolePicker.ItemsSource = Roles;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les rôles : {ex.Message}", "OK");
        }
    }

    public void LoadUser(UserViewModel user)
    {
        _userId = user.Id;
        PrenomEntry.Text = user.Prenom;
        NomEntry.Text = user.Nom;
        EmailEntry.Text = user.Email;
        TelephoneEntry.Text = user.Telephone;
        
        // Wait for roles to load to set the selected item, handled differently usually
        // but for simplicity, we can do it after load or manually select by ID
        Device.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(500); // Give it a bit of time to load roles
            if (user.RoleId.HasValue && Roles.Any())
            {
                RolePicker.SelectedItem = Roles.FirstOrDefault(r => r.Id == user.RoleId.Value);
            }
        });

        OnPropertyChanged(nameof(FormTitle));
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        ErrorContainer.IsVisible = false;
        ErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(NomEntry.Text) || string.IsNullOrWhiteSpace(PrenomEntry.Text) || string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            ShowError("Le prénom, nom et l'email sont obligatoires.");
            return;
        }

        var selectedRole = RolePicker.SelectedItem as RoleDto;

        var dto = new UserCreateDto
        {
            Nom = NomEntry.Text.Trim(),
            Prenom = PrenomEntry.Text.Trim(),
            Email = EmailEntry.Text.Trim(),
            Telephone = string.IsNullOrWhiteSpace(TelephoneEntry.Text) ? null : TelephoneEntry.Text.Trim(),
            RoleId = selectedRole?.Id
        };

        SetLoadingState(true);

        try
        {
            if (_userId.HasValue)
            {
                await _userApiService.UpdateUserAsync(_userId.Value, dto);
            }
            else
            {
                await _userApiService.CreateUserAsync(dto);
            }

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors de l'enregistrement : {ex.Message}");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
        ErrorContainer.IsVisible = true;
    }

    private void SetLoadingState(bool isLoading)
    {
        SaveButton.IsVisible = !isLoading;
        SavingIndicator.IsVisible = isLoading;
        SavingIndicator.IsRunning = isLoading;

        NomEntry.IsEnabled = !isLoading;
        PrenomEntry.IsEnabled = !isLoading;
        EmailEntry.IsEnabled = !isLoading;
        TelephoneEntry.IsEnabled = !isLoading;
        RolePicker.IsEnabled = !isLoading;
    }
}
