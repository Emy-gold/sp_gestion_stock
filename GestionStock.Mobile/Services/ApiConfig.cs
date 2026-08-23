namespace GestionStock.Mobile.Services;

public static class ApiConfig
{
#if ANDROID
    public static string BaseUrl => "http://10.0.2.2:5025/api/";

#elif IOS
    public static string BaseUrl => "http://192.168.8.103:5025/api/";

#elif WINDOWS
    public static string BaseUrl => "http://localhost:5025/api/";

#else
    public static string BaseUrl => "http://localhost:5025/api/";

#endif
}