using GestionStock.Mobile.Pages;
using GestionStock.Mobile.Services;

namespace GestionStock.Mobile;

public partial class AppShell : Shell
{
    private readonly AuthService? _authService;

    public AppShell() : this(null)
    {
    }

    public AppShell(AuthService? authService)
    {
        InitializeComponent();
        _authService = authService;

        Routing.RegisterRoute(nameof(ArticleFormPage), typeof(ArticleFormPage));
        Routing.RegisterRoute(nameof(CategoryFormPage), typeof(CategoryFormPage));

        UpdateFlyoutHeader();
    }

    private void UpdateFlyoutHeader()
    {
        if (_authService?.CurrentUser != null)
        {
            var user = _authService.CurrentUser;
            FlyoutUserName.Text = $"{user.Prenom} {user.Nom}".Trim();
            FlyoutUserEmail.Text = user.Email;
            FlyoutUserRole.Text = $"⭐ {user.Role}";

            var initial = string.Empty;
            if (!string.IsNullOrEmpty(user.Prenom))
                initial += user.Prenom[0];
            if (!string.IsNullOrEmpty(user.Nom))
                initial += user.Nom[0];

            FlyoutAvatarInitial.Text = string.IsNullOrEmpty(initial) ? "SP" : initial.ToUpper();
        }
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Déconnexion", "Voulez-vous vraiment vous déconnecter ?", "Oui", "Non");
        if (confirm)
        {
            _authService?.Logout();
            
            // Navigate to LoginPage
            var loginPage = Handler?.MauiContext?.Services.GetService<LoginPage>() 
                ?? new LoginPage(_authService ?? new AuthService(new HttpClient()));

            Application.Current!.MainPage = loginPage;
        }
    }
}
