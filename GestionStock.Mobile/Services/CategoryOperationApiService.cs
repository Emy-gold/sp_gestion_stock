using GestionStock.Shared.DTOs;
using System.Net.Http.Json;

namespace GestionStock.Mobile.Services;

public class CategoryOperationApiService
{
    private readonly HttpClient _httpClient;

    public CategoryOperationApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CategoryOperationDto>> GetCategoriesAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<CategoryOperationDto>>("categoryoperations");
            return result ?? new List<CategoryOperationDto>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur de chargement des catégories d'opérations : {ex.Message}", ex);
        }
    }

    public async Task<CategoryOperationDto?> GetCategoryAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CategoryOperationDto>($"categoryoperations/{id}");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la récupération de la catégorie d'opération : {ex.Message}", ex);
        }
    }

    public async Task<bool> CreateCategoryAsync(CategoryOperationCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("categoryoperations", dto);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la création de la catégorie d'opération : {ex.Message}", ex);
        }
    }

    public async Task<bool> UpdateCategoryAsync(int id, CategoryOperationCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"categoryoperations/{id}", dto);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la modification de la catégorie d'opération : {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"categoryoperations/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la suppression de la catégorie d'opération : {ex.Message}", ex);
        }
    }
}
