using System.Collections.ObjectModel;
using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

/// <summary>
/// Code-behind for the stock operation creation form.
/// </summary>
public partial class OperationFormPage : ContentPage
{
    // ─── Services injected via DI ─────────────────────────────────────────
    private readonly CategoryOperationApiService _categoryService;
    private readonly FournisseurApiService       _fournisseurService;
    private readonly ArticleApiService           _articleService;
    private readonly OperationApiService         _operationService;

    // ─── Local data sources ───────────────────────────────────────────────
    private List<CategoryOperationDto> _categories  = new();
    private List<FournisseurDto>       _fournisseurs = new();
    private List<ArticleDto>           _articles     = new();

    /// <summary>Collection bound to the DetailsStack BindableLayout.</summary>
    public ObservableCollection<DetailLineViewModel> DetailLines { get; } = new();

    // ─── Constructor ──────────────────────────────────────────────────────
    public OperationFormPage(
        CategoryOperationApiService categoryService,
        FournisseurApiService       fournisseurService,
        ArticleApiService           articleService,
        OperationApiService         operationService)
    {
        InitializeComponent();
        BindingContext = this;

        _categoryService    = categoryService;
        _fournisseurService = fournisseurService;
        _articleService     = articleService;
        _operationService   = operationService;
    }

    // ─── Lifecycle ────────────────────────────────────────────────────────
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Generate a unique operation number
        NumeroEntry.Text = $"OP-{DateTime.Now:yyyyMMdd-HHmmss}";

        // Default date = today
        DatePicker.Date = DateTime.Today;

        await LoadPickersAsync();
    }

    // ─── Data loading ─────────────────────────────────────────────────────
    private async Task LoadPickersAsync()
    {
        try
        {
            var categoryTask    = _categoryService.GetCategoriesAsync();
            var fournisseurTask = _fournisseurService.GetFournisseursAsync();
            var articleTask     = _articleService.GetArticlesAsync();

            await Task.WhenAll(categoryTask, fournisseurTask, articleTask);

            _categories  = categoryTask.Result;
            _fournisseurs = fournisseurTask.Result;
            _articles    = articleTask.Result;

            CategoryPicker.ItemsSource    = _categories;
            FournisseurPicker.ItemsSource = _fournisseurs;
            ArticlePicker.ItemsSource     = _articles;
        }
        catch (Exception ex)
        {
            ShowError($"Impossible de charger les données : {ex.Message}");
        }
    }

    // ─── Add detail line ──────────────────────────────────────────────────
    private void OnAddLineClicked(object sender, EventArgs e)
    {
        HideError();

        // — Validate article selection —
        if (ArticlePicker.SelectedIndex < 0)
        {
            ShowError("Veuillez sélectionner un article.");
            return;
        }

        // — Validate quantity —
        if (!decimal.TryParse(QuantiteEntry.Text?.Trim(), out var qty) || qty <= 0)
        {
            ShowError("La quantité doit être un nombre positif.");
            return;
        }

        var article   = _articles[ArticlePicker.SelectedIndex];
        var emplacement = EmplacementEntry.Text?.Trim();
        var remarque    = LineRemarqueEntry.Text?.Trim();

        // Add to observable collection (the BindableLayout will update automatically)
        DetailLines.Add(new DetailLineViewModel
        {
            ArticleId          = article.Id,
            ArticleDesignation = article.Designation,
            Quantite           = qty,
            Emplacement        = string.IsNullOrWhiteSpace(emplacement) ? null : emplacement,
            Remarque           = string.IsNullOrWhiteSpace(remarque) ? null : remarque
        });

        // Reset line-level inputs
        ArticlePicker.SelectedIndex  = -1;
        QuantiteEntry.Text           = string.Empty;
        EmplacementEntry.Text        = string.Empty;
        LineRemarqueEntry.Text       = string.Empty;
    }

    // ─── Remove detail line ───────────────────────────────────────────────
    private void OnRemoveLineClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is DetailLineViewModel line)
        {
            DetailLines.Remove(line);
        }
    }

    // ─── Save operation ───────────────────────────────────────────────────
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        HideError();

        // — Validate category operation —
        if (CategoryPicker.SelectedIndex < 0)
        {
            ShowError("Veuillez sélectionner une catégorie d'opération.");
            return;
        }

        // — Validate that at least one detail line was added —
        if (DetailLines.Count == 0)
        {
            ShowError("Ajoutez au moins un article à l'opération.");
            return;
        }

        // — Build DTO —
        var category = _categories[CategoryPicker.SelectedIndex];
        int? fournisseurId = FournisseurPicker.SelectedIndex >= 0
            ? _fournisseurs[FournisseurPicker.SelectedIndex].Id
            : (int?)null;

        var dto = new OperationCreateDto
        {
            Numero              = NumeroEntry.Text,
            DateOperation       = DatePicker.Date ?? DateTime.Today,
            Observation         = string.IsNullOrWhiteSpace(ObservationEditor.Text) ? null : ObservationEditor.Text.Trim(),
            CategoryOperationId = 0,
            FournisseurId       = fournisseurId,
            Details             = DetailLines.Select(d => new DetailOperationCreateDto
            {
                ArticleId   = d.ArticleId,
                Quantite    = d.Quantite,
                Emplacement = d.Emplacement,
                Remarque    = d.Remarque
            }).ToList()
        };

        // — Show loading state —
        SaveButton.IsEnabled      = false;
        SavingIndicator.IsRunning = true;
        SavingIndicator.IsVisible = true;

        try
        {
            var success = await _operationService.CreateOperationAsync(dto);

            if (success)
            {
                await DisplayAlert("Succès", "L'opération a été enregistrée.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ShowError("L'enregistrement a échoué. Vérifiez les données et réessayez.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Erreur : {ex.Message}");
        }
        finally
        {
            SaveButton.IsEnabled      = true;
            SavingIndicator.IsRunning = false;
            SavingIndicator.IsVisible = false;
        }
    }

    // ─── Helpers ──────────────────────────────────────────────────────────
    private void ShowError(string message)
    {
        ErrorLabel.Text          = message;
        ErrorLabel.IsVisible     = true;
        ErrorContainer.IsVisible = true;
    }

    private void HideError()
    {
        ErrorLabel.IsVisible     = false;
        ErrorContainer.IsVisible = false;
    }
}

/// <summary>
/// View-model for a single operation detail line shown in the list.
/// </summary>
public class DetailLineViewModel
{
    public int     ArticleId          { get; set; }
    public string  ArticleDesignation { get; set; } = string.Empty;
    public decimal Quantite           { get; set; }
    public string? Emplacement        { get; set; }
    public string? Remarque           { get; set; }
}
