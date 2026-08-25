using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

public partial class ArticlesPage : ContentPage
{
    private readonly ArticleApiService _articleApiService;

    public ArticlesPage(ArticleApiService articleApiService)
    {
        InitializeComponent();
        _articleApiService = articleApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadArticlesAsync();
    }

    private async Task LoadArticlesAsync()
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            var articles = await _articleApiService.GetArticlesAsync();
            ArticlesCollectionView.ItemsSource = articles;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les articles : {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            RefreshViewControl.IsRefreshing = false;
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadArticlesAsync();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadArticlesAsync();
    }
}