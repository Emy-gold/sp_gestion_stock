using GestionStock.Mobile.Services;

namespace GestionStock.Mobile.Pages;

public partial class OperationsPage : ContentPage
{
    private readonly OperationApiService _operationApiService;

    public OperationsPage(OperationApiService operationApiService)
    {
        InitializeComponent();
        _operationApiService = operationApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOperationsAsync();
    }

    private async Task LoadOperationsAsync()
    {
        try
        {
            var operations = await _operationApiService.GetOperationsAsync();
            OperationsCollectionView.ItemsSource = operations;
            OperationsCountLabel.Text = $"{operations.Count} opération{(operations.Count > 1 ? "s" : "")}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les opérations : {ex.Message}", "OK");
        }
        finally
        {
            OperationsRefreshView.IsRefreshing = false;
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadOperationsAsync();
    }
}
