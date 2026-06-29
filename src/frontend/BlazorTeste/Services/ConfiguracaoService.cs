using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class ConfiguracaoService(HttpClient http)
{
    public async Task<ConfiguracaoGeral?> GetGeralAsync(int entidadeId) =>
        await http.GetFromJsonAsync<ConfiguracaoGeral>($"api/configuracoes/geral?entidadeId={entidadeId}", ApiJsonOptions.Default);

    public async Task<ConfiguracaoCobranca?> GetCobrancaAsync(int entidadeId) =>
        await http.GetFromJsonAsync<ConfiguracaoCobranca>($"api/configuracoes/cobranca?entidadeId={entidadeId}", ApiJsonOptions.Default);

    public async Task<ConfiguracaoEmail?> GetEmailAsync(int entidadeId) =>
        await http.GetFromJsonAsync<ConfiguracaoEmail>($"api/configuracoes/email?entidadeId={entidadeId}", ApiJsonOptions.Default);

    public async Task<ConfiguracaoBanco?> GetBancoAsync(int entidadeId) =>
        await http.GetFromJsonAsync<ConfiguracaoBanco>($"api/configuracoes/banco?entidadeId={entidadeId}", ApiJsonOptions.Default);
}
