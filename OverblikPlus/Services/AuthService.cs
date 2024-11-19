using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using OverblikPlus.Dtos.User;

namespace OverblikPlus.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;
    }

    public async Task<bool> LoginAsync(string Email, string Password)
    {
        var loginDto = new { Email, Password };
        Console.WriteLine("Sending login request...");
    
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginDto);

            Console.WriteLine($"Login response status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null)
                {
                    Console.WriteLine($"Login successful. Token: {result.Token}");
                    await _authStateProvider.SetTokenAsync(result.Token, result.RefreshToken);
                    return true;
                }
                else
                {
                    Console.WriteLine("Login response is null.");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login failed. Status: {response.StatusCode}, Error: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during login: {ex.Message}");
        }

        return false;
    }



    public async Task LogoutAsync()
    {
        var response = await _httpClient.PostAsync("api/Auth/logout", null);

        if (response.IsSuccessStatusCode)
        {
            await _authStateProvider.RemoveTokenAsync();
            Console.WriteLine("User logged out successfully.");
        }
        else
        {
            Console.WriteLine($"Logout failed: {response.StatusCode}");
        }
    }

    
    public async Task<bool> RegisterAsync(CreateUserDto createUserDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", createUserDto);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Registration failed: {errorContent}");
        return false;
    }



    public async Task<bool> RefreshTokenAsync()
    {
        var refreshToken = await _authStateProvider.GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken))
        {
            Console.WriteLine("No refresh token available.");
            return false;
        }

        var response = await _httpClient.PostAsJsonAsync("api/Auth/refresh", new { refreshToken });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result != null)
            {
                await _authStateProvider.SetTokenAsync(result.Token, result.RefreshToken);
                return true;
            }
        }

        Console.WriteLine($"Failed to refresh token. Status code: {response.StatusCode}");
        return false;
    }


}

public class LoginResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}