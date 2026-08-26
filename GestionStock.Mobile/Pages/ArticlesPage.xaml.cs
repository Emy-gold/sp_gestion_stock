using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

public partial class ArticlesPage : ContentPage
{
    private readonly ArticleApiService _articleApiService;
    private readonly CategoryArticleApiService _categoryApiService;

    private List<ArticleDto> _allArticles = new();
    private List<CategoryArticleDto> _categories = new();
    private int? _selectedCategoryId = null;

    public ArticlesPage(ArticleApiService articleApiService, CategoryArticleApiService categoryApiService)
    {
        InitializeComponent();
        _articleApiService = articleApiService;
        _categoryApiService = categoryApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            var articlesTask = _articleApiService.GetArticlesAsync();
            var categoriesTask = _categoryApiService.GetCategoriesAsync();

            await Task.WhenAll(articlesTask, categoriesTask);

            _allArticles = await articlesTask;
            _categories = await categoriesTask;

            BuildCategoryChips();
            ApplyFilter();
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

    private void BuildCategoryChips()
    {
        CategoryChipsContainer.Children.Clear();

        // "Tous" chip
        var allButton = CreateChipButton("Tous", null, _selectedCategoryId == null);
        CategoryChipsContainer.Children.Add(allButton);

        foreach (var cat in _categories)
        {
            var isSelected = _selectedCategoryId == cat.Id;
            var chip = CreateChipButton(cat.Nom, cat.Id, isSelected);
            CategoryChipsContainer.Children.Add(chip);
        }
    }

    private Button CreateChipButton(string text, int? categoryId, bool isSelected)
    {
        var btn = new Button
        {
            Text = text,
            FontSize = 12,
            FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None,
            BackgroundColor = isSelected ? Color.FromArgb("#1A2B4A") : Color.FromArgb("#E2E8F0"),
            TextColor = isSelected ? Colors.White : Color.FromArgb("#1A2B4A"),
            CornerRadius = 16,
            HeightRequest = 32,
            Padding = new Thickness(14, 0)
        };

        btn.Clicked += (s, e) =>
        {
            _selectedCategoryId = categoryId;
            BuildCategoryChips();
            ApplyFilter();
        };

        return btn;
    }

    private void ApplyFilter()
    {
        var query = ArticleSearchBar.Text?.Trim().ToLower() ?? string.Empty;

        var filtered = _allArticles.AsEnumerable();

        if (_selectedCategoryId.HasValue)
        {
            filtered = filtered.Where(a => a.CategoryArticleId == _selectedCategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            filtered = filtered.Where(a =>
                a.Reference.ToLower().Contains(query) ||
                a.Designation.ToLower().Contains(query) ||
                (a.CodeBarre?.ToLower().Contains(query) == true) ||
                (a.CategoryArticleNom?.ToLower().Contains(query) == true));
        }

        ArticlesCollectionView.ItemsSource = filtered.ToList();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        ApplyFilter();
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadDataAsync();
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ArticleFormPage));
    }

    private async void OnArticleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border { BindingContext: ArticleDto article })
        {
            await Shell.Current.GoToAsync($"{nameof(ArticleFormPage)}?articleId={article.Id}");
        }
    }
}