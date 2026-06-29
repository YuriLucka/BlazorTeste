using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class CobrancaService(HttpClient http)
{
    public async Task<List<Cobranca>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Cobranca>>("api/cobrancas", ApiJsonOptions.Default) ?? new();

    public async Task<List<Cobranca>> GetByStatusAsync(StatusCobranca status) =>
        await http.GetFromJsonAsync<List<Cobranca>>($"api/cobrancas?status={status}", ApiJsonOptions.Default) ?? new();

    public async Task<List<Cobranca>> GetByContribuinteAsync(int contribuinteId) =>
        await http.GetFromJsonAsync<List<Cobranca>>($"api/cobrancas?contribuinteId={contribuinteId}", ApiJsonOptions.Default) ?? new();
}
