using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class RelatorioService(HttpClient http)
{
    public async Task<List<Relatorio>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Relatorio>>("api/relatorios", ApiJsonOptions.Default) ?? new();

    public async Task<GerarRelatorioResult?> GerarAsync(int id, GerarRelatorioRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/relatorios/{id}/gerar", request, ApiJsonOptions.Default);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GerarRelatorioResult>(ApiJsonOptions.Default);
    }
}
