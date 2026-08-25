using GestionStock.Shared.DTOs;
using System.Net.Http.Json;

namespace GestionStock.Mobile.Services;

public class ArticleApiService
{
    private readonly HttpClient _httpClient;

    public ArticleApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ArticleDto>> GetArticlesAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<ArticleDto>>("articles");
        return result ?? new List<ArticleDto>();
    }

    public async Task<ArticleDto?> GetArticleAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<ArticleDto>($"articles/{id}");
    }

    public async Task<bool> CreateArticleAsync(ArticleCreateDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("articles", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateArticleAsync(int id, ArticleUpdateDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"articles/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"articles/{id}");
        return response.IsSuccessStatusCode;
    }
}