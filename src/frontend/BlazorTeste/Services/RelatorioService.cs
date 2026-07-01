using BlazorTeste.Models;
using System.Net;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public record RelatorioGeracaoResult(bool Sucesso, string Mensagem, List<string> Erros);

public class RelatorioService(HttpClient http)
{
    public async Task<List<Relatorio>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Relatorio>>("api/relatorios", ApiJsonOptions.Default) ?? new();

    public async Task<RelatorioGeracaoResult> GerarAsync(int id, GerarRelatorioRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/relatorios/{id}/gerar", request, ApiJsonOptions.Default);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var problem = await response.Content.ReadFromJsonAsync<ApiValidationProblem>(ApiJsonOptions.Default);
            var erros = problem?.Errors.SelectMany(e => e.Value).ToList() ?? new List<string>();
            return new RelatorioGeracaoResult(false, "Corrija os campos indicados.", erros);
        }

        if (!response.IsSuccessStatusCode)
            return new RelatorioGeracaoResult(false, "Erro ao gerar relatório.", new List<string>());

        var result = await response.Content.ReadFromJsonAsync<GerarRelatorioResult>(ApiJsonOptions.Default);
        return new RelatorioGeracaoResult(result?.Sucesso ?? false, result?.Mensagem ?? "Erro ao gerar relatório.", new List<string>());
    }

    private class ApiValidationProblem
    {
        public Dictionary<string, List<string>> Errors { get; set; } = new();
    }
}
