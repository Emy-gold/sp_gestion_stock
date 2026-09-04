using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

public partial class CategoryOperationsPage : ContentPage
{
    private readonly CategoryOperationApiService _categoryApiService;
    private readonly AuthService _authService;
    private List<CategoryOperationDto> _allCategories = new();

    public CategoryOperationsPage(CategoryOperationApiService categoryApiService, AuthService authService)
    {
        InitializeComponent();
        _categoryApiService = categoryApiService;
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var isAdmin = string.Equals(_authService.CurrentUser?.Role, "Administrateur", StringComparison.OrdinalIgnoreCase);
        AddButton.IsVisible = isAdmin;

        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            _allCategories = await _categoryApiService.GetCategoriesAsync();
            CollectionView.ItemsSource = _allCategories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les catégories d'opérations : {ex.Message}", "OK");
        }
        finally
        {
            RefreshView.IsRefreshing = false;
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue?.Trim().ToLower() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(query))
        {
            CollectionView.ItemsSource = _allCategories;
        }
        else
        {
            CollectionView.ItemsSource = _allCategories
                .Where(c => c.Nom.ToLower().Contains(query) || (c.Description?.ToLower().Contains(query) == true))
                .ToList();
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadCategoriesAsync();
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        var isAdmin = string.Equals(_authService.CurrentUser?.Role, "Administrateur", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin) return;

        await Shell.Current.GoToAsync(nameof(CategoryOperationFormPage));
    }

    private async void OnCategoryTapped(object? sender, TappedEventArgs e)
    {
        var isAdmin = string.Equals(_authService.CurrentUser?.Role, "Administrateur", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin) return;

        if (e.Parameter is CategoryOperationDto category)
        {
            await Shell.Current.GoToAsync($"{nameof(CategoryOperationFormPage)}?categoryId={category.Id}");
        }
    }
}
