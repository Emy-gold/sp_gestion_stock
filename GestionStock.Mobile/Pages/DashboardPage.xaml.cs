using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly ArticleApiService _articleApiService;
    private readonly CategoryArticleApiService _categoryApiService;
    private readonly FournisseurApiService _fournisseurApiService;
    private readonly AuthService _authService;

    public DashboardPage(
        ArticleApiService articleApiService,
        CategoryArticleApiService categoryApiService,
        FournisseurApiService fournisseurApiService,
        AuthService authService)
    {
        InitializeComponent();
        _articleApiService = articleApiService;
        _categoryApiService = categoryApiService;
        _fournisseurApiService = fournisseurApiService;
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        UpdateUserInfo();
        await LoadDashboardDataAsync();
    }

    private void UpdateUserInfo()
    {
        if (_authService.CurrentUser != null)
        {
            UserNameLabel.Text = $"{_authService.CurrentUser.Prenom} {_authService.CurrentUser.Nom}";
        }
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            var articlesTask = _articleApiService.GetArticlesAsync();
            var categoriesTask = _categoryApiService.GetCategoriesAsync();
            var fournisseursTask = _fournisseurApiService.GetFournisseursAsync();

            await Task.WhenAll(articlesTask, categoriesTask, fournisseursTask);

            var articles = await articlesTask;
            var categories = await categoriesTask;
            var fournisseurs = await fournisseursTask;

            TotalArticlesLabel.Text = articles.Count.ToString();
            TotalCategoriesLabel.Text = categories.Count.ToString();
            TotalFournisseursLabel.Text = fournisseurs.Count.ToString();

            var lowStockCount = articles.Count(a => a.StockActuel <= 5);
            StockAlertsLabel.Text = lowStockCount.ToString();

            RecentArticlesCollectionView.ItemsSource = articles.Take(5).ToList();
        }
        catch (Exception ex)
        {
            // Silently handle or fallback to 0
            System.Diagnostics.Debug.WriteLine($"Error loading dashboard: {ex.Message}");
        }
        finally
        {
            DashboardRefreshView.IsRefreshing = false;
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadDashboardDataAsync();
    }

    private async void OnAddArticleClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ArticleFormPage));
    }

    private async void OnViewArticlesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ArticlesPage");
    }
}
