using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

[QueryProperty(nameof(ArticleId), "articleId")]
public partial class ArticleFormPage : ContentPage
{
    private readonly ArticleApiService _articleApiService;
    private readonly CategoryArticleApiService _categoryApiService;

    private List<CategoryArticleDto> _categories = new();
    private ArticleDto? _existingArticle;

    public string? ArticleId { get; set; }

    public string Title { get; set; } = "Nouvel article";

    public ArticleFormPage(ArticleApiService articleApiService, CategoryArticleApiService categoryApiService)
    {
        InitializeComponent();
        _articleApiService = articleApiService;
        _categoryApiService = categoryApiService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategoriesAsync();

        if (!string.IsNullOrEmpty(ArticleId) && int.TryParse(ArticleId, out var id))
        {
            Title = "Modifier l'article";
            await LoadExistingArticleAsync(id);
        }
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            _categories = await _categoryApiService.GetCategoriesAsync();
            CategoryPicker.ItemsSource = _categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les catégories : {ex.Message}", "OK");
        }
    }

    private async Task LoadExistingArticleAsync(int id)
    {
        try
        {
            _existingArticle = await _articleApiService.GetArticleAsync(id);
            if (_existingArticle is null)
                return;

            ReferenceEntry.Text = _existingArticle.Reference;
            DesignationEntry.Text = _existingArticle.Designation;
            DescriptionEditor.Text = _existingArticle.Description;
            CodeBarreEntry.Text = _existingArticle.CodeBarre;

            var matchingCategory = _categories.FirstOrDefault(c => c.Id == _existingArticle.CategoryArticleId);
            if (matchingCategory != null)
                CategoryPicker.SelectedItem = matchingCategory;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger l'article : {ex.Message}", "OK");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        ErrorContainer.IsVisible = false;

        if (string.IsNullOrWhiteSpace(ReferenceEntry.Text) ||
            string.IsNullOrWhiteSpace(DesignationEntry.Text) ||
            CategoryPicker.SelectedItem is not CategoryArticleDto selectedCategory)
        {
            ErrorLabel.Text = "Référence, désignation et catégorie sont obligatoires.";
            ErrorLabel.IsVisible = true;
            ErrorContainer.IsVisible = true;
            return;
        }

        SavingIndicator.IsVisible = true;
        SavingIndicator.IsRunning = true;
        SaveButton.IsEnabled = false;

        try
        {
            bool success;

            if (_existingArticle is null)
            {
                var createDto = new ArticleCreateDto
                {
                    Reference = ReferenceEntry.Text,
                    Designation = DesignationEntry.Text,
                    Description = DescriptionEditor.Text,
                    CodeBarre = CodeBarreEntry.Text,
                    CategoryArticleId = selectedCategory.Id
                };
                success = await _articleApiService.CreateArticleAsync(createDto);
            }
            else
            {
                var updateDto = new ArticleUpdateDto
                {
                    Reference = ReferenceEntry.Text,
                    Designation = DesignationEntry.Text,
                    Description = DescriptionEditor.Text,
                    CodeBarre = CodeBarreEntry.Text,
                    CategoryArticleId = selectedCategory.Id,
                    Actif = true
                };
                success = await _articleApiService.UpdateArticleAsync(_existingArticle.Id, updateDto);
            }

            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorLabel.Text = "Échec de l'enregistrement. Vérifie les données saisies.";
                ErrorLabel.IsVisible = true;
                ErrorContainer.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Erreur : {ex.Message}";
            ErrorLabel.IsVisible = true;
            ErrorContainer.IsVisible = true;
        }
        finally
        {
            SavingIndicator.IsVisible = false;
            SavingIndicator.IsRunning = false;
            SaveButton.IsEnabled = true;
        }
    }
}