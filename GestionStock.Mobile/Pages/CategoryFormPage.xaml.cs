using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

[QueryProperty(nameof(CategoryId), "categoryId")]
public partial class CategoryFormPage : ContentPage
{
    private readonly CategoryArticleApiService _categoryApiService;
    private readonly AuthService _authService;

    private List<CategoryArticleDto> _categories = new();
    private CategoryArticleDto? _existingCategory;
    private string? _selectedImageBase64;
    public System.Collections.ObjectModel.ObservableCollection<string> AttributeNames { get; set; } = new();

    public string? CategoryId { get; set; }

    private string _formTitle = "Nouvelle catégorie";
    public string FormTitle
    {
        get => _formTitle;
        set
        {
            _formTitle = value;
            OnPropertyChanged();
        }
    }

    public CategoryFormPage(CategoryArticleApiService categoryApiService, AuthService authService)
    {
        InitializeComponent();
        _categoryApiService = categoryApiService;
        _authService = authService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var isAdmin = string.Equals(_authService.CurrentUser?.Role, "Administrateur", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin)
        {
            await DisplayAlert("Accès refusé", "Seuls les administrateurs peuvent créer ou modifier des catégories d'articles.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }
        
        if (!string.IsNullOrEmpty(CategoryId) && int.TryParse(CategoryId, out var id))
        {
            FormTitle = "Modifier la catégorie";
            DeleteButton.IsVisible = true;
            await LoadCategoriesAsync(id); // Load all except current one for parent picker
            await LoadExistingCategoryAsync(id);
        }
        else
        {
            FormTitle = "Nouvelle catégorie";
            DeleteButton.IsVisible = false;
            await LoadCategoriesAsync(null);
        }
    }

    private async Task LoadCategoriesAsync(int? excludeId)
    {
        try
        {
            var allCategories = await _categoryApiService.GetCategoriesAsync();
            // We can't set the category itself as its own parent
            _categories = allCategories.Where(c => c.Id != excludeId).ToList();
            ParentCategoryPicker.ItemsSource = _categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les catégories parentes : {ex.Message}", "OK");
        }
    }

    private async Task LoadExistingCategoryAsync(int id)
    {
        try
        {
            _existingCategory = await _categoryApiService.GetCategoryAsync(id);
            if (_existingCategory is null)
                return;

            NomEntry.Text = _existingCategory.Nom;
            DescriptionEditor.Text = _existingCategory.Description;

            if (!string.IsNullOrEmpty(_existingCategory.Image))
            {
                _selectedImageBase64 = _existingCategory.Image;
                ShowImagePreview(_selectedImageBase64);
            }

            if (_existingCategory.ParentId.HasValue)
            {
                var matchingCategory = _categories.FirstOrDefault(c => c.Id == _existingCategory.ParentId);
                if (matchingCategory != null)
                    ParentCategoryPicker.SelectedItem = matchingCategory;
            }

            AttributeNames.Clear();
            if (_existingCategory.Attributes != null)
            {
                foreach (var attrKey in _existingCategory.Attributes.Keys)
                {
                    AttributeNames.Add(attrKey);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger la catégorie : {ex.Message}", "OK");
        }
    }

    private void OnClearParentClicked(object? sender, EventArgs e)
    {
        ParentCategoryPicker.SelectedItem = null;
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
        CategoryImagePreview.Source = imageSource;
        CategoryImagePreview.IsVisible = true;
        ImagePlaceholderContainer.IsVisible = false;
        RemovePhotoButton.IsVisible = true;
    }

    private void OnRemovePhotoClicked(object? sender, EventArgs e)
    {
        _selectedImageBase64 = null;
        CategoryImagePreview.Source = null;
        CategoryImagePreview.IsVisible = false;
        ImagePlaceholderContainer.IsVisible = true;
        RemovePhotoButton.IsVisible = false;
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_existingCategory == null)
            return;

        var confirm = await DisplayAlert("Confirmation", $"Voulez-vous vraiment supprimer la catégorie '{_existingCategory.Nom}' ?", "Supprimer", "Annuler");
        if (!confirm)
            return;

        try
        {
            var success = await _categoryApiService.DeleteCategoryAsync(_existingCategory.Id);
            if (success)
            {
                await DisplayAlert("Succès", "Catégorie supprimée avec succès.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Erreur", "Impossible de supprimer la catégorie.", "OK");
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

        var nom = NomEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(nom))
        {
            ErrorLabel.Text = "Le nom de la catégorie est obligatoire.";
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
            var selectedParent = ParentCategoryPicker.SelectedItem as CategoryArticleDto;

            var attributesDict = new Dictionary<string, string>();
            foreach (var attrName in AttributeNames)
            {
                attributesDict[attrName] = string.Empty;
            }

            var dto = new CategoryArticleCreateDto
            {
                Nom = nom,
                Description = DescriptionEditor.Text?.Trim(),
                Image = _selectedImageBase64,
                ParentId = selectedParent?.Id,
                Attributes = attributesDict
            };

            if (_existingCategory is null)
            {
                success = await _categoryApiService.CreateCategoryAsync(dto);
            }
            else
            {
                success = await _categoryApiService.UpdateCategoryAsync(_existingCategory.Id, dto);
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

    private void OnAddAttributeClicked(object? sender, EventArgs e)
    {
        var attrName = NewAttributeEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(attrName))
            return;

        if (!AttributeNames.Contains(attrName))
        {
            AttributeNames.Add(attrName);
            NewAttributeEntry.Text = string.Empty;
        }
        else
        {
            DisplayAlert("Info", "Cet attribut existe déjà.", "OK");
        }
    }

    private void OnRemoveAttributeClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string attrName)
        {
            AttributeNames.Remove(attrName);
        }
    }
}
