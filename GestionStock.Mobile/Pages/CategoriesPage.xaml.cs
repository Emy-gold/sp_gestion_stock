using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoryArticleApiService _categoryApiService;
    private List<CategoryArticleDto> _allCategories = new();

    public CategoriesPage(CategoryArticleApiService categoryApiService)
    {
        InitializeComponent();
        _categoryApiService = categoryApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            _allCategories = await _categoryApiService.GetCategoriesAsync();
            CategoriesCollectionView.ItemsSource = _allCategories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les catégories : {ex.Message}", "OK");
        }
        finally
        {
            CategoriesRefreshView.IsRefreshing = false;
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue?.Trim().ToLower() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(query))
        {
            CategoriesCollectionView.ItemsSource = _allCategories;
        }
        else
        {
            CategoriesCollectionView.ItemsSource = _allCategories
                .Where(c => c.Nom.ToLower().Contains(query) || (c.Description?.ToLower().Contains(query) == true))
                .ToList();
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadCategoriesAsync();
    }

    private async void OnAddCategoryClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CategoryFormPage));
    }

    private async void OnCategoryTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is CategoryArticleDto category)
        {
            await Shell.Current.GoToAsync($"{nameof(CategoryFormPage)}?categoryId={category.Id}");
        }
    }
}
