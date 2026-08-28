namespace GestionStock.Mobile.Services;

public static class ApiConfig
{
    // ==============================================================
    // CONFIGURATION RÉSEAU
    // - Émulateur Android  : 10.0.2.2 (alias localhost de la machine hôte)
    // - Téléphone physique : IP de la machine hôte sur le réseau Wi-Fi
    //   → Retrouver l'IP avec : ipconfig  (chercher "Adresse IPv4")
    // ==============================================================

    // ⚙️ Mettre ici l'IP de votre PC sur le réseau Wi-Fi (ipconfig)
    private const string HostIp = "10.44.10.50";

#if ANDROID
    // Utiliser ANDROID_EMULATOR dans les propriétés du projet pour l'émulateur
    public static string BaseUrl => $"http://{HostIp}:5026/api/";

#elif IOS
    public static string BaseUrl => $"http://{HostIp}:5026/api/";

#elif WINDOWS
    public static string BaseUrl => "http://localhost:5026/api/";

#else
    public static string BaseUrl => $"http://{HostIp}:5026/api/";

#endif
}