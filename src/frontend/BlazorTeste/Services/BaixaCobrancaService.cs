using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class BaixaCobrancaService(HttpClient http)
{
    public async Task<List<RegistroBaixa>> GetHistoricoAsync() =>
        await http.GetFromJsonAsync<List<RegistroBaixa>>("api/baixa/historico", ApiJsonOptions.Default) ?? new();

    public async Task<List<RegistroBaixa>> GetHistoricoByEntidadeAsync(int entidadeId) =>
        await http.GetFromJsonAsync<List<RegistroBaixa>>($"api/baixa/historico?entidadeId={entidadeId}", ApiJsonOptions.Default) ?? new();

    public async Task<bool> BaixarAsync(int cobrancaId)
    {
        var response = await http.PostAsync($"api/baixa/{cobrancaId}/baixar?tipo=Manual", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<ProcessarArquivoResult?> ProcessarArquivoAsync(int entidadeId = 0) =>
        await http.PostAsJsonAsync($"api/baixa/processar-arquivo?entidadeId={entidadeId}", new { })
            .ContinueWith(async t =>
            {
                var r = await t;
                return r.IsSuccessStatusCode
                    ? await r.Content.ReadFromJsonAsync<ProcessarArquivoResult>(ApiJsonOptions.Default)
                    : null;
            }).Unwrap();
}
