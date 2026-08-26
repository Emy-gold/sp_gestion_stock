using GestionStock.Mobile.Services;

namespace GestionStock.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private void OnTogglePasswordClicked(object? sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
    }

    private void OnFillAdminClicked(object? sender, EventArgs e)
    {
        EmailEntry.Text = "admin@standardprofil.com";
        PasswordEntry.Text = "AdminPassword123!";
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        ErrorBanner.IsVisible = false;

        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Veuillez saisir votre email et mot de passe.");
            return;
        }

        LoginLoadingIndicator.IsVisible = true;
        LoginLoadingIndicator.IsRunning = true;
        LoginButton.IsEnabled = false;

        try
        {
            var (success, message) = await _authService.LoginAsync(email, password);

            if (success)
            {
                // Re-initialize AppShell with authenticated state
                Application.Current!.MainPage = new AppShell(_authService);
            }
            else
            {
                ShowError(message);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Erreur : {ex.Message}");
        }
        finally
        {
            LoginLoadingIndicator.IsVisible = false;
            LoginLoadingIndicator.IsRunning = false;
            LoginButton.IsEnabled = true;
        }
    }

    private void ShowError(string message)
    {
        ErrorMessageLabel.Text = message;
        ErrorBanner.IsVisible = true;
    }
}
