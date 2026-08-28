using GestionStock.Shared.DTOs;
using System.Net.Http.Json;

namespace GestionStock.Mobile.Services;

public class UserApiService
{
    private readonly HttpClient _httpClient;

    public UserApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<UserDto>>("users");
            return result ?? new List<UserDto>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erreur de chargement des utilisateurs : {ex.Message}", ex);
        }
    }

    public async Task<UserDto?> CreateUserAsync(UserCreateDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("users", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<UserDto>();

        var error = await response.Content.ReadAsStringAsync();
        throw new Exception($"Erreur : {error}");
    }

    public async Task UpdateUserAsync(int id, UserCreateDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"users/{id}", dto);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erreur : {error}");
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"users/{id}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Erreur lors de la suppression.");
    }
}
