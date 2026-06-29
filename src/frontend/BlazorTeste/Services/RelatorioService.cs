using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class RelatorioService(HttpClient http)
{
    public async Task<List<Relatorio>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Relatorio>>("api/relatorios", ApiJsonOptions.Default) ?? new();
}
