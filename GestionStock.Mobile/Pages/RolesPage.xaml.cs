using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

public partial class RolesPage : ContentPage
{
    private readonly RoleApiService _roleApiService;
    private List<RoleDto> _roles = new();

    public RolesPage(RoleApiService roleApiService)
    {
        InitializeComponent();
        _roleApiService = roleApiService;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await LoadRolesAsync();
    }

    private async Task LoadRolesAsync()
    {
        try
        {
            _roles = await _roleApiService.GetRolesAsync();
            RolesCollectionView.ItemsSource = _roles;
            RolesCountLabel.Text = $"{_roles.Count} rôle{(_roles.Count > 1 ? "s" : "")}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les rôles : {ex.Message}", "OK");
        }
        finally
        {
            RolesRefreshView.IsRefreshing = false;
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadRolesAsync();
    }

    private async void OnAddRoleClicked(object? sender, EventArgs e)
    {
        RoleFormPage.CurrentRole = null;
        await Shell.Current.GoToAsync(nameof(RoleFormPage));
    }

    private async void OnEditRoleClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is RoleDto role)
        {
            RoleFormPage.CurrentRole = role;
            await Shell.Current.GoToAsync(nameof(RoleFormPage));
        }
    }

    private async void OnDeleteRoleClicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn || btn.CommandParameter is not RoleDto role)
            return;

        var confirm = await DisplayAlert(
            "Supprimer",
            $"Voulez-vous supprimer le rôle {role.Nom} ?",
            "Oui, supprimer",
            "Annuler");

        if (!confirm) return;

        try
        {
            await _roleApiService.DeleteRoleAsync(role.Id);
            await LoadRolesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }
}
