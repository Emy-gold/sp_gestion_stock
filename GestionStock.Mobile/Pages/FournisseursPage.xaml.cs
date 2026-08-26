using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

public partial class FournisseursPage : ContentPage
{
    private readonly FournisseurApiService _fournisseurApiService;
    private List<FournisseurDto> _allFournisseurs = new();

    public FournisseursPage(FournisseurApiService fournisseurApiService)
    {
        InitializeComponent();
        _fournisseurApiService = fournisseurApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
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
}
