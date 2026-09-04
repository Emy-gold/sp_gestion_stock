using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;
using System.Collections.ObjectModel;

namespace GestionStock.Mobile.Pages;

public partial class UserFormPage : ContentPage
{
    private readonly UserApiService _userApiService;
    private readonly RoleApiService _roleApiService;
    private int? _userId;

    // Used to pass data from UsersPage
    public static UserViewModel? CurrentUser { get; set; }

    public string FormTitle => _userId.HasValue ? "Modifier l'utilisateur" : "Nouvel utilisateur";
    public ObservableCollection<RoleDto> Roles { get; set; } = new();

    public UserFormPage(UserApiService userApiService, RoleApiService roleApiService)
    {
        InitializeComponent();
        _userApiService = userApiService;
        _roleApiService = roleApiService;
        BindingContext = this;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Reset form
        _userId = null;
        PrenomEntry.Text = string.Empty;
        NomEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        TelephoneEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        RolePicker.SelectedItem = null;

        await LoadRolesAsync();

        // Load existing user data if editing
        if (CurrentUser != null)
        {
            _userId = CurrentUser.Id;
            PrenomEntry.Text = CurrentUser.Prenom;
            NomEntry.Text = CurrentUser.Nom;
            EmailEntry.Text = CurrentUser.Email;
            TelephoneEntry.Text = CurrentUser.Telephone ?? string.Empty;

            if (CurrentUser.RoleId.HasValue)
            {
                RolePicker.SelectedItem = Roles.FirstOrDefault(r => r.Id == CurrentUser.RoleId.Value);
            }
        }

        OnPropertyChanged(nameof(FormTitle));
    }

    private async Task LoadRolesAsync()
    {
        try
        {
            var roles = await _roleApiService.GetRolesAsync();
            Roles.Clear();
            foreach (var role in roles)
                Roles.Add(role);
            RolePicker.ItemsSource = Roles;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les rôles : {ex.Message}", "OK");
        }
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

        if (!_userId.HasValue && string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ShowError("Le mot de passe est obligatoire pour un nouvel utilisateur.");
            return;
        }

        var selectedRole = RolePicker.SelectedItem as RoleDto;

        var dto = new UserCreateDto
        {
            Nom = NomEntry.Text.Trim(),
            Prenom = PrenomEntry.Text.Trim(),
            Email = EmailEntry.Text.Trim(),
            Telephone = string.IsNullOrWhiteSpace(TelephoneEntry.Text) ? null : TelephoneEntry.Text.Trim(),
            MotDePasse = PasswordEntry.Text?.Trim() ?? string.Empty,
            RoleId = selectedRole?.Id
        };

        SetLoadingState(true);

        try
        {
            if (_userId.HasValue)
                await _userApiService.UpdateUserAsync(_userId.Value, dto);
            else
                await _userApiService.CreateUserAsync(dto);

            CurrentUser = null;
            await Shell.Current.GoToAsync("..");
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

