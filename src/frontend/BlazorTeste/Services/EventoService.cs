using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class EventoService(HttpClient http)
{
    public async Task<List<Evento>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Evento>>("api/eventos", ApiJsonOptions.Default) ?? new();

    public async Task<List<Evento>> GetByEntidadeAsync(int entidadeId) =>
        await http.GetFromJsonAsync<List<Evento>>($"api/eventos?entidadeId={entidadeId}", ApiJsonOptions.Default) ?? new();
}
