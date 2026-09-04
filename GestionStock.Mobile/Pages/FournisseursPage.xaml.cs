using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

public partial class FournisseursPage : ContentPage
{
    private readonly FournisseurApiService _fournisseurApiService;
    private readonly AuthService _authService;
    private List<FournisseurDto> _allFournisseurs = new();

    public FournisseursPage(FournisseurApiService fournisseurApiService, AuthService authService)
    {
        InitializeComponent();
        _fournisseurApiService = fournisseurApiService;
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var isAdmin = string.Equals(_authService.CurrentUser?.Role, "Administrateur", StringComparison.OrdinalIgnoreCase);
        AddFournisseurButton.IsVisible = isAdmin;

        await LoadFournisseursAsync();
    }

    private async Task LoadFournisseursAsync()
    {
        try
        {
            _allFournisseurs = await _fournisseurApiService.GetFournisseursAsync();
            FournisseursCollectionView.ItemsSource = _allFournisseurs;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les fournisseurs : {ex.Message}", "OK");
        }
        finally
        {
            FournisseursRefreshView.IsRefreshing = false;
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue?.Trim().ToLower() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(query))
        {
            FournisseursCollectionView.ItemsSource = _allFournisseurs;
        }
        else
        {
            FournisseursCollectionView.ItemsSource = _allFournisseurs
                .Where(f => f.Nom.ToLower().Contains(query) || (f.Email?.ToLower().Contains(query) == true) || (f.Telephone?.Contains(query) == true))
                .ToList();
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadFournisseursAsync();
    }

    private async void OnAddFournisseurClicked(object? sender, EventArgs e)
    {
        var isAdmin = string.Equals(_authService.CurrentUser?.Role, "Administrateur", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin) return;

        await Shell.Current.GoToAsync(nameof(FournisseurFormPage));
    }

    private async void OnFournisseurTapped(object? sender, TappedEventArgs e)
    {
        var isAdmin = string.Equals(_authService.CurrentUser?.Role, "Administrateur", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin) return;

        if (e.Parameter is FournisseurDto fournisseur)
        {
            await Shell.Current.GoToAsync($"{nameof(FournisseurFormPage)}?fournisseurId={fournisseur.Id}");
        }
    }
}
