using GestionStock.Shared.DTOs;
using System.Net.Http.Json;

namespace GestionStock.Mobile.Services;

public class FournisseurApiService
{
    private readonly HttpClient _httpClient;

    public FournisseurApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<FournisseurDto>> GetFournisseursAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<FournisseurDto>>("fournisseurs");
            return result ?? new List<FournisseurDto>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur de chargement des fournisseurs : {ex.Message}", ex);
        }
    }

    public async Task<FournisseurDto?> GetFournisseurAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<FournisseurDto>($"fournisseurs/{id}");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur de chargement du fournisseur : {ex.Message}", ex);
        }
    }

    public async Task<bool> CreateFournisseurAsync(FournisseurCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("fournisseurs", dto);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la création du fournisseur : {ex.Message}", ex);
        }
    }

    public async Task<bool> UpdateFournisseurAsync(int id, FournisseurUpdateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"fournisseurs/{id}", dto);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la mise à jour du fournisseur : {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteFournisseurAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"fournisseurs/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new Exception(string.IsNullOrWhiteSpace(errorMsg) ? "Échec de la suppression." : errorMsg);
            }
            return true;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la suppression du fournisseur : {ex.Message}", ex);
        }
    }
}
