using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class JuridicoService(HttpClient http)
{
    public async Task<List<Processo>> GetProcessosAsync() =>
        await http.GetFromJsonAsync<List<Processo>>("api/juridico/processos") ?? new();

    public async Task<List<Advogado>> GetAdvogadosAsync() =>
        await http.GetFromJsonAsync<List<Advogado>>("api/juridico/advogados") ?? new();

    public async Task<List<Audiencia>> GetAudienciasAsync() =>
        await http.GetFromJsonAsync<List<Audiencia>>("api/juridico/audiencias") ?? new();
}
