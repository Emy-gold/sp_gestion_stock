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
    private string? _selectedImageBase64;

    public string? ArticleId { get; set; }

    private string _formTitle = "Nouvel article";
    public string FormTitle
    {
        get => _formTitle;
        set
        {
            _formTitle = value;
            OnPropertyChanged();
        }
    }

    public ArticleFormPage(ArticleApiService articleApiService, CategoryArticleApiService categoryApiService)
    {
        InitializeComponent();
        _articleApiService = articleApiService;
        _categoryApiService = categoryApiService;
        BindingContext = this;

        CategoryPicker.SelectedIndexChanged += OnCategoryChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategoriesAsync();

        if (!string.IsNullOrEmpty(ArticleId) && int.TryParse(ArticleId, out var id))
        {
            FormTitle = "Modifier l'article";
            DeleteButton.IsVisible = true;
            await LoadExistingArticleAsync(id);
        }
        else
        {
            FormTitle = "Nouvel article";
            DeleteButton.IsVisible = false;
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

            if (!string.IsNullOrEmpty(_existingArticle.Image))
            {
                _selectedImageBase64 = _existingArticle.Image;
                ShowImagePreview(_selectedImageBase64);
            }

            var matchingCategory = _categories.FirstOrDefault(c => c.Id == _existingArticle.CategoryArticleId);
            if (matchingCategory != null)
                CategoryPicker.SelectedItem = matchingCategory;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger l'article : {ex.Message}", "OK");
        }
    }

    private async void OnPickPhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    await ProcessSelectedPhoto(photo);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de sélectionner la photo : {ex.Message}", "OK");
        }
    }

    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    await ProcessSelectedPhoto(photo);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de capturer la photo : {ex.Message}", "OK");
        }
    }

    private async Task ProcessSelectedPhoto(FileResult photo)
    {
        using var stream = await photo.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var bytes = memoryStream.ToArray();

        var contentType = photo.ContentType ?? "image/jpeg";
        _selectedImageBase64 = $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";

        ShowImagePreview(_selectedImageBase64);
    }

    private void ShowImagePreview(string imageSource)
    {
        ArticleImagePreview.Source = imageSource;
        ArticleImagePreview.IsVisible = true;
        ImagePlaceholderContainer.IsVisible = false;
        RemovePhotoButton.IsVisible = true;
    }

    private void OnRemovePhotoClicked(object? sender, EventArgs e)
    {
        _selectedImageBase64 = null;
        ArticleImagePreview.Source = null;
        ArticleImagePreview.IsVisible = false;
        ImagePlaceholderContainer.IsVisible = true;
        RemovePhotoButton.IsVisible = false;
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_existingArticle == null)
            return;

        var confirm = await DisplayAlert("Confirmation", $"Voulez-vous vraiment supprimer l'article '{_existingArticle.Designation}' ?", "Supprimer", "Annuler");
        if (!confirm)
            return;

        try
        {
            var success = await _articleApiService.DeleteArticleAsync(_existingArticle.Id);
            if (success)
            {
                await DisplayAlert("Succès", "Article supprimé avec succès.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Erreur", "Impossible de supprimer l'article.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        ErrorContainer.IsVisible = false;

        var reference = ReferenceEntry.Text?.Trim();
        var designation = DesignationEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(reference) ||
            string.IsNullOrWhiteSpace(designation) ||
            CategoryPicker.SelectedItem is not CategoryArticleDto selectedCategory)
        {
            ErrorLabel.Text = "La référence, la désignation et la catégorie sont obligatoires.";
            ErrorLabel.IsVisible = true;
            ErrorContainer.IsVisible = true;
            return;
        }

        SavingIndicator.IsVisible = true;
        SavingIndicator.IsRunning = true;
        SaveButton.IsEnabled = false;

        var attributeValues = new Dictionary<string, string>();
        foreach (var child in DynamicFieldsStack.Children)
        {
            if (child is VerticalStackLayout fieldStack)
            {
                var border = fieldStack.Children.OfType<Border>().FirstOrDefault();
                if (border?.Content is Entry entry && !string.IsNullOrEmpty(entry.AutomationId))
                {
                    attributeValues[entry.AutomationId] = entry.Text?.Trim() ?? string.Empty;
                }
            }
        }

        try
        {
            bool success;

            if (_existingArticle is null)
            {
                var createDto = new ArticleCreateDto
                {
                    Reference = reference,
                    Designation = designation,
                    Description = DescriptionEditor.Text?.Trim(),
                    CodeBarre = CodeBarreEntry.Text?.Trim(),
                    Image = _selectedImageBase64,
                    CategoryArticleId = selectedCategory.Id,
                    AttributeValues = attributeValues
                };
                success = await _articleApiService.CreateArticleAsync(createDto);
            }
            else
            {
                var updateDto = new ArticleUpdateDto
                {
                    Reference = reference,
                    Designation = designation,
                    Description = DescriptionEditor.Text?.Trim(),
                    CodeBarre = CodeBarreEntry.Text?.Trim(),
                    Image = _selectedImageBase64,
                    CategoryArticleId = selectedCategory.Id,
                    Actif = true,
                    AttributeValues = attributeValues
                };
                success = await _articleApiService.UpdateArticleAsync(_existingArticle.Id, updateDto);
            }

            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorLabel.Text = "Échec de l'enregistrement. Vérifiez les informations saisies.";
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

    private void OnCategoryChanged(object? sender, EventArgs e)
    {
        DynamicFieldsStack.Children.Clear();

        if (CategoryPicker.SelectedItem is not CategoryArticleDto selectedCategory || 
            selectedCategory.Attributes == null || 
            selectedCategory.Attributes.Count == 0)
        {
            DynamicAttributesContainer.IsVisible = false;
            return;
        }

        DynamicAttributesContainer.IsVisible = true;

        foreach (var attrName in selectedCategory.Attributes.Keys)
        {
            var fieldStack = new VerticalStackLayout { Spacing = 6 };

            var label = new Label
            {
                Text = attrName.ToUpper(),
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#374151"),
                CharacterSpacing = 1
            };

            var border = new Border
            {
                BackgroundColor = Colors.White,
                Padding = new Thickness(14, 0),
                StrokeThickness = 1.5,
                Stroke = Color.FromArgb("#E2E8F0"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 }
            };

            var entry = new Entry
            {
                Placeholder = $"Saisir {attrName.ToLower()}",
                PlaceholderColor = Color.FromArgb("#94A3B8"),
                TextColor = Color.FromArgb("#1E293B"),
                BackgroundColor = Colors.Transparent,
                HeightRequest = 48,
                AutomationId = attrName
            };

            if (_existingArticle?.AttributeValues != null && 
                _existingArticle.AttributeValues.TryGetValue(attrName, out var existingValue))
            {
                entry.Text = existingValue;
            }

            border.Content = entry;
            fieldStack.Children.Add(label);
            fieldStack.Children.Add(border);

            DynamicFieldsStack.Children.Add(fieldStack);
        }
    }
}