using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class ContribuinteService(HttpClient http)
{
    public async Task<List<Contribuinte>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Contribuinte>>("api/contribuintes") ?? new();

    public async Task<List<Contribuinte>> GetByEntidadeAsync(int entidadeId) =>
        await http.GetFromJsonAsync<List<Contribuinte>>($"api/contribuintes?entidadeId={entidadeId}") ?? new();

    public async Task<Contribuinte?> GetByIdAsync(int id) =>
        await http.GetFromJsonAsync<Contribuinte>($"api/contribuintes/{id}");
}
