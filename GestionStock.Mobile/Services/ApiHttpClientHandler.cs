namespace GestionStock.Mobile.Services;

public static class ApiHttpClientHandler
{
    public static HttpClientHandler GetPlatformHandler()
    {
        var handler = new HttpClientHandler();

#if DEBUG
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif

        return handler;
    }
}