using GestionStock.Mobile.Services;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Pages;

public partial class RoleFormPage : ContentPage
{
    private readonly RoleApiService _roleApiService;
    private int? _roleId;

    public string FormTitle => _roleId.HasValue ? "Modifier le Rôle" : "Nouveau Rôle";

    public RoleFormPage(RoleApiService roleApiService)
    {
        InitializeComponent();
        _roleApiService = roleApiService;
        BindingContext = this;
    }

    public void LoadRole(RoleDto role)
    {
        _roleId = role.Id;
        NomEntry.Text = role.Nom;
        DescriptionEditor.Text = role.Description;
        OnPropertyChanged(nameof(FormTitle));
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        ErrorContainer.IsVisible = false;
        ErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(NomEntry.Text))
        {
            ShowError("Le nom du rôle est obligatoire.");
            return;
        }

        var dto = new RoleCreateDto
        {
            Nom = NomEntry.Text.Trim(),
            Description = DescriptionEditor.Text?.Trim()
        };

        SetLoadingState(true);

        try
        {
            if (_roleId.HasValue)
            {
                await _roleApiService.UpdateRoleAsync(_roleId.Value, dto);
            }
            else
            {
                await _roleApiService.CreateRoleAsync(dto);
            }

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors de l'enregistrement : {ex.Message}");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
        ErrorContainer.IsVisible = true;
    }

    private void SetLoadingState(bool isLoading)
    {
        SaveButton.IsVisible = !isLoading;
        SavingIndicator.IsVisible = isLoading;
        SavingIndicator.IsRunning = isLoading;

        NomEntry.IsEnabled = !isLoading;
        DescriptionEditor.IsEnabled = !isLoading;
    }
}
