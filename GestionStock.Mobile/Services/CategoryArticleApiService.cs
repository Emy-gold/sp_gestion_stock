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
        var result = await _httpClient.GetFromJsonAsync<List<CategoryArticleDto>>("categoryarticles");
        return result ?? new List<CategoryArticleDto>();
    }
}