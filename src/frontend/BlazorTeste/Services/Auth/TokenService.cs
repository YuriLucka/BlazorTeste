using BlazorTeste.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;

namespace BlazorTeste.Services.Auth;

public class TokenService
{
    private string? _accessToken;

    public string? AccessToken => _accessToken;

    public event Action? OnTokenChanged;

    public void SetToken(string token)
    {
        _accessToken = token;
        OnTokenChanged?.Invoke();
    }

    public void ClearToken()
    {
        _accessToken = null;
        OnTokenChanged?.Invoke();
    }

    public async Task InitializeAsync(HttpClient authClient)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            var response = await authClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result is not null)
                    _accessToken = result.AccessToken;
            }
        }
        catch { /* no session to restore */ }
    }
}
