namespace GestionStock.Mobile.Services;

public static class ApiConfig
{
    // Note: Port 7117 = HTTPS only, Port 5026 = HTTP
    // L'émulateur Android utilise 10.0.2.2 pour accéder au localhost de la machine hôte

#if ANDROID
    public static string BaseUrl => "http://10.0.2.2:5026/api/";

#elif IOS
    public static string BaseUrl => "http://192.168.8.103:5026/api/";

#elif WINDOWS
    public static string BaseUrl => "http://localhost:5026/api/";

#else
    public static string BaseUrl => "http://localhost:5026/api/";

#endif
}