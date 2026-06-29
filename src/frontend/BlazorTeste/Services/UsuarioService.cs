using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class UsuarioService(HttpClient http)
{
    public async Task<List<Usuario>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Usuario>>("api/usuarios") ?? new();

    public async Task<Usuario?> GetByEmailAsync(string email) =>
        await http.GetFromJsonAsync<Usuario>($"api/usuarios/by-email?email={Uri.EscapeDataString(email)}");
}
