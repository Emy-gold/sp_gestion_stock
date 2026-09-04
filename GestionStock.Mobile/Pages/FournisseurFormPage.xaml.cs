using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

[QueryProperty(nameof(FournisseurId), "fournisseurId")]
public partial class FournisseurFormPage : ContentPage
{
    private readonly FournisseurApiService _fournisseurApiService;
    private FournisseurDto? _existingFournisseur;

    public string? FournisseurId { get; set; }

    private string _formTitle = "Nouveau fournisseur";
    public string FormTitle
    {
        get => _formTitle;
        set
        {
            _formTitle = value;
            OnPropertyChanged();
        }
    }

    public FournisseurFormPage(FournisseurApiService fournisseurApiService)
    {
        InitializeComponent();
        _fournisseurApiService = fournisseurApiService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrEmpty(FournisseurId) && int.TryParse(FournisseurId, out var id))
        {
            FormTitle = "Modifier le fournisseur";
            DeleteButton.IsVisible = true;
            ActifContainer.IsVisible = true;
            await LoadExistingFournisseurAsync(id);
        }
        else
        {
            FormTitle = "Nouveau fournisseur";
            DeleteButton.IsVisible = false;
            ActifContainer.IsVisible = false;
        }
    }

    private async Task LoadExistingFournisseurAsync(int id)
    {
        try
        {
            _existingFournisseur = await _fournisseurApiService.GetFournisseurAsync(id);
            if (_existingFournisseur is null)
                return;

            NomEntry.Text = _existingFournisseur.Nom;
            TelephoneEntry.Text = _existingFournisseur.Telephone;
            EmailEntry.Text = _existingFournisseur.Email;
            AdresseEditor.Text = _existingFournisseur.Adresse;
            ActifCheckBox.IsChecked = _existingFournisseur.Actif;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger le fournisseur : {ex.Message}", "OK");
        }
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_existingFournisseur == null)
            return;

        var confirm = await DisplayAlert("Confirmation", $"Voulez-vous vraiment supprimer le fournisseur '{_existingFournisseur.Nom}' ?", "Supprimer", "Annuler");
        if (!confirm)
            return;

        try
        {
            var success = await _fournisseurApiService.DeleteFournisseurAsync(_existingFournisseur.Id);
            if (success)
            {
                await DisplayAlert("Succès", "Fournisseur supprimé avec succès.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Erreur", "Impossible de supprimer le fournisseur.", "OK");
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
            ErrorLabel.Text = "Le nom du fournisseur est obligatoire.";
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

            if (_existingFournisseur is null)
            {
                var dto = new FournisseurCreateDto
                {
                    Nom = nom,
                    Telephone = TelephoneEntry.Text?.Trim(),
                    Email = EmailEntry.Text?.Trim(),
                    Adresse = AdresseEditor.Text?.Trim()
                };
                success = await _fournisseurApiService.CreateFournisseurAsync(dto);
            }
            else
            {
                var dto = new FournisseurUpdateDto
                {
                    Nom = nom,
                    Telephone = TelephoneEntry.Text?.Trim(),
                    Email = EmailEntry.Text?.Trim(),
                    Adresse = AdresseEditor.Text?.Trim(),
                    Actif = ActifCheckBox.IsChecked
                };
                success = await _fournisseurApiService.UpdateFournisseurAsync(_existingFournisseur.Id, dto);
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
