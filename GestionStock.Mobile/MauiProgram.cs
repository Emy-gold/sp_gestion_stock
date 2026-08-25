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

            builder.Services.AddSingleton(sp => new HttpClient(ApiHttpClientHandler.GetPlatformHandler())
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            });

            builder.Services.AddSingleton<ArticleApiService>();

            builder.Services.AddTransient<ArticlesPage>();

            return builder.Build();
        }
    }
}
