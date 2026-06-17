using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class FinanceiroService(HttpClient http)
{
    public async Task<List<LancamentoFinanceiro>> GetLancamentosAsync() =>
        await http.GetFromJsonAsync<List<LancamentoFinanceiro>>("api/financeiro/lancamentos") ?? new();

    public async Task<List<Fornecedor>> GetFornecedoresAsync() =>
        await http.GetFromJsonAsync<List<Fornecedor>>("api/financeiro/fornecedores") ?? new();
}
