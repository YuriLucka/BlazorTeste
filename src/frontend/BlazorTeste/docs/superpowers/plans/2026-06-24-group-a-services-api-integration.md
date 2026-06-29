# Group A Services API Integration — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Substituir dados estáticos de CobrancaService, JuridicoService e FinanceiroService por chamadas HTTP à API.

**Architecture:** Reescrita direta de cada service — trocar static `List<T>` por primary constructor `(HttpClient http)` e `GetFromJsonAsync`. Sem mudanças em UI, backend ou Program.cs. Padrão idêntico ao ContribuinteService já migrado.

**Tech Stack:** Blazor WASM, `System.Net.Http.Json`, ASP.NET Core 10

## Global Constraints

- Namespace: `BlazorTeste.Services`
- Using obrigatório: `using BlazorTeste.Models;` e `using System.Net.Http.Json;`
- Padrão: primary constructor `(HttpClient http)`, `GetFromJsonAsync`, `?? new()` em retornos de lista
- API base URL configurada no HttpClient: `http://localhost:5141` — não hardcodar nos services
- Assinaturas públicas devem ser idênticas às originais — nenhuma página `.razor` pode quebrar
- Não alterar `Program.cs`, nenhuma página `.razor`, nenhum controller

---

### Task 1: Migrar CobrancaService

**Files:**
- Modify: `BlazorTeste/Services/CobrancaService.cs` (reescrita completa)

**Interfaces:**
- Produces:
  - `GetAllAsync() : Task<List<Cobranca>>`
  - `GetByStatusAsync(StatusCobranca status) : Task<List<Cobranca>>`
  - `GetByContribuinteAsync(int contribuinteId) : Task<List<Cobranca>>`

- [ ] **Step 1: Substituir conteúdo de CobrancaService.cs**

Substituir todo o conteúdo do arquivo por:

```csharp
using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class CobrancaService(HttpClient http)
{
    public async Task<List<Cobranca>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Cobranca>>("api/cobrancas") ?? new();

    public async Task<List<Cobranca>> GetByStatusAsync(StatusCobranca status) =>
        await http.GetFromJsonAsync<List<Cobranca>>($"api/cobrancas?status={status}") ?? new();

    public async Task<List<Cobranca>> GetByContribuinteAsync(int contribuinteId) =>
        await http.GetFromJsonAsync<List<Cobranca>>($"api/cobrancas?contribuinteId={contribuinteId}") ?? new();
}
```

- [ ] **Step 2: Verificar build**

```
dotnet build BlazorTeste/BlazorTeste.csproj
```

Saída esperada: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add BlazorTeste/Services/CobrancaService.cs
git commit -m "feat: integrate CobrancaService with REST API"
```

---

### Task 2: Migrar JuridicoService

**Files:**
- Modify: `BlazorTeste/Services/JuridicoService.cs` (reescrita completa)

**Interfaces:**
- Produces:
  - `GetProcessosAsync() : Task<List<Processo>>`
  - `GetAdvogadosAsync() : Task<List<Advogado>>`
  - `GetAudienciasAsync() : Task<List<Audiencia>>`

- [ ] **Step 1: Substituir conteúdo de JuridicoService.cs**

Substituir todo o conteúdo do arquivo por:

```csharp
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
```

- [ ] **Step 2: Verificar build**

```
dotnet build BlazorTeste/BlazorTeste.csproj
```

Saída esperada: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add BlazorTeste/Services/JuridicoService.cs
git commit -m "feat: integrate JuridicoService with REST API"
```

---

### Task 3: Migrar FinanceiroService

**Files:**
- Modify: `BlazorTeste/Services/FinanceiroService.cs` (reescrita completa)

**Interfaces:**
- Produces:
  - `GetLancamentosAsync() : Task<List<LancamentoFinanceiro>>`
  - `GetFornecedoresAsync() : Task<List<Fornecedor>>`

- [ ] **Step 1: Substituir conteúdo de FinanceiroService.cs**

Substituir todo o conteúdo do arquivo por:

```csharp
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
```

- [ ] **Step 2: Verificar build**

```
dotnet build BlazorTeste/BlazorTeste.csproj
```

Saída esperada: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add BlazorTeste/Services/FinanceiroService.cs
git commit -m "feat: integrate FinanceiroService with REST API"
```
