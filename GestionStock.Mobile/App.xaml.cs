using GestionStock.Mobile.Pages;
using GestionStock.Mobile.Services;

namespace GestionStock.Mobile
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var authService = _serviceProvider.GetRequiredService<AuthService>();

            Page initialPage;
            if (authService.IsAuthenticated)
            {
                initialPage = new AppShell(authService);
            }
            else
            {
                initialPage = _serviceProvider.GetRequiredService<LoginPage>();
            }

            return new Window(initialPage);
        }
    }
}