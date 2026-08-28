using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly ArticleApiService _articleApiService;
    private readonly CategoryArticleApiService _categoryApiService;
    private readonly FournisseurApiService _fournisseurApiService;
    private readonly OperationApiService _operationApiService;
    private readonly AuthService _authService;

    public DashboardPage(
        ArticleApiService articleApiService,
        CategoryArticleApiService categoryApiService,
        FournisseurApiService fournisseurApiService,
        OperationApiService operationApiService,
        AuthService authService)
    {
        InitializeComponent();
        _articleApiService = articleApiService;
        _categoryApiService = categoryApiService;
        _fournisseurApiService = fournisseurApiService;
        _operationApiService = operationApiService;
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
            var operationsTask = _operationApiService.GetOperationsAsync();

            await Task.WhenAll(articlesTask, categoriesTask, fournisseursTask, operationsTask);

            var articles = await articlesTask;
            var categories = await categoriesTask;
            var fournisseurs = await fournisseursTask;
            var operations = await operationsTask;

            TotalArticlesLabel.Text = articles.Count.ToString();
            TotalCategoriesLabel.Text = categories.Count.ToString();
            TotalFournisseursLabel.Text = fournisseurs.Count.ToString();
            TotalOperationsLabel.Text = operations.Count.ToString();

            RecentArticlesCollectionView.ItemsSource = articles.Take(5).ToList();
            RecentOperationsCollectionView.ItemsSource = operations
                .OrderByDescending(o => o.DateOperation)
                .Take(5)
                .ToList();
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

    private async void OnViewOperationsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//OperationsPage");
    }
}
