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
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<ArticleDto>>("articles");
            return result ?? new List<ArticleDto>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Impossible de joindre l'API sur {_httpClient.BaseAddress}articles. Vérifiez que l'API GestionStock.Api est bien démarrée. Détails : {ex.Message}", ex);
        }
    }

    public async Task<ArticleDto?> GetArticleAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ArticleDto>($"articles/{id}");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la récupération de l'article {id} ({_httpClient.BaseAddress}articles/{id}) : {ex.Message}", ex);
        }
    }

    public async Task<bool> CreateArticleAsync(ArticleCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("articles", dto);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la création de l'article ({_httpClient.BaseAddress}articles) : {ex.Message}", ex);
        }
    }

    public async Task<bool> UpdateArticleAsync(int id, ArticleUpdateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"articles/{id}", dto);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la modification de l'article {id} ({_httpClient.BaseAddress}articles/{id}) : {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"articles/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la suppression de l'article {id} ({_httpClient.BaseAddress}articles/{id}) : {ex.Message}", ex);
        }
    }
}