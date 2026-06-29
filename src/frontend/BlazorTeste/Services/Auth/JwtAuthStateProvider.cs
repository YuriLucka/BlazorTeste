using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorTeste.Services.Auth;

public class JwtAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly TokenService _tokenService;

    public JwtAuthStateProvider(TokenService tokenService)
    {
        _tokenService = tokenService;
        _tokenService.OnTokenChanged += OnTokenChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrEmpty(_tokenService.AccessToken))
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));

        var claims = ParseClaimsFromJwt(_tokenService.AccessToken);
        var identity = new ClaimsIdentity(claims, "jwt");
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    private void OnTokenChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var padded = payload.Length % 4 == 0
            ? payload
            : payload + new string('=', 4 - payload.Length % 4);
        var bytes = Convert.FromBase64String(
            padded.Replace('-', '+').Replace('_', '/'));
        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(bytes)!;
        return dict.Select(kv => new Claim(kv.Key, kv.Value.ToString()));
    }

    public void Dispose() => _tokenService.OnTokenChanged -= OnTokenChanged;
}
