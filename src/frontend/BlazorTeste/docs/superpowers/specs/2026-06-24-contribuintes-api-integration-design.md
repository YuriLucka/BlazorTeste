# Contribuintes — Integração com API e SQL Server

**Data:** 2026-06-24  
**Escopo:** Substituir dados estáticos do frontend por chamadas HTTP à API, conectando ao SQL Server local.

## Contexto

O frontend usa `ContribuinteService` com 30 registros hardcoded. O backend já possui:
- `ContribuintesController` com endpoints `GET /api/contribuintes` e `GET /api/contribuintes/{id}`
- `AppDbContext` com tabela `Contribuintes` + owned entities (Enderecos, Contatos, Socios, HistoricoMensais)
- Migration `InitialCreate` criada e pronta para aplicar
- Connection string em `appsettings.json`: `(localdb)\MSSQLLocalDB`, database `SindERP`
- Seed data com 50 contribuintes gerados automaticamente

## Mudanças

### 1. Backend — trocar InMemory → SQL Server

**Arquivo:** `BlazorTeste.Api/Program.cs`

Substituir:
```csharp
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("SindErpDb"));
```
Por:
```csharp
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

Após a mudança, aplicar a migration:
```
dotnet ef database update --project BlazorTeste.Api
```

O seed data (`SeedData.Initialize`) já popula 50 contribuintes na primeira execução.

### 2. Frontend — ContribuinteService via HttpClient

**Arquivo:** `BlazorTeste/Services/ContribuinteService.cs`

Substituir a lista estática por injeção de `HttpClient` e chamadas HTTP:

| Método | Endpoint |
|--------|----------|
| `GetAllAsync()` | `GET /api/contribuintes` |
| `GetByEntidadeAsync(int entidadeId)` | `GET /api/contribuintes?entidadeId={entidadeId}` |
| `GetByIdAsync(int id)` | `GET /api/contribuintes/{id}` |

Mesmo padrão já adotado em `MailingService` e `UsuarioService`.

### 3. Frontend — registro do serviço com HttpClient

**Arquivo:** `BlazorTeste/Program.cs`

Verificar e garantir que `ContribuinteService` está registrado com `HttpClient` apontando para a URL base da API (mesmo padrão dos outros services integrados).

## O que NÃO muda

- Nenhuma alteração na UI (`Contribuintes.razor`)
- Nenhuma alteração no modelo (`Contribuinte.cs` no Shared)
- Nenhuma alteração nos endpoints do backend

## Critérios de sucesso

- API conecta ao SQL Server local sem erros
- Tabelas criadas pela migration
- Seed data popula 50 contribuintes na primeira execução
- Página `/contribuintes` exibe dados vindos da API
- Filtro por entidade funciona (`GetByEntidadeAsync`)
- Detalhe do contribuinte abre com dados reais
