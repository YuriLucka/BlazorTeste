using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class GuiaSindicalService(HttpClient http)
{
    public async Task<List<GuiaSindical>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<GuiaSindical>>("api/guiasindical", ApiJsonOptions.Default) ?? new();

    public async Task<List<GuiaSindical>> GetByEntidadeAsync(int entidadeId) =>
        await http.GetFromJsonAsync<List<GuiaSindical>>($"api/guiasindical?entidadeId={entidadeId}", ApiJsonOptions.Default) ?? new();
}
