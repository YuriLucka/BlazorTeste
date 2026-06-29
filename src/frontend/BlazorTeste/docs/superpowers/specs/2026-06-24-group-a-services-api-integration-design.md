# Group A Services — Integração com API

**Data:** 2026-06-24  
**Escopo:** Migrar CobrancaService, JuridicoService e FinanceiroService de dados estáticos para chamadas HTTP à API.

## Contexto

Três services ainda usam `List<T>` hardcoded. Os controllers correspondentes já existem no backend e já consultam o banco SQL Server. Padrão idêntico ao `ContribuinteService` já migrado.

## Mudanças

### CobrancaService

**Arquivo:** `BlazorTeste/Services/CobrancaService.cs`

| Método | Endpoint |
|--------|----------|
| `GetAllAsync()` | `GET api/cobrancas` |
| `GetByStatusAsync(StatusCobranca status)` | `GET api/cobrancas?status={status}` |
| `GetByContribuinteAsync(int contribuinteId)` | `GET api/cobrancas?contribuinteId={contribuinteId}` |

### JuridicoService

**Arquivo:** `BlazorTeste/Services/JuridicoService.cs`

| Método | Endpoint |
|--------|----------|
| `GetProcessosAsync()` | `GET api/juridico/processos` |
| `GetAdvogadosAsync()` | `GET api/juridico/advogados` |
| `GetAudienciasAsync()` | `GET api/juridico/audiencias` |

### FinanceiroService

**Arquivo:** `BlazorTeste/Services/FinanceiroService.cs`

| Método | Endpoint |
|--------|----------|
| `GetLancamentosAsync()` | `GET api/financeiro/lancamentos` |
| `GetFornecedoresAsync()` | `GET api/financeiro/fornecedores` |

## Padrão a seguir

Idêntico ao `ContribuinteService` e `MailingService`:
```csharp
public class XService(HttpClient http)
{
    public async Task<List<T>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<T>>("api/x") ?? new();
}
```

## O que NÃO muda

- Nenhuma página `.razor`
- Nenhum controller no backend
- `Program.cs` do frontend (serviços já registrados como scoped com HttpClient)

## Critérios de sucesso

- Três services reescritos sem dados estáticos
- Assinaturas públicas idênticas às anteriores (nenhuma página quebra)
- Build sem erros
- Páginas Cobranças, Jurídico e Financeiro exibem dados vindos do banco
