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
    public int? RoleId { get; set; }
    public string? RoleNom { get; set; }

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
        Telephone = dto.Telephone,
        RoleId = dto.RoleId,
        RoleNom = dto.RoleNom
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

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
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
        UserFormPage.CurrentUser = existing;
        await Shell.Current.GoToAsync(nameof(UserFormPage));
    }
}
