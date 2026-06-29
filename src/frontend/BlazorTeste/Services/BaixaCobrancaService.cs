using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class BaixaCobrancaService(HttpClient http)
{
    public async Task<List<RegistroBaixa>> GetHistoricoAsync() =>
        await http.GetFromJsonAsync<List<RegistroBaixa>>("api/baixa/historico", ApiJsonOptions.Default) ?? new();

    public async Task<List<RegistroBaixa>> GetHistoricoByEntidadeAsync(int entidadeId) =>
        await http.GetFromJsonAsync<List<RegistroBaixa>>($"api/baixa/historico?entidadeId={entidadeId}", ApiJsonOptions.Default) ?? new();
}
