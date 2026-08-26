using GestionStock.Shared.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace GestionStock.Mobile.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private LoginResponseDto? _currentUser;

    public event Action? AuthStateChanged;

    public bool IsAuthenticated => _currentUser?.IsSuccess == true;
    public LoginResponseDto? CurrentUser => _currentUser;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(bool success, string message)> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", new LoginDto
            {
                Email = email.Trim(),
                Password = password
            });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                if (result != null && result.IsSuccess)
                {
                    _currentUser = result;
                    AuthStateChanged?.Invoke();
                    return (true, result.Message ?? "Connexion réussie");
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            try
            {
                var errorObj = JsonSerializer.Deserialize<LoginResponseDto>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (false, errorObj?.Message ?? "Identifiants incorrects.");
            }
            catch
            {
                return (false, "Identifiants incorrects ou serveur indisponible.");
            }
        }
        catch (HttpRequestException ex)
        {
            return (false, $"Impossible de joindre le serveur ({_httpClient.BaseAddress}auth/login) : {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"Erreur de connexion : {ex.Message}");
        }
    }

    public void Logout()
    {
        _currentUser = null;
        AuthStateChanged?.Invoke();
    }
}
