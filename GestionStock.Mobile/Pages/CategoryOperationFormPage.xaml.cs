using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;
using System.Collections.ObjectModel;

namespace GestionStock.Mobile.Pages;

[QueryProperty(nameof(CategoryId), "categoryId")]
public partial class CategoryOperationFormPage : ContentPage
{
    private readonly CategoryOperationApiService _categoryApiService;
    private readonly AuthService _authService;
    private CategoryOperationDto? _existingCategory;

    public string? CategoryId { get; set; }

    private string _formTitle = "Nouvelle catégorie d'opération";
    public string FormTitle
    {
        get => _formTitle;
        set
        {
            _formTitle = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> AttributeNames { get; set; } = new();

    public CategoryOperationFormPage(CategoryOperationApiService categoryApiService, AuthService authService)
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
            await DisplayAlert("Accès refusé", "Seuls les administrateurs peuvent créer ou modifier des catégories d'opérations.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }
        
        if (!string.IsNullOrEmpty(CategoryId) && int.TryParse(CategoryId, out var id))
        {
            FormTitle = "Modifier la catégorie d'opération";
            DeleteButton.IsVisible = true;
            await LoadExistingCategoryAsync(id);
        }
        else
        {
            FormTitle = "Nouvelle catégorie d'opération";
            DeleteButton.IsVisible = false;
            AttributeNames.Clear();
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
            await DisplayAlert("Erreur", $"Impossible de charger la catégorie d'opération : {ex.Message}", "OK");
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

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_existingCategory == null)
            return;

        var confirm = await DisplayAlert("Confirmation", $"Voulez-vous vraiment supprimer la catégorie d'opération '{_existingCategory.Nom}' ?", "Supprimer", "Annuler");
        if (!confirm)
            return;

        try
        {
            var success = await _categoryApiService.DeleteCategoryAsync(_existingCategory.Id);
            if (success)
            {
                await DisplayAlert("Succès", "Catégorie d'opération supprimée avec succès.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Erreur", "Impossible de supprimer la catégorie d'opération car elle est probablement liée à des opérations existantes.", "OK");
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
            ErrorLabel.Text = "Le nom de la catégorie d'opération est obligatoire.";
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

            var attributesDict = new Dictionary<string, string>();
            foreach (var attrName in AttributeNames)
            {
                attributesDict[attrName] = string.Empty;
            }

            var dto = new CategoryOperationCreateDto
            {
                Nom = nom,
                Description = DescriptionEditor.Text?.Trim(),
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
}
