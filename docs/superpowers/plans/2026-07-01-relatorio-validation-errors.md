# Relatorio Validation Errors Display Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** When `POST /api/relatorios/{id}/gerar` rejects a request with a 400 validation error, show the user the specific field-level messages instead of a generic "Erro ao gerar relatório." and keep the drawer open so they can fix and retry.

**Architecture:** `RelatorioService.GerarAsync` changes return type from `GerarRelatorioResult?` to a new `RelatorioGeracaoResult` record carrying `Sucesso`, `Mensagem`, and a flattened `Erros` list; on HTTP 400 it parses the `ValidationProblemDetails`-shaped body (`{ errors: { field: [messages] } }`) that `FluentValidationFilter` already produces. `Relatorios.razor`'s `GerarRelatorio()` consumes the new shape: closes the drawer + green snackbar on success, keeps the drawer open + one warning snackbar per message on validation failure, closes + red snackbar on any other failure.

**Tech Stack:** Blazor WebAssembly, `System.Net.Http.Json`, MudBlazor `ISnackbar`.

## Global Constraints

- No client-side pre-validation is added — only display of what the backend's `GerarRelatorioRequestValidator` already returns (explicit scope decision).
- Do not modify `GerarRelatorioRequestValidator` or `FluentValidationFilter` (backend) — already implemented and reviewed in prior work this session.
- No automated frontend test project exists (no bUnit) — verification is manual via browser, not a unit test.
- The `errors` key in the response body is lowercase and fixed by ASP.NET Core's `ValidationProblemDetails` regardless of the API's configured JSON naming policy — `ApiJsonOptions.Default` already sets `PropertyNameCaseInsensitive = true`, which covers any case variation.

---

### Task 1: RelatorioService returns structured validation errors, Relatorios.razor displays them

**Files:**
- Modify: `src/frontend/BlazorTeste/Services/RelatorioService.cs`
- Modify: `src/frontend/BlazorTeste/Components/Pages/Relatorios/Relatorios.razor`

**Interfaces:**
- Produces: `public record RelatorioGeracaoResult(bool Sucesso, string Mensagem, List<string> Erros);` (public, in `RelatorioService.cs`) — consumed by `Relatorios.razor`. `public async Task<RelatorioGeracaoResult> GerarAsync(int id, GerarRelatorioRequest request)` — same method name as before, changed return type (was `Task<GerarRelatorioResult?>`).

- [ ] **Step 1: Rewrite `RelatorioService.cs`**

Read the current file first: `src/frontend/BlazorTeste/Services/RelatorioService.cs`. Its current full content is:

```csharp
using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class RelatorioService(HttpClient http)
{
    public async Task<List<Relatorio>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Relatorio>>("api/relatorios", ApiJsonOptions.Default) ?? new();

    public async Task<GerarRelatorioResult?> GerarAsync(int id, GerarRelatorioRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/relatorios/{id}/gerar", request, ApiJsonOptions.Default);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GerarRelatorioResult>(ApiJsonOptions.Default);
    }
}
```

Replace the entire file content with:

```csharp
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
```

- [ ] **Step 2: Update `Relatorios.razor`'s `GerarRelatorio()` method**

Find the current method (currently lines 173-192 of `src/frontend/BlazorTeste/Components/Pages/Relatorios/Relatorios.razor`):

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
        _drawerOpen = false;
        if (result?.Sucesso == true)
            Snackbar.Add(result.Mensagem, Severity.Success);
        else
            Snackbar.Add("Erro ao gerar relatório.", Severity.Error);
    }
```

Replace it with:

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

- [ ] **Step 3: Build to verify it compiles**

Run: `dotnet build src/frontend/BlazorTeste/BlazorTeste.csproj`
Expected: `Build succeeded`, 0 errors. (If Visual Studio has this project open and running, a file-lock build failure is an environment issue, not a code defect — report it as such.)

- [ ] **Step 4: Commit**

```bash
git add src/frontend/BlazorTeste/Services/RelatorioService.cs src/frontend/BlazorTeste/Components/Pages/Relatorios/Relatorios.razor
git commit -m "feat(frontend): display backend validation errors when generating a relatorio"
```

---

### Task 2: Manual browser verification

**Files:** none (verification only, no code changes)

- [ ] **Step 1: Confirm the app is running**

The frontend needs to be running at `https://localhost:7130` (and the backend API at `https://localhost:7247`) with the new code compiled in. If either isn't running, start them:

```bash
dotnet run --project src/backend/BlazorTeste.Api/BlazorTeste.Api.csproj --launch-profile https
dotnet run --project src/frontend/BlazorTeste/BlazorTeste.csproj --launch-profile https
```

(Run each in the background; wait for "Now listening on: https://localhost:7247" / the frontend's "App url: https://0.0.0.0:7130/" before proceeding.)

- [ ] **Step 2: Regression check — generate a valid PDF report**

Log in, navigate to `https://localhost:7130/relatorios`, click any report card's "PDF" button (or open its config drawer and pick PDF), click "Gerar Relatório".

Expected: drawer closes, a green success snackbar appears (e.g. "Relatório '...' gerado com sucesso no formato PDF.").

- [ ] **Step 3: Regression check — generate a valid Excel report**

Open a report's config drawer, select "Excel" format, click "Gerar Relatório".

Expected: drawer closes, green success snackbar. (This confirms the earlier "Excel"/"XLSX" mismatch fix is still working through this new code path.)

- [ ] **Step 4: Attempt to trigger the date-order validation error**

Open a report's config drawer, try to use the "Período" `MudDateRangePicker` to select an end date before a start date (e.g. click a later date first, then an earlier one, or any interaction that could produce an inverted range).

Expected outcome A (validator reachable): if the picker allows an inverted range, click "Gerar Relatório" — the drawer should stay open, and a warning snackbar reading "Data fim não pode ser anterior à data início." should appear.

Expected outcome B (validator unreachable from UI): if the date range picker's own calendar interaction prevents selecting an inverted range at all (common behavior for this MudBlazor component), this step cannot produce a 400 through the UI. In that case, do not force an artificial test — note this as a finding: the `GerarRelatorioRequestValidator`'s date-order rule is currently unreachable from the existing UI, so this specific display path (warning snackbar wording) is implemented but not exercisable through normal user interaction today. Report which outcome (A or B) actually occurred.

No commit for this task — it's verification only.
