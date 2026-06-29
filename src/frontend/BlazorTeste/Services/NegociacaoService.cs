using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class NegociacaoService(HttpClient http)
{
    public async Task<List<Negociacao>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Negociacao>>("api/negociacoes", ApiJsonOptions.Default) ?? new();

    public async Task<List<Negociacao>> GetByEntidadeAsync(int entidadeId) =>
        await http.GetFromJsonAsync<List<Negociacao>>($"api/negociacoes?entidadeId={entidadeId}", ApiJsonOptions.Default) ?? new();
}
