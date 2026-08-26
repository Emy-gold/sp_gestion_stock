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
}
