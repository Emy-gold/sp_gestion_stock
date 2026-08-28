using System.Net.Http.Json;
using GestionStock.Shared.DTOs;

namespace GestionStock.Mobile.Services;

public class RoleApiService
{
    private readonly HttpClient _httpClient;

    public RoleApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<RoleDto>> GetRolesAsync()
    {
        var response = await _httpClient.GetAsync("roles");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<RoleDto>>() ?? new List<RoleDto>();
    }

    public async Task<RoleDto?> GetRoleAsync(int id)
    {
        var response = await _httpClient.GetAsync($"roles/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<RoleDto>();
        }
        return null;
    }

    public async Task<RoleDto?> CreateRoleAsync(RoleCreateDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("roles", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RoleDto>();
    }

    public async Task UpdateRoleAsync(int id, RoleCreateDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"roles/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteRoleAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"roles/{id}");
        response.EnsureSuccessStatusCode();
    }
}
