# Contribuintes API Integration — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Substituir dados estáticos do frontend por chamadas HTTP reais à API, com dados persistidos no SQL Server local.

**Architecture:** O backend já possui `ContribuintesController` com os endpoints necessários. A migration `InitialCreate` já existe. Basta (1) trocar o provider do EF Core de InMemory para SQL Server, (2) aplicar a migration, e (3) reescrever `ContribuinteService` para usar `HttpClient` — mesmo padrão de `MailingService`.

**Tech Stack:** ASP.NET Core 10, EF Core 10 + SQL Server, Blazor WASM, MudBlazor, `System.Net.Http.Json`

## Global Constraints

- Target framework: `net10.0`
- SQL Server: `(localdb)\MSSQLLocalDB`, database `SindERP`
- API base URL (frontend): `http://localhost:5141`
- EF Core packages já instalados: `Microsoft.EntityFrameworkCore.SqlServer` v10.0.9, `Microsoft.EntityFrameworkCore.Design` v10.0.9
- Não alterar a UI (`Contribuintes.razor`) nem o modelo (`Contribuinte.cs`)
- Seguir padrão de `MailingService`: primary constructor `(HttpClient http)`, `GetFromJsonAsync`

---

### Task 1: Backend — trocar InMemory por SQL Server e aplicar migration

**Files:**
- Modify: `BlazorTeste.Api/Program.cs:9-10`

**Interfaces:**
- Produces: API conectada ao SQL Server, tabelas criadas, seed data com 50 contribuintes populado

- [ ] **Step 1: Substituir UseInMemoryDatabase por UseSqlServer**

Em `BlazorTeste.Api/Program.cs`, trocar as linhas 9-10:

```csharp
// ANTES
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("SindErpDb"));

// DEPOIS
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

O `using Microsoft.EntityFrameworkCore;` já está na linha 5 — não precisa adicionar.

- [ ] **Step 2: Aplicar a migration no banco**

No terminal, dentro de `BlazorTeste.Api`:

```
dotnet ef database update
```

Saída esperada: `Build started... Done. Applying migration '20260617130512_InitialCreate'. Done.`

Se o banco `SindERP` não existir, o EF Core cria automaticamente.

- [ ] **Step 3: Verificar startup da API**

```
dotnet run --project BlazorTeste.Api
```

Saída esperada: `Now listening on: http://localhost:5141` sem erros.  
Abrir `http://localhost:5141/api/contribuintes` no browser — deve retornar JSON com 50 contribuintes.

- [ ] **Step 4: Commit**

```bash
git add BlazorTeste.Api/Program.cs
git commit -m "feat: switch EF Core provider from InMemory to SQL Server"
```

---

### Task 2: Frontend — ContribuinteService via HttpClient

**Files:**
- Modify: `BlazorTeste/Services/ContribuinteService.cs` (reescrita completa)

**Interfaces:**
- Consumes: `GET http://localhost:5141/api/contribuintes` → `List<Contribuinte>`
- Consumes: `GET http://localhost:5141/api/contribuintes?entidadeId={id}` → `List<Contribuinte>`
- Consumes: `GET http://localhost:5141/api/contribuintes/{id}` → `Contribuinte?`
- Produces: mesmas assinaturas públicas de antes — `GetAllAsync()`, `GetByEntidadeAsync(int)`, `GetByIdAsync(int)` — sem quebrar `Contribuintes.razor`

- [ ] **Step 1: Reescrever ContribuinteService**

Substituir todo o conteúdo de `BlazorTeste/Services/ContribuinteService.cs` por:

```csharp
using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class ContribuinteService(HttpClient http)
{
    public async Task<List<Contribuinte>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Contribuinte>>("api/contribuintes") ?? new();

    public async Task<List<Contribuinte>> GetByEntidadeAsync(int entidadeId) =>
        await http.GetFromJsonAsync<List<Contribuinte>>($"api/contribuintes?entidadeId={entidadeId}") ?? new();

    public async Task<Contribuinte?> GetByIdAsync(int id) =>
        await http.GetFromJsonAsync<Contribuinte>($"api/contribuintes/{id}");
}
```

`HttpClient` já está registrado com `BaseAddress = http://localhost:5141` em `Program.cs` (linha 14-17).  
`ContribuinteService` já está registrado como scoped na linha 20 — nenhuma mudança em `Program.cs`.

- [ ] **Step 2: Build para verificar compilação**

```
dotnet build BlazorTeste/BlazorTeste.csproj
```

Saída esperada: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Testar end-to-end**

1. Certificar que a API está rodando em `http://localhost:5141`
2. Rodar o frontend: `dotnet run --project BlazorTeste`
3. Navegar para `/contribuintes`
4. Verificar: grid exibe contribuintes vindos do banco (50 registros do seed)
5. Clicar em uma linha — drawer abre com dados reais (endereços, contatos, etc.)
6. Verificar no DevTools (Network) que a requisição vai para `http://localhost:5141/api/contribuintes`

- [ ] **Step 4: Commit**

```bash
git add BlazorTeste/Services/ContribuinteService.cs
git commit -m "feat: integrate ContribuinteService with REST API"
```
