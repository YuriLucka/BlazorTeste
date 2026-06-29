using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class EntidadeService(HttpClient http)
{
    public async Task<List<Entidade>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Entidade>>("api/entidades") ?? new();

    public async Task<Entidade?> GetByIdAsync(int id) =>
        await http.GetFromJsonAsync<Entidade>($"api/entidades/{id}");
}
