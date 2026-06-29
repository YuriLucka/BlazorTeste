## Task 3: Update WASM frontend services

**Files:**
- Modify: `BlazorTeste/Services/GuiaSindicalService.cs`
- Modify: `BlazorTeste/Services/NegociacaoService.cs`
- Modify: `BlazorTeste/Services/EventoService.cs`
- Modify: `BlazorTeste/Services/RelatorioService.cs`
- Modify: `BlazorTeste/Services/ConfiguracaoService.cs`
- Modify: `BlazorTeste/Services/BaixaCobrancaService.cs`

**Interfaces:**
- Consumes: Tasks 1 + 2 endpoints (listed above)
- `Program.cs` already registers all 6 services as `AddScoped` — no changes needed there

- [ ] **Step 1: Replace GuiaSindicalService**

Replace `BlazorTeste/Services/GuiaSindicalService.cs`:

```csharp
using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class GuiaSindicalService(HttpClient http)
{
    public async Task<List<GuiaSindical>> GetAllAsync(int? entidadeId = null)
    {
        var url = entidadeId.HasValue ? $"api/guiasindical?entidadeId={entidadeId}" : "api/guiasindical";
        return await http.GetFromJsonAsync<List<GuiaSindical>>(url, ApiJsonOptions.Default) ?? new();
    }
}
```

- [ ] **Step 2: Replace NegociacaoService**

Replace `BlazorTeste/Services/NegociacaoService.cs`:

```csharp
using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class NegociacaoService(HttpClient http)
{
    public async Task<List<Negociacao>> GetAllAsync(int? entidadeId = null)
    {
        var url = entidadeId.HasValue ? $"api/negociacoes?entidadeId={entidadeId}" : "api/negociacoes";
        return await http.GetFromJsonAsync<List<Negociacao>>(url, ApiJsonOptions.Default) ?? new();
    }
}
```

- [ ] **Step 3: Replace EventoService**

Replace `BlazorTeste/Services/EventoService.cs`:

```csharp
using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class EventoService(HttpClient http)
{
    public async Task<List<Evento>> GetAllAsync(int? entidadeId = null)
    {
        var url = entidadeId.HasValue ? $"api/eventos?entidadeId={entidadeId}" : "api/eventos";
        return await http.GetFromJsonAsync<List<Evento>>(url, ApiJsonOptions.Default) ?? new();
    }
}
```

- [ ] **Step 4: Replace RelatorioService**

Replace `BlazorTeste/Services/RelatorioService.cs`:

```csharp
using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class RelatorioService(HttpClient http)
{
    public async Task<List<Relatorio>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Relatorio>>("api/relatorios", ApiJsonOptions.Default) ?? new();
}
```

- [ ] **Step 5: Replace ConfiguracaoService**

Replace `BlazorTeste/Services/ConfiguracaoService.cs`:

```csharp
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
```

**Note:** Return types change from non-nullable to nullable (`ConfiguracaoGeral?` etc.) because the API may return 404. If the pages use `_geral is not null` guards (check page code — `Configuracoes.razor` already does `@if (_geral is not null)`), this is safe. If any page assigns to a non-nullable variable, update the page's `@code` field type to nullable.

- [ ] **Step 6: Replace BaixaCobrancaService**

Replace `BlazorTeste/Services/BaixaCobrancaService.cs`:

```csharp
using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class BaixaCobrancaService(HttpClient http)
{
    public async Task<List<RegistroBaixa>> GetHistoricoAsync(int? entidadeId = null)
    {
        var url = entidadeId.HasValue ? $"api/baixa/historico?entidadeId={entidadeId}" : "api/baixa/historico";
        return await http.GetFromJsonAsync<List<RegistroBaixa>>(url, ApiJsonOptions.Default) ?? new();
    }
}
```

- [ ] **Step 7: Check ConfiguracaoService nullable compatibility with Configuracoes.razor**

Read `BlazorTeste/Components/Pages/Configuracoes/Configuracoes.razor` and check `@code` variable declarations for `_geral`, `_cobranca`, `_email`, `_banco`. If any are declared as non-nullable (e.g., `ConfiguracaoGeral _geral = new()`), change them to nullable (e.g., `ConfiguracaoGeral? _geral`). The page already has `@if (_geral is not null)` guards so this is safe.

- [ ] **Step 8: Build to verify 0 errors**

Run from `C:\Users\yrodrigues\source\repos\BlazorTeste\BlazorTeste`:

```
dotnet build
```

Expected: `Build succeeded. 0 Warning(s). 0 Error(s)`

- [ ] **Step 9: Commit**

```
git -C "C:\Users\yrodrigues\source\repos\BlazorTeste\BlazorTeste" add -A
git -C "C:\Users\yrodrigues\source\repos\BlazorTeste\BlazorTeste" commit -m "feat: migrate all static services to API endpoints"
```
