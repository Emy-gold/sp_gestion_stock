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

    public async Task<CategoryArticleDto?> GetCategoryAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CategoryArticleDto>($"categoryarticles/{id}");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur réseau : {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Impossible de charger la catégorie : {ex.Message}", ex);
        }
    }

    public async Task<bool> CreateCategoryAsync(CategoryArticleCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("categoryarticles", dto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"L'API a retourné une erreur : {error}");
            }
            return true;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur réseau : {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Impossible de créer la catégorie : {ex.Message}", ex);
        }
    }

    public async Task<bool> UpdateCategoryAsync(int id, CategoryArticleCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"categoryarticles/{id}", dto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"L'API a retourné une erreur : {error}");
            }
            return true;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur réseau : {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Impossible de modifier la catégorie : {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"categoryarticles/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"L'API a retourné une erreur : {error}");
            }
            return true;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur réseau : {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Impossible de supprimer la catégorie : {ex.Message}", ex);
        }
    }
}