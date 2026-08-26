using GestionStock.Shared.DTOs;
using System.Net.Http.Json;


namespace GestionStock.Mobile.Services;

public class CategoryArticleApiService
{
    private readonly HttpClient _httpClient;

    public CategoryArticleApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CategoryArticleDto>> GetCategoriesAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<CategoryArticleDto>>("categoryarticles");
            return result ?? new List<CategoryArticleDto>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur réseau vers {_httpClient.BaseAddress}categoryarticles : {ex.Message} (StatusCode: {ex.StatusCode})", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur lors du chargement des catégories : {ex.GetType().Name} — {ex.Message}", ex);
        }
    }
}