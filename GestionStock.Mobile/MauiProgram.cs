using Microsoft.Extensions.Logging;
using GestionStock.Mobile.Services;
using GestionStock.Mobile.Pages;

namespace GestionStock.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // HTTP Client
            builder.Services.AddSingleton(sp => new HttpClient(ApiHttpClientHandler.GetPlatformHandler())
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            });

            // Services
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<CategoryArticleApiService>();
            builder.Services.AddSingleton<ArticleApiService>();
            builder.Services.AddSingleton<FournisseurApiService>();
            builder.Services.AddSingleton<OperationApiService>();
            builder.Services.AddSingleton<CategoryOperationApiService>();
            builder.Services.AddSingleton<UserApiService>();
            builder.Services.AddSingleton<RoleApiService>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ArticlesPage>();
            builder.Services.AddTransient<ArticleFormPage>();
            builder.Services.AddTransient<CategoriesPage>();
            builder.Services.AddTransient<CategoryFormPage>();
            builder.Services.AddTransient<OperationsPage>();
            builder.Services.AddTransient<OperationFormPage>();
            builder.Services.AddTransient<CategoryOperationsPage>();
            builder.Services.AddTransient<CategoryOperationFormPage>();
            builder.Services.AddTransient<FournisseursPage>();
            builder.Services.AddTransient<UsersPage>();
            builder.Services.AddTransient<UserFormPage>();
            builder.Services.AddTransient<RolesPage>();
            builder.Services.AddTransient<RoleFormPage>();

            return builder.Build();
        }
    }
}
