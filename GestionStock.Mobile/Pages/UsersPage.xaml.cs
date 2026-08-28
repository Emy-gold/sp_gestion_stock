using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

// ViewModel enrichi pour l'affichage dans la liste
public class UserViewModel
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }

    public string NomComplet => $"{Prenom} {Nom}".Trim();

    public string Initiales
    {
        get
        {
            var ini = string.Empty;
            if (!string.IsNullOrEmpty(Prenom)) ini += Prenom[0];
            if (!string.IsNullOrEmpty(Nom)) ini += Nom[0];
            return string.IsNullOrEmpty(ini) ? "?" : ini.ToUpper();
        }
    }

    public static UserViewModel FromDto(UserDto dto) => new()
    {
        Id = dto.Id,
        Nom = dto.Nom,
        Prenom = dto.Prenom,
        Email = dto.Email,
        Telephone = dto.Telephone
    };
}

public partial class UsersPage : ContentPage
{
    private readonly UserApiService _userApiService;
    private List<UserViewModel> _users = new();

    public UsersPage(UserApiService userApiService)
    {
        InitializeComponent();
        _userApiService = userApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            var dtos = await _userApiService.GetUsersAsync();
            _users = dtos.Select(UserViewModel.FromDto).ToList();
            UsersCollectionView.ItemsSource = _users;
            UsersCountLabel.Text = $"{_users.Count} utilisateur{(_users.Count > 1 ? "s" : "")}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les utilisateurs : {ex.Message}", "OK");
        }
        finally
        {
            UsersRefreshView.IsRefreshing = false;
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadUsersAsync();
    }

    private async void OnAddUserClicked(object? sender, EventArgs e)
    {
        await ShowUserFormAsync(null);
    }

    private async void OnEditUserClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UserViewModel user)
            await ShowUserFormAsync(user);
    }

    private async void OnDeleteUserClicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn || btn.CommandParameter is not UserViewModel user)
            return;

        var confirm = await DisplayAlert(
            "Supprimer",
            $"Voulez-vous supprimer l'utilisateur {user.NomComplet} ?",
            "Oui, supprimer",
            "Annuler");

        if (!confirm) return;

        try
        {
            await _userApiService.DeleteUserAsync(user.Id);
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private async Task ShowUserFormAsync(UserViewModel? existing)
    {
        var isEdit = existing != null;
        var title = isEdit ? "Modifier l'utilisateur" : "Nouvel utilisateur";

        // Prenom
        var prenom = await DisplayPromptAsync(title, "Prénom :",
            initialValue: existing?.Prenom ?? string.Empty,
            placeholder: "ex: Jean",
            maxLength: 50,
            keyboard: Keyboard.Text);
        if (prenom == null) return;

        // Nom
        var nom = await DisplayPromptAsync(title, "Nom :",
            initialValue: existing?.Nom ?? string.Empty,
            placeholder: "ex: Dupont",
            maxLength: 50,
            keyboard: Keyboard.Text);
        if (nom == null) return;

        // Email
        var email = await DisplayPromptAsync(title, "Email :",
            initialValue: existing?.Email ?? string.Empty,
            placeholder: "ex: jean.dupont@email.com",
            maxLength: 100,
            keyboard: Keyboard.Email);
        if (email == null) return;

        // Téléphone
        var telephone = await DisplayPromptAsync(title, "Téléphone (optionnel) :",
            initialValue: existing?.Telephone ?? string.Empty,
            placeholder: "ex: +212 6 00 00 00 00",
            maxLength: 20,
            keyboard: Keyboard.Telephone);
        if (telephone == null) return;

        if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Validation", "Le nom et l'email sont obligatoires.", "OK");
            return;
        }

        var dto = new UserCreateDto
        {
            Nom = nom.Trim(),
            Prenom = prenom.Trim(),
            Email = email.Trim(),
            Telephone = string.IsNullOrWhiteSpace(telephone) ? null : telephone.Trim()
        };

        try
        {
            if (isEdit)
                await _userApiService.UpdateUserAsync(existing!.Id, dto);
            else
                await _userApiService.CreateUserAsync(dto);

            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }
}
