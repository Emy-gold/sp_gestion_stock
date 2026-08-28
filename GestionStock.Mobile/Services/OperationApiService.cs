using GestionStock.Shared.DTOs;
using System.Net.Http.Json;

namespace GestionStock.Mobile.Services;

public class OperationApiService
{
    private readonly HttpClient _httpClient;

    public OperationApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<OperationDto>> GetOperationsAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<OperationDto>>("operations");
            return result ?? new List<OperationDto>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur de chargement des opérations : {ex.Message}", ex);
        }
    }

    public async Task<bool> CreateOperationAsync(OperationCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("operations", dto);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la création de l'opération : {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteOperationAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"operations/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur lors de la suppression de l'opération : {ex.Message}", ex);
        }
    }
}
