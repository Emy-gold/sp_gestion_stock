namespace GestionStock.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Pages.ArticleFormPage), typeof(Pages.ArticleFormPage));
        }
    }
}
