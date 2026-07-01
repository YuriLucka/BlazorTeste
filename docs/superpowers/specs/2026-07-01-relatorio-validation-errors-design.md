# Exibir erros de validação do backend no drawer "Gerar Relatório" — design

## Contexto

`POST /api/relatorios/{id}/gerar` já é validado no backend pelo `GerarRelatorioRequestValidator` (`FluentValidation`, adicionado em trabalho anterior nesta mesma sessão) — rejeita `Formato` fora de `PDF`/`EXCEL` e `DataFim < DataInicio` com 400 `application/problem+json`.

O frontend chama esse endpoint via `RelatorioService.GerarAsync` (`src/frontend/BlazorTeste/Services/RelatorioService.cs:11-16`), mas descarta qualquer erro:

```csharp
public async Task<GerarRelatorioResult?> GerarAsync(int id, GerarRelatorioRequest request)
{
    var response = await http.PostAsJsonAsync($"api/relatorios/{id}/gerar", request, ApiJsonOptions.Default);
    if (!response.IsSuccessStatusCode) return null;
    return await response.Content.ReadFromJsonAsync<GerarRelatorioResult>(ApiJsonOptions.Default);
}
```

`Relatorios.razor.GerarRelatorio()` (`:173-192`) trata qualquer `null` como erro genérico: fecha o drawer e mostra "Erro ao gerar relatório." — sem distinguir "campo inválido, corrija e tente de novo" de "erro de servidor". Nenhum outro lugar do frontend já faz parsing de `ValidationProblemDetails` (grep confirmou zero ocorrências) — este é o primeiro caso.

## Objetivo

Quando o backend rejeitar a requisição por validação (400), mostrar ao usuário a(s) mensagem(ns) de erro específica(s) do `GerarRelatorioRequestValidator`, e manter o drawer aberto pra ele corrigir e tentar de novo — em vez do genérico atual que fecha o drawer e esconde o motivo.

## Mudanças

### `RelatorioService.cs`

Novo tipo de retorno local (definido no mesmo arquivo, não em `Models/Domain.cs` — é específico dessa chamada):

```csharp
public record RelatorioGeracaoResult(bool Sucesso, string Mensagem, List<string> Erros);
```

`GerarAsync` reescrito:

```csharp
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
```

A chave `errors` (minúscula) é fixa no `ValidationProblemDetails` do ASP.NET Core independente da política de nomenclatura configurada em `Program.cs` — `PropertyNameCaseInsensitive = true` já configurado em `ApiJsonOptions.Default` cobre qualquer variação de case de qualquer forma.

### `Relatorios.razor`

`GerarRelatorio()` (`:173-192`) reescrito:

```csharp
private async Task GerarRelatorio()
{
    if (_selectedRel is null) return;
    _gerando = true;
    var request = new GerarRelatorioRequest
    {
        DataInicio = _dateRange?.Start,
        DataFim = _dateRange?.End,
        Contribuinte = string.IsNullOrEmpty(_filtroContribuinte) ? null : _filtroContribuinte,
        Status = string.IsNullOrEmpty(_filtroStatus) ? null : _filtroStatus,
        Formato = _formato
    };
    var result = await RelatorioService.GerarAsync(_selectedRel.Id, request);
    _gerando = false;

    if (result.Sucesso)
    {
        _drawerOpen = false;
        Snackbar.Add(result.Mensagem, Severity.Success);
    }
    else if (result.Erros.Count > 0)
    {
        foreach (var erro in result.Erros)
            Snackbar.Add(erro, Severity.Warning);
    }
    else
    {
        _drawerOpen = false;
        Snackbar.Add(result.Mensagem, Severity.Error);
    }
}
```

Drawer só fecha em sucesso ou erro não-validação; em erro de validação fica aberto (usuário vê os campos preenchidos, lê os avisos, corrige e clica "Gerar Relatório" de novo).

## Verificação

Sem projeto de teste automatizado pro frontend (mesma limitação já documentada nos trabalhos anteriores desta sessão — sem bUnit). Verificação manual no browser:

1. Gerar relatório com filtros válidos (regressão — formato PDF e Excel, já corrigidos em trabalho anterior, devem continuar funcionando).
2. Tentar forçar `DataFim < DataInicio` via o `MudDateRangePicker`. Se o próprio componente impedir selecionar um intervalo invertido pela interface de calendário (comportamento comum desses componentes), documentar isso como achado à parte — regra de validação do backend hoje inalcançável a partir da UI atual — em vez de forçar um teste artificial.

## Fora de escopo

- Validação client-side prévia (bloquear input inválido antes de enviar) — decisão explícita do usuário: só exibir o que o backend já retorna, sem duplicar regra no frontend.
- Qualquer mudança no `GerarRelatorioRequestValidator` ou no `FluentValidationFilter` (backend) — já implementados e revisados em trabalho anterior desta sessão.
- Outros formulários/endpoints além deste.
