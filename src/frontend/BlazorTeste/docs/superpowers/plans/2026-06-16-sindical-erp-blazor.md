# SindERP — Blazor WASM MVP Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a navigable union management ERP prototype in Blazor WebAssembly with MudBlazor, containing 8 modules with realistic mock data and an elegant Material Design UI.

**Architecture:** Standalone Blazor WebAssembly app (SDK `Microsoft.NET.Sdk.BlazorWebAssembly`) converted from the existing Blazor Web App. All data lives in Singleton services. `AppStateService` tracks the active entity (multi-tenant simulation) and dark mode. No backend, no auth.

**Tech Stack:** .NET 10, Blazor WebAssembly, MudBlazor 6.x, C#

---

## File Map

| File | Action | Purpose |
|------|--------|---------|
| `BlazorTeste.csproj` | Modify | Change SDK + add MudBlazor |
| `Program.cs` | Modify | WebAssemblyHostBuilder + DI |
| `wwwroot/index.html` | Create | WASM entry point |
| `Components/App.razor` | Modify | Router only (no HTML shell) |
| `Components/Routes.razor` | Delete | Merged into App.razor |
| `Components/_Imports.razor` | Modify | Add MudBlazor + Models + Services usings |
| `Components/Layout/MainLayout.razor` | Modify | MudLayout shell + theme |
| `Components/Layout/NavMenu.razor` | Modify | MudNavMenu |
| `Components/Layout/MainLayout.razor.css` | Clear | Not needed |
| `Components/Layout/NavMenu.razor.css` | Clear | Not needed |
| `Components/Layout/ReconnectModal.*` | Delete | Server-only |
| `Components/Pages/Home.razor` | Delete | Replaced by Dashboard |
| `Components/Pages/Counter.razor` | Delete | Not needed |
| `Components/Pages/Weather.razor` | Delete | Not needed |
| `Components/Pages/Error.razor` | Delete | Not needed |
| `Components/Pages/NotFound.razor` | Delete | Handled in App.razor |
| `Services/AppStateService.cs` | Create | Active entity + dark mode |
| `Services/EntidadeService.cs` | Create | Mock entidades |
| `Services/ContribuinteService.cs` | Create | Mock contribuintes |
| `Services/CobrancaService.cs` | Create | Mock cobranças |
| `Services/JuridicoService.cs` | Create | Mock processos/advogados/audiências |
| `Services/FinanceiroService.cs` | Create | Mock lançamentos/fornecedores |
| `Services/MailingService.cs` | Create | Mock campanhas |
| `Services/UsuarioService.cs` | Create | Mock usuários |
| `Models/Entidade.cs` | Create | Entity model |
| `Models/Contribuinte.cs` | Create | Contributor + related models |
| `Models/Cobranca.cs` | Create | Billing models + enums |
| `Models/Juridico.cs` | Create | Process/lawyer/hearing models |
| `Models/Financeiro.cs` | Create | Financial models |
| `Models/Mailing.cs` | Create | Campaign model |
| `Models/Usuario.cs` | Create | User + permission models |
| `Components/Pages/Dashboard/Dashboard.razor` | Create | KPIs + chart + tables |
| `Components/Pages/Contribuintes/Contribuintes.razor` | Create | DataGrid + detail drawer |
| `Components/Pages/Cobranca/Cobrancas.razor` | Create | DataGrid + status chips |
| `Components/Pages/Juridico/Juridico.razor` | Create | Tabs: processos/advogados/audiências |
| `Components/Pages/Financeiro/Financeiro.razor` | Create | Tabs: lançamentos/fluxo/fornecedores |
| `Components/Pages/Mailing/Mailing.razor` | Create | Card grid of campaigns |
| `Components/Pages/Entidades/Entidades.razor` | Create | Card grid of entities |
| `Components/Pages/Usuarios/Usuarios.razor` | Create | DataGrid with chips |

---

### Task 1: Convert to Blazor WASM Standalone + Add MudBlazor

**Files:**
- Modify: `BlazorTeste.csproj`
- Modify: `Program.cs`
- Create: `wwwroot/index.html`
- Modify: `Components/App.razor`
- Delete: `Components/Routes.razor`
- Modify: `Components/_Imports.razor`
- Delete: `Components/Layout/ReconnectModal.razor`, `Components/Layout/ReconnectModal.razor.css`, `Components/Layout/ReconnectModal.razor.js`
- Delete: `Components/Pages/Home.razor`, `Components/Pages/Counter.razor`, `Components/Pages/Weather.razor`, `Components/Pages/Error.razor`, `Components/Pages/NotFound.razor`

- [ ] **Step 1: Replace BlazorTeste.csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>BlazorTeste</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="MudBlazor" Version="6.*" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: Replace Program.cs**

```csharp
using BlazorTeste.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

await builder.Build().RunAsync();
```

- [ ] **Step 3: Create wwwroot/index.html**

```html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>SindERP</title>
    <base href="/" />
    <link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
    <link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
    <link href="app.css" rel="stylesheet" />
</head>
<body>
    <div id="app">
        <div style="display:flex;justify-content:center;align-items:center;height:100vh;font-family:Roboto,sans-serif;color:#888;">
            Carregando SindERP...
        </div>
    </div>
    <script src="_framework/blazor.webassembly.js"></script>
    <script src="_content/MudBlazor/MudBlazor.min.js"></script>
</body>
</html>
```

- [ ] **Step 4: Replace Components/App.razor**

```razor
<Router AppAssembly="typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
    <NotFound>
        <LayoutView Layout="typeof(Layout.MainLayout)">
            <MudText Typo="Typo.h5" Class="pa-8">Página não encontrada.</MudText>
        </LayoutView>
    </NotFound>
</Router>
```

- [ ] **Step 5: Replace Components/_Imports.razor**

```razor
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using BlazorTeste
@using BlazorTeste.Components
@using BlazorTeste.Models
@using BlazorTeste.Services
@using MudBlazor
```

- [ ] **Step 6: Delete Routes.razor and old pages**

Delete these files:
- `Components/Routes.razor`
- `Components/Layout/ReconnectModal.razor`
- `Components/Layout/ReconnectModal.razor.css`
- `Components/Layout/ReconnectModal.razor.js`
- `Components/Pages/Home.razor`
- `Components/Pages/Counter.razor`
- `Components/Pages/Weather.razor`
- `Components/Pages/Error.razor`
- `Components/Pages/NotFound.razor`

- [ ] **Step 7: Run and verify project compiles**

```bash
dotnet build
```

Expected: Build succeeded, 0 errors. Fix any namespace/reference errors before continuing.

- [ ] **Step 8: Commit**

```bash
git add -A
git commit -m "chore: convert to Blazor WASM standalone, add MudBlazor"
```

---

### Task 2: Layout Shell + App State

**Files:**
- Create: `Services/AppStateService.cs`
- Modify: `Program.cs`
- Modify: `Components/Layout/MainLayout.razor`
- Modify: `Components/Layout/NavMenu.razor`
- Clear: `Components/Layout/MainLayout.razor.css`
- Clear: `Components/Layout/NavMenu.razor.css`

- [ ] **Step 1: Create Services/AppStateService.cs**

```csharp
using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class AppStateService
{
    public Entidade? EntidadeAtiva { get; private set; }
    public bool DarkMode { get; private set; }

    public event Action? OnChange;

    public void SetEntidade(Entidade entidade)
    {
        EntidadeAtiva = entidade;
        OnChange?.Invoke();
    }

    public void ToggleDarkMode()
    {
        DarkMode = !DarkMode;
        OnChange?.Invoke();
    }
}
```

- [ ] **Step 2: Register AppStateService in Program.cs**

```csharp
using BlazorTeste.Components;
using BlazorTeste.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddSingleton<AppStateService>();

await builder.Build().RunAsync();
```

- [ ] **Step 3: Replace Components/Layout/MainLayout.razor**

```razor
@inherits LayoutComponentBase
@inject AppStateService AppState
@inject IDialogService DialogService
@implements IDisposable

<MudThemeProvider Theme="_theme" IsDarkMode="AppState.DarkMode" />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <MudAppBar Elevation="1" Dense="false" Color="Color.Primary">
        <MudIconButton Icon="@Icons.Material.Filled.Menu"
                       Color="Color.Inherit"
                       Edge="Edge.Start"
                       OnClick="@(() => _drawerOpen = !_drawerOpen)" />
        <MudText Typo="Typo.h6" Class="ml-2 d-none d-sm-block" Style="letter-spacing:.5px;">
            SindERP
        </MudText>
        <MudSpacer />
        <MudTooltip Text="Trocar entidade">
            <MudChip T="string"
                     Icon="@Icons.Material.Filled.AccountTree"
                     Color="Color.Surface"
                     Size="Size.Small"
                     Class="mr-1"
                     OnClick="OpenEntityDialog">
                @(AppState.EntidadeAtiva?.Sigla ?? "Selecionar entidade")
            </MudChip>
        </MudTooltip>
        <MudTooltip Text="@(AppState.DarkMode ? "Modo claro" : "Modo escuro")">
            <MudIconButton Icon="@(AppState.DarkMode ? Icons.Material.Filled.LightMode : Icons.Material.Filled.DarkMode)"
                           Color="Color.Inherit"
                           OnClick="@(() => AppState.ToggleDarkMode())" />
        </MudTooltip>
        <MudAvatar Color="Color.Secondary" Size="Size.Small" Class="ml-1">AD</MudAvatar>
    </MudAppBar>

    <MudDrawer @bind-Open="_drawerOpen"
               Elevation="2"
               Variant="@DrawerVariant.Responsive"
               ClipMode="DrawerClipMode.Always">
        <MudDrawerHeader Class="pa-4 pb-2">
            <div>
                <MudText Typo="Typo.subtitle1" Color="Color.Primary" Style="font-weight:700;">SindERP</MudText>
                <MudText Typo="Typo.caption" Color="Color.Secondary">Sistema Sindical</MudText>
            </div>
        </MudDrawerHeader>
        <MudDivider />
        <NavMenu />
    </MudDrawer>

    <MudMainContent>
        <MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="pa-4 mt-2">
            @Body
        </MudContainer>
    </MudMainContent>
</MudLayout>

@code {
    private bool _drawerOpen = true;

    private readonly MudTheme _theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1565C0",
            Secondary = "#00897B",
            Background = "#F5F7FA",
            AppbarBackground = "#1565C0",
            DrawerBackground = "#FFFFFF"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#5C9CE6",
            Secondary = "#4DB6A6",
            Background = "#1A1A2E",
            Surface = "#16213E",
            AppbarBackground = "#0D1B2A",
            DrawerBackground = "#16213E"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "240px"
        }
    };

    protected override void OnInitialized()
    {
        AppState.OnChange += StateHasChanged;
    }

    private void OpenEntityDialog()
    {
        // Implemented in Task 11 (Entidades) — placeholder so UI compiles
    }

    public void Dispose()
    {
        AppState.OnChange -= StateHasChanged;
    }
}
```

- [ ] **Step 4: Replace Components/Layout/NavMenu.razor**

```razor
<MudNavMenu Class="py-2">
    <MudNavLink Href="/"
                Match="NavLinkMatch.All"
                Icon="@Icons.Material.Filled.Dashboard">
        Dashboard
    </MudNavLink>

    <MudDivider Class="my-2" />
    <MudText Typo="Typo.overline" Class="px-4 pb-1" Color="Color.Secondary">Operacional</MudText>

    <MudNavLink Href="/contribuintes" Icon="@Icons.Material.Filled.Business">
        Contribuintes
    </MudNavLink>
    <MudNavLink Href="/cobrancas" Icon="@Icons.Material.Filled.RequestQuote">
        Cobranças
    </MudNavLink>

    <MudDivider Class="my-2" />
    <MudText Typo="Typo.overline" Class="px-4 pb-1" Color="Color.Secondary">Módulos</MudText>

    <MudNavLink Href="/juridico" Icon="@Icons.Material.Filled.Gavel">
        Jurídico
    </MudNavLink>
    <MudNavLink Href="/financeiro" Icon="@Icons.Material.Filled.AccountBalance">
        Financeiro
    </MudNavLink>
    <MudNavLink Href="/mailing" Icon="@Icons.Material.Filled.Email">
        Mailing
    </MudNavLink>

    <MudDivider Class="my-2" />
    <MudText Typo="Typo.overline" Class="px-4 pb-1" Color="Color.Secondary">Administração</MudText>

    <MudNavLink Href="/entidades" Icon="@Icons.Material.Filled.AccountTree">
        Entidades
    </MudNavLink>
    <MudNavLink Href="/usuarios" Icon="@Icons.Material.Filled.People">
        Usuários
    </MudNavLink>
</MudNavMenu>
```

- [ ] **Step 5: Clear CSS files**

Set `Components/Layout/MainLayout.razor.css` content to empty string.
Set `Components/Layout/NavMenu.razor.css` content to empty string.

- [ ] **Step 6: Run and verify layout renders**

```bash
dotnet run
```

Open browser. Verify: sidebar with nav items, topbar with "SindERP", dark mode toggle works, "Selecionar entidade" chip visible.

- [ ] **Step 7: Commit**

```bash
git add -A
git commit -m "feat: add MudBlazor layout shell and AppStateService"
```

---

### Task 3: Domain Models

**Files:**
- Create: `Models/Entidade.cs`
- Create: `Models/Contribuinte.cs`
- Create: `Models/Cobranca.cs`
- Create: `Models/Juridico.cs`
- Create: `Models/Financeiro.cs`
- Create: `Models/Mailing.cs`
- Create: `Models/Usuario.cs`

- [ ] **Step 1: Create Models/Entidade.cs**

```csharp
namespace BlazorTeste.Models;

public class Entidade
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Sigla { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string CidadeSede { get; set; } = "";
    public int TotalContribuintes { get; set; }
    public List<string> Cnaes { get; set; } = new();
    public List<string> Cidades { get; set; } = new();
}
```

- [ ] **Step 2: Create Models/Contribuinte.cs**

```csharp
namespace BlazorTeste.Models;

public class Contribuinte
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string RazaoSocial { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string Cnae { get; set; } = "";
    public decimal CapitalSocial { get; set; }
    public int NumeroFuncionarios { get; set; }
    public string RegimeTributario { get; set; } = "";
    public DateTime DataCadastro { get; set; }
    public DateTime DataAbertura { get; set; }
    public string Situacao { get; set; } = "Ativo";
    public List<Endereco> Enderecos { get; set; } = new();
    public List<Contato> Contatos { get; set; } = new();
    public List<Socio> Socios { get; set; } = new();
    public List<HistoricoMensal> Historico { get; set; } = new();
}

public class Endereco
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "Estabelecimento";
    public string Logradouro { get; set; } = "";
    public string Numero { get; set; } = "";
    public string Complemento { get; set; } = "";
    public string Bairro { get; set; } = "";
    public string Cidade { get; set; } = "";
    public string Estado { get; set; } = "SP";
    public string Cep { get; set; } = "";
}

public class Contato
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "Email";
    public string Valor { get; set; } = "";
    public string Descricao { get; set; } = "";
}

public class Socio
{
    public int Id { get; set; }
    public int ContribuinteId { get; set; }
    public string Matricula { get; set; } = "";
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public decimal ValorMensalidade { get; set; }
}

public class HistoricoMensal
{
    public int Id { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public decimal CapitalSocial { get; set; }
    public int NumeroFuncionarios { get; set; }
}
```

- [ ] **Step 3: Create Models/Cobranca.cs**

```csharp
namespace BlazorTeste.Models;

public enum StatusCobranca { Pendente, Pago, Vencido, Cancelado }
public enum TipoCobranca { Sindical, Confederativa, Associativa, Negocial }

public class Cobranca
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int ContribuinteId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public TipoCobranca Tipo { get; set; }
    public decimal Valor { get; set; }
    public decimal Multa { get; set; }
    public decimal Juros { get; set; }
    public DateTime Vencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusCobranca Status { get; set; }
    public string LinhaDigitavel { get; set; } = "";
    public int AnoReferencia { get; set; }
}
```

- [ ] **Step 4: Create Models/Juridico.cs**

```csharp
namespace BlazorTeste.Models;

public class Processo
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Numero { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string Vara { get; set; } = "";
    public string Tribunal { get; set; } = "";
    public string Situacao { get; set; } = "";
    public int AdvogadoId { get; set; }
    public string NomeAdvogado { get; set; } = "";
    public DateTime DataAbertura { get; set; }
    public string Descricao { get; set; } = "";
}

public class Advogado
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Nome { get; set; } = "";
    public string Oab { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefone { get; set; } = "";
    public int ProcessosAtivos { get; set; }
}

public class Audiencia
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public string NumeroProcesso { get; set; } = "";
    public int AdvogadoId { get; set; }
    public string NomeAdvogado { get; set; } = "";
    public DateTime DataHora { get; set; }
    public string Tipo { get; set; } = "";
    public string Local { get; set; } = "";
    public string Situacao { get; set; } = "Agendada";
}
```

- [ ] **Step 5: Create Models/Financeiro.cs**

```csharp
namespace BlazorTeste.Models;

public enum TipoLancamento { Entrada, Saida }

public class LancamentoFinanceiro
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public DateTime Data { get; set; }
    public string Categoria { get; set; } = "";
    public string Descricao { get; set; } = "";
    public int? FornecedorId { get; set; }
    public string NomeFornecedor { get; set; } = "";
    public decimal Valor { get; set; }
    public TipoLancamento Tipo { get; set; }
    public string ContaBancaria { get; set; } = "";
    public bool Realizado { get; set; }
}

public class Fornecedor
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Nome { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefone { get; set; } = "";
}
```

- [ ] **Step 6: Create Models/Mailing.cs**

```csharp
namespace BlazorTeste.Models;

public enum StatusCampanha { Rascunho, Agendada, Enviada, Erro }

public class Campanha
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Assunto { get; set; } = "";
    public string Destinatarios { get; set; } = "";
    public int TotalDestinatarios { get; set; }
    public DateTime? DataEnvio { get; set; }
    public StatusCampanha Status { get; set; }
    public string Criador { get; set; } = "";
}
```

- [ ] **Step 7: Create Models/Usuario.cs**

```csharp
namespace BlazorTeste.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime UltimoAcesso { get; set; }
    public List<PermissaoEntidade> Permissoes { get; set; } = new();
}

public class PermissaoEntidade
{
    public int EntidadeId { get; set; }
    public string NomeEntidade { get; set; } = "";
    public List<string> Modulos { get; set; } = new();
}
```

- [ ] **Step 8: Commit**

```bash
git add Models/
git commit -m "feat: add domain models"
```

---

### Task 4: Mock Data Services

**Files:**
- Create: `Services/EntidadeService.cs`
- Create: `Services/ContribuinteService.cs`
- Create: `Services/CobrancaService.cs`
- Create: `Services/JuridicoService.cs`
- Create: `Services/FinanceiroService.cs`
- Create: `Services/MailingService.cs`
- Create: `Services/UsuarioService.cs`
- Modify: `Program.cs`

- [ ] **Step 1: Create Services/EntidadeService.cs**

```csharp
using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class EntidadeService
{
    private readonly List<Entidade> _entidades = new()
    {
        new() { Id = 1, Nome = "Sindicato de Hotéis e Meios de Hospedagem", Sigla = "SINDHOSP", Cnpj = "12.345.678/0001-90", CidadeSede = "São Paulo", TotalContribuintes = 1243, Cnaes = new() { "5510-8/01", "5590-6/01", "5590-6/02" }, Cidades = new() { "São Paulo", "Guarulhos", "Osasco" } },
        new() { Id = 2, Nome = "Sindicato de Bares e Restaurantes", Sigla = "SINDBAR", Cnpj = "23.456.789/0001-01", CidadeSede = "São Paulo", TotalContribuintes = 8921, Cnaes = new() { "5611-2/01", "5611-2/03", "5612-1/00" }, Cidades = new() { "São Paulo", "São Bernardo do Campo", "Santo André" } },
        new() { Id = 3, Nome = "Sindicato de Empresas de Turismo", Sigla = "SINDETUR", Cnpj = "34.567.890/0001-12", CidadeSede = "São Paulo", TotalContribuintes = 562, Cnaes = new() { "7911-2/00", "7912-1/00", "7990-2/00" }, Cidades = new() { "São Paulo", "Campinas", "Sorocaba" } },
        new() { Id = 4, Nome = "Sindicato de Promotores de Eventos", Sigla = "SINDEVEN", Cnpj = "45.678.901/0001-23", CidadeSede = "São Paulo", TotalContribuintes = 387, Cnaes = new() { "8230-0/01", "8230-0/02" }, Cidades = new() { "São Paulo", "Barueri", "Cotia" } },
        new() { Id = 5, Nome = "Federação Estadual de Turismo e Hospitalidade", Sigla = "FETURH", Cnpj = "56.789.012/0001-34", CidadeSede = "São Paulo", TotalContribuintes = 4, Cnaes = new(), Cidades = new() { "São Paulo" } }
    };

    public List<Entidade> GetAll() => _entidades;
    public Entidade? GetById(int id) => _entidades.FirstOrDefault(e => e.Id == id);
}
```

- [ ] **Step 2: Create Services/ContribuinteService.cs**

```csharp
using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class ContribuinteService
{
    private readonly List<Contribuinte> _contribuintes;

    public ContribuinteService()
    {
        _contribuintes = GenerateContribuintes();
    }

    private static List<Contribuinte> GenerateContribuintes()
    {
        var razoesSociais = new[]
        {
            "HOTEL METROPOLITANO LTDA", "RESTAURANTE BOM SABOR LTDA", "BAR E LANCHONETE PONTO CERTO EIRELI",
            "POUSADA VILA SERENA LTDA", "CHURRASCARIA GAÚCHA DO SUL LTDA", "HOTEL CONFORT INN LTDA",
            "PIZZARIA BELLA NAPOLI EIRELI", "RESTAURANTE SABOR & ARTE LTDA", "CAFETERIA AROMA & COR EIRELI",
            "HOTEL PARK SUITES LTDA", "BAR DO ZEZINHO EIRELI", "RESTAURANTE ORIENTAL JAPAN LTDA",
            "HOSTEL URBAN STAY LTDA", "HAMBURGUERIA PRIME BURGUER LTDA", "PADARIA E RESTAURANTE TRIGO LTDA",
            "HOTEL BUSINESS CLASS LTDA", "CHOPERIA MÜNCHNER LTDA", "RESTAURANTE FRUTOS DO MAR LTDA",
            "MOTEL ESTRADA REAL LTDA", "PIZZARIA NAPOLITANA EIRELI", "SORVETERIA GELATO ITALIANO LTDA",
            "HOTEL EXECUTIVE PLAZA LTDA", "LANCHONETE FAST & GOOD EIRELI", "RESTAURANTE CANTINA ROMANA LTDA",
            "BAR E PETISCARIA BOTECO BOM LTDA", "HOTEL VILLA BELLA LTDA", "SUSHERIA TOKYO GRILL LTDA",
            "CHURRASCARIA BRAVA BRASA LTDA", "RESTAURANTE VEGETARIANO VIVA LTDA", "CONFEITARIA DOCE ARTE LTDA",
            "HOTEL GOLDEN PARK LTDA", "BISTRÔ FRANÇOISE LTDA", "RESTAURANTE EMPÓRIO GOURMET LTDA",
            "HOTEL SEASONS LTDA", "TAQUERIA EL SOMBRERO EIRELI", "CAFETERIA PRIMA COFFEE LTDA",
            "RESORT BEIRA MAR LTDA", "RESTAURANTE FAMÍLIA ITALIANA LTDA", "BAR KARAOKÊ FUN TIME LTDA",
            "HOTEL BOUTIQUE CHARM LTDA", "COZINHA FUSION ORIENTAL LTDA", "RESTAURANTE TERRAÇO PANORÂMICO LTDA",
            "APART HOTEL RESIDENCES LTDA", "BOTEQUIM CENTRAL EIRELI", "RESTAURANTE GRILL MASTER LTDA",
            "POUSADA DO SOL LTDA", "CAFETERIA XÍCARA & CIA EIRELI", "HOTEL BUSINESS EXECUTIVE LTDA",
            "CHOPERIA MALTE DOURADO LTDA", "RESTAURANTE VISTA LINDA LTDA"
        };

        var cidades = new[] { "São Paulo", "Guarulhos", "Osasco", "São Bernardo do Campo", "Santo André", "Campinas" };
        var regimes = new[] { "Simples Nacional", "Lucro Presumido", "Lucro Real" };
        var cnaes = new[] { "5510-8/01", "5611-2/01", "5611-2/03", "5590-6/01", "5612-1/00" };
        var rng = new Random(42);

        return razoesSociais.Select((nome, i) => new Contribuinte
        {
            Id = i + 1,
            EntidadeId = rng.Next(1, 5),
            RazaoSocial = nome,
            Cnpj = $"{rng.Next(10, 99)}.{rng.Next(100, 999)}.{rng.Next(100, 999)}/0001-{rng.Next(10, 99)}",
            Cnae = cnaes[rng.Next(cnaes.Length)],
            CapitalSocial = Math.Round((decimal)(rng.NextDouble() * 990000 + 10000), 2),
            NumeroFuncionarios = rng.Next(1, 200),
            RegimeTributario = regimes[rng.Next(regimes.Length)],
            DataCadastro = DateTime.Today.AddDays(-rng.Next(30, 3650)),
            DataAbertura = DateTime.Today.AddDays(-rng.Next(365, 7300)),
            Situacao = rng.Next(10) < 8 ? "Ativo" : "Inativo",
            Enderecos = new List<Endereco>
            {
                new() { Id = i * 3 + 1, Tipo = "Estabelecimento", Logradouro = $"Rua das Flores", Numero = rng.Next(1, 999).ToString(), Bairro = "Centro", Cidade = cidades[rng.Next(cidades.Length)], Cep = $"01{rng.Next(100, 999)}-{rng.Next(100, 999)}" },
                new() { Id = i * 3 + 2, Tipo = "Cobrança", Logradouro = "Av. Paulista", Numero = rng.Next(1, 2000).ToString(), Bairro = "Bela Vista", Cidade = "São Paulo", Cep = "01310-100" }
            },
            Contatos = new List<Contato>
            {
                new() { Id = i * 2 + 1, Tipo = "Email", Valor = $"contato@empresa{i + 1}.com.br", Descricao = "E-mail principal" },
                new() { Id = i * 2 + 2, Tipo = "Telefone", Valor = $"(11) {rng.Next(2000, 9999)}-{rng.Next(1000, 9999)}", Descricao = "Telefone comercial" }
            },
            Historico = Enumerable.Range(1, 6).Select(m => new HistoricoMensal
            {
                Id = i * 6 + m,
                Mes = DateTime.Today.AddMonths(-6 + m).Month,
                Ano = DateTime.Today.AddMonths(-6 + m).Year,
                CapitalSocial = Math.Round((decimal)(rng.NextDouble() * 990000 + 10000), 2),
                NumeroFuncionarios = rng.Next(1, 200)
            }).ToList()
        }).ToList();
    }

    public List<Contribuinte> GetAll() => _contribuintes;
    public Contribuinte? GetById(int id) => _contribuintes.FirstOrDefault(c => c.Id == id);
    public List<Contribuinte> GetByEntidade(int entidadeId) => _contribuintes.Where(c => c.EntidadeId == entidadeId).ToList();
}
```

- [ ] **Step 3: Create Services/CobrancaService.cs**

```csharp
using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class CobrancaService
{
    private readonly List<Cobranca> _cobrancas;

    public CobrancaService(ContribuinteService contribuinteService)
    {
        _cobrancas = GenerateCobrancas(contribuinteService.GetAll());
    }

    private static List<Cobranca> GenerateCobrancas(List<Contribuinte> contribuintes)
    {
        var rng = new Random(42);
        var tipos = Enum.GetValues<TipoCobranca>();
        var result = new List<Cobranca>();

        for (int i = 0; i < 80; i++)
        {
            var contribuinte = contribuintes[rng.Next(contribuintes.Count)];
            var tipo = tipos[rng.Next(tipos.Length)];
            var vencimento = DateTime.Today.AddDays(rng.Next(-90, 60));
            var valor = Math.Round((decimal)(rng.NextDouble() * 2000 + 100), 2);
            var pago = rng.Next(3) == 0;
            var vencido = !pago && vencimento < DateTime.Today;

            result.Add(new Cobranca
            {
                Id = i + 1,
                EntidadeId = contribuinte.EntidadeId,
                ContribuinteId = contribuinte.Id,
                RazaoSocialContribuinte = contribuinte.RazaoSocial,
                Tipo = tipo,
                Valor = valor,
                Multa = vencido ? Math.Round(valor * 0.02m, 2) : 0,
                Juros = vencido ? Math.Round(valor * 0.01m * Math.Max(0m, (decimal)(DateTime.Today - vencimento).TotalDays / 30), 2) : 0,
                Vencimento = vencimento,
                DataPagamento = pago ? vencimento.AddDays(rng.Next(-5, 5)) : null,
                Status = pago ? StatusCobranca.Pago : vencido ? StatusCobranca.Vencido : StatusCobranca.Pendente,
                LinhaDigitavel = $"34191.79001 01043.510047 91020.15000{rng.Next(1, 9)} {rng.Next(1, 9)} {rng.Next(10000, 99999)}0000000",
                AnoReferencia = DateTime.Today.Year
            });
        }

        return result;
    }

    public List<Cobranca> GetAll() => _cobrancas;
    public List<Cobranca> GetByStatus(StatusCobranca status) => _cobrancas.Where(c => c.Status == status).ToList();
    public List<Cobranca> GetByContribuinte(int contribuinteId) => _cobrancas.Where(c => c.ContribuinteId == contribuinteId).ToList();
}
```

- [ ] **Step 4: Create Services/JuridicoService.cs**

```csharp
using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class JuridicoService
{
    private readonly List<Advogado> _advogados;
    private readonly List<Processo> _processos;
    private readonly List<Audiencia> _audiencias;

    public JuridicoService()
    {
        _advogados = new List<Advogado>
        {
            new() { Id = 1, EntidadeId = 1, Nome = "Dr. Carlos Eduardo Mendes", Oab = "OAB/SP 123.456", Email = "carlos.mendes@jurmendes.adv.br", Telefone = "(11) 3456-7890", ProcessosAtivos = 5 },
            new() { Id = 2, EntidadeId = 1, Nome = "Dra. Fernanda Lima Costa", Oab = "OAB/SP 234.567", Email = "fernanda.costa@fclima.adv.br", Telefone = "(11) 4567-8901", ProcessosAtivos = 3 },
            new() { Id = 3, EntidadeId = 2, Nome = "Dr. Ricardo Alves Pinheiro", Oab = "OAB/SP 345.678", Email = "r.pinheiro@rapinheiro.adv.br", Telefone = "(11) 5678-9012", ProcessosAtivos = 7 },
            new() { Id = 4, EntidadeId = 2, Nome = "Dra. Amanda Souza Ferreira", Oab = "OAB/SP 456.789", Email = "amanda.ferreira@asferreira.adv.br", Telefone = "(11) 6789-0123", ProcessosAtivos = 4 },
            new() { Id = 5, EntidadeId = 3, Nome = "Dr. Marcelo Torres Braga", Oab = "OAB/SP 567.890", Email = "marcelo.braga@mtbraga.adv.br", Telefone = "(11) 7890-1234", ProcessosAtivos = 2 },
            new() { Id = 6, EntidadeId = 3, Nome = "Dra. Patrícia Gomes Vieira", Oab = "OAB/SP 678.901", Email = "patricia.vieira@pgvieira.adv.br", Telefone = "(11) 8901-2345", ProcessosAtivos = 6 },
            new() { Id = 7, EntidadeId = 4, Nome = "Dr. Roberto Nascimento Silva", Oab = "OAB/SP 789.012", Email = "roberto.silva@rnsilva.adv.br", Telefone = "(11) 9012-3456", ProcessosAtivos = 1 },
            new() { Id = 8, EntidadeId = 1, Nome = "Dra. Juliana Castro Moreira", Oab = "OAB/SP 890.123", Email = "juliana.moreira@jcmoreira.adv.br", Telefone = "(11) 0123-4567", ProcessosAtivos = 3 }
        };

        var rng = new Random(42);
        var tiposP = new[] { "Reclamação trabalhista", "Ação de cobrança", "Mandado de segurança", "Ação civil pública", "Execução fiscal" };
        var varas = new[] { "1ª Vara do Trabalho", "2ª Vara do Trabalho", "3ª Vara Cível", "4ª Vara Federal" };
        var tribunais = new[] { "TRT 2ª Região", "TJSP", "TRF 3ª Região" };
        var situacoes = new[] { "Em andamento", "Em andamento", "Em andamento", "Suspenso", "Encerrado" };

        _processos = Enumerable.Range(1, 15).Select(i =>
        {
            var adv = _advogados[rng.Next(_advogados.Count)];
            return new Processo
            {
                Id = i,
                EntidadeId = adv.EntidadeId,
                Numero = $"{rng.Next(1000, 9999)}-{rng.Next(10, 99)}.{DateTime.Today.Year}.5.02.{rng.Next(1000, 9999)}",
                Tipo = tiposP[rng.Next(tiposP.Length)],
                Vara = varas[rng.Next(varas.Length)],
                Tribunal = tribunais[rng.Next(tribunais.Length)],
                Situacao = situacoes[rng.Next(situacoes.Length)],
                AdvogadoId = adv.Id,
                NomeAdvogado = adv.Nome,
                DataAbertura = DateTime.Today.AddDays(-rng.Next(30, 1825)),
                Descricao = "Processo referente a ação judicial conforme documentação em arquivo."
            };
        }).ToList();

        var tiposA = new[] { "Inicial", "Instrução", "Julgamento", "Conciliação", "Perícia" };
        var locais = new[] { "Foro Central da Barra Funda", "Foro Regional da Lapa", "TRT 2ª Região", "TJSP — Pátio do Colégio" };
        var sitAud = new[] { "Agendada", "Agendada", "Realizada", "Cancelada" };

        _audiencias = Enumerable.Range(1, 20).Select(i =>
        {
            var processo = _processos[rng.Next(_processos.Count)];
            var adv = _advogados.First(a => a.Id == processo.AdvogadoId);
            return new Audiencia
            {
                Id = i,
                ProcessoId = processo.Id,
                NumeroProcesso = processo.Numero,
                AdvogadoId = adv.Id,
                NomeAdvogado = adv.Nome,
                DataHora = DateTime.Today.AddDays(rng.Next(-30, 60)).Date.AddHours(rng.Next(8, 17)),
                Tipo = tiposA[rng.Next(tiposA.Length)],
                Local = locais[rng.Next(locais.Length)],
                Situacao = sitAud[rng.Next(sitAud.Length)]
            };
        }).ToList();
    }

    public List<Advogado> GetAdvogados() => _advogados;
    public List<Processo> GetProcessos() => _processos;
    public List<Audiencia> GetAudiencias() => _audiencias;
}
```

- [ ] **Step 5: Create Services/FinanceiroService.cs**

```csharp
using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class FinanceiroService
{
    private readonly List<Fornecedor> _fornecedores;
    private readonly List<LancamentoFinanceiro> _lancamentos;

    public FinanceiroService()
    {
        _fornecedores = new List<Fornecedor>
        {
            new() { Id = 1, EntidadeId = 1, Nome = "Telecom Solutions LTDA", Cnpj = "11.222.333/0001-44", Categoria = "Telecomunicações", Email = "financeiro@telecomsolutions.com.br", Telefone = "(11) 3000-1000" },
            new() { Id = 2, EntidadeId = 1, Nome = "Limpeza Total Serviços EIRELI", Cnpj = "22.333.444/0001-55", Categoria = "Serviços Gerais", Email = "contato@limpezatotal.com.br", Telefone = "(11) 3000-2000" },
            new() { Id = 3, EntidadeId = 2, Nome = "TI Soluções Corporativas LTDA", Cnpj = "33.444.555/0001-66", Categoria = "Tecnologia", Email = "suporte@tisolucoes.com.br", Telefone = "(11) 3000-3000" },
            new() { Id = 4, EntidadeId = 2, Nome = "Segurança Preventiva LTDA", Cnpj = "44.555.666/0001-77", Categoria = "Segurança", Email = "operacional@segprev.com.br", Telefone = "(11) 3000-4000" },
            new() { Id = 5, EntidadeId = 1, Nome = "Assessoria Contábil ABC LTDA", Cnpj = "55.666.777/0001-88", Categoria = "Contabilidade", Email = "contabilidade@assessoriaabc.com.br", Telefone = "(11) 3000-5000" }
        };

        var rng = new Random(42);
        var categorias = new[] { "Aluguel", "Folha de Pagamento", "Energia Elétrica", "Telecomunicações", "Material de Escritório", "Serviços Terceirizados", "Impostos e Taxas" };
        var contas = new[] { "Santander — CC 12345-6", "Itaú — CC 67890-1", "Bradesco — CC 11223-4" };

        _lancamentos = Enumerable.Range(1, 30).Select(i =>
        {
            var isEntrada = rng.Next(4) == 0;
            var fornecedor = isEntrada ? null : _fornecedores[rng.Next(_fornecedores.Count)];
            var data = DateTime.Today.AddDays(-rng.Next(0, 180));
            return new LancamentoFinanceiro
            {
                Id = i,
                EntidadeId = rng.Next(1, 5),
                Data = data,
                Categoria = isEntrada ? "Arrecadação de Contribuintes" : categorias[rng.Next(categorias.Length)],
                Descricao = isEntrada ? "Recebimento de boletos — lote automático" : $"Pagamento referente a {(fornecedor?.Categoria ?? "serviço").ToLower()}",
                FornecedorId = fornecedor?.Id,
                NomeFornecedor = fornecedor?.Nome ?? "",
                Valor = Math.Round((decimal)(rng.NextDouble() * (isEntrada ? 50000 : 10000) + 500), 2),
                Tipo = isEntrada ? TipoLancamento.Entrada : TipoLancamento.Saida,
                ContaBancaria = contas[rng.Next(contas.Length)],
                Realizado = data <= DateTime.Today
            };
        }).ToList();
    }

    public List<LancamentoFinanceiro> GetLancamentos() => _lancamentos;
    public List<Fornecedor> GetFornecedores() => _fornecedores;
}
```

- [ ] **Step 6: Create Services/MailingService.cs**

```csharp
using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class MailingService
{
    private readonly List<Campanha> _campanhas = new()
    {
        new() { Id = 1, EntidadeId = 1, Assunto = "Comunicado — Assembleia Geral Ordinária 2026", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 1243, DataEnvio = DateTime.Today.AddDays(-30), Status = StatusCampanha.Enviada, Criador = "Ana Lima" },
        new() { Id = 2, EntidadeId = 1, Assunto = "Boleto de Contribuição Sindical — Vencimento Abril/2026", Destinatarios = "Contribuintes ativos", TotalDestinatarios = 1180, DataEnvio = DateTime.Today.AddDays(-5), Status = StatusCampanha.Enviada, Criador = "Carlos Silva" },
        new() { Id = 3, EntidadeId = 2, Assunto = "Edital — Eleição Diretoria 2026–2029", Destinatarios = "Sócios ativos", TotalDestinatarios = 420, DataEnvio = DateTime.Today.AddDays(7), Status = StatusCampanha.Agendada, Criador = "Fernanda Costa" },
        new() { Id = 4, EntidadeId = 2, Assunto = "Novo Portal do Contribuinte — Acesse já!", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 8921, DataEnvio = null, Status = StatusCampanha.Rascunho, Criador = "Pedro Martins" },
        new() { Id = 5, EntidadeId = 3, Assunto = "Convenção Coletiva 2026 — Resultado das Negociações", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 562, DataEnvio = DateTime.Today.AddDays(-15), Status = StatusCampanha.Enviada, Criador = "Lucia Fernandes" },
        new() { Id = 6, EntidadeId = 1, Assunto = "Lembrete — Prazo Final Contribuição Confederativa", Destinatarios = "Contribuintes com cobrança pendente", TotalDestinatarios = 287, DataEnvio = DateTime.Today.AddDays(-2), Status = StatusCampanha.Erro, Criador = "Carlos Silva" },
        new() { Id = 7, EntidadeId = 4, Assunto = "Curso de Capacitação — Gestão de Eventos 2026", Destinatarios = "Sócios ativos", TotalDestinatarios = 150, DataEnvio = null, Status = StatusCampanha.Rascunho, Criador = "Beatriz Nunes" },
        new() { Id = 8, EntidadeId = 2, Assunto = "Relatório Mensal — Negociações Coletivas Março/2026", Destinatarios = "Contribuintes sócios", TotalDestinatarios = 320, DataEnvio = DateTime.Today.AddDays(14), Status = StatusCampanha.Agendada, Criador = "Fernanda Costa" },
        new() { Id = 9, EntidadeId = 1, Assunto = "Atualização Cadastral Obrigatória — Prazo até 30/06", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 1243, DataEnvio = DateTime.Today.AddDays(-60), Status = StatusCampanha.Enviada, Criador = "Ana Lima" },
        new() { Id = 10, EntidadeId = 3, Assunto = "Benefícios para Sócios — Novidades 2026", Destinatarios = "Contribuintes", TotalDestinatarios = 562, DataEnvio = null, Status = StatusCampanha.Rascunho, Criador = "Lucia Fernandes" }
    };

    public List<Campanha> GetAll() => _campanhas;
}
```

- [ ] **Step 7: Create Services/UsuarioService.cs**

```csharp
using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class UsuarioService
{
    private readonly List<Usuario> _usuarios = new()
    {
        new() { Id = 1, Nome = "Ana Lima", Email = "ana.lima@sindhosp.org.br", UltimoAcesso = DateTime.Now.AddMinutes(-15), Permissoes = new() { new() { EntidadeId = 1, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing" } }, new() { EntidadeId = 5, NomeEntidade = "FETURH", Modulos = new() { "Contribuintes" } } } },
        new() { Id = 2, Nome = "Carlos Silva", Email = "carlos.silva@sindhosp.org.br", UltimoAcesso = DateTime.Now.AddHours(-2), Permissoes = new() { new() { EntidadeId = 1, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Mailing" } } } },
        new() { Id = 3, Nome = "Fernanda Costa", Email = "fernanda.costa@sindbar.org.br", UltimoAcesso = DateTime.Now.AddHours(-1), Permissoes = new() { new() { EntidadeId = 2, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Usuários" } } } },
        new() { Id = 4, Nome = "Pedro Martins", Email = "pedro.martins@sindbar.org.br", UltimoAcesso = DateTime.Now.AddDays(-1), Permissoes = new() { new() { EntidadeId = 2, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Mailing" } } } },
        new() { Id = 5, Nome = "Admin Sistema", Email = "admin@dpi.com.br", UltimoAcesso = DateTime.Now.AddHours(-3), Permissoes = new() { new() { EntidadeId = 1, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Entidades", "Usuários" } }, new() { EntidadeId = 2, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Entidades", "Usuários" } } } },
        new() { Id = 6, Nome = "Lucia Fernandes", Email = "lucia.fernandes@sindetur.org.br", UltimoAcesso = DateTime.Now.AddDays(-2), Permissoes = new() { new() { EntidadeId = 3, NomeEntidade = "SINDETUR", Modulos = new() { "Contribuintes", "Cobranças", "Mailing" } }, new() { EntidadeId = 4, NomeEntidade = "SINDEVEN", Modulos = new() { "Contribuintes" } } } }
    };

    public List<Usuario> GetAll() => _usuarios;
}
```

- [ ] **Step 8: Register all services in Program.cs**

```csharp
using BlazorTeste.Components;
using BlazorTeste.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddSingleton<AppStateService>();
builder.Services.AddSingleton<EntidadeService>();
builder.Services.AddSingleton<ContribuinteService>();
builder.Services.AddSingleton<CobrancaService>();
builder.Services.AddSingleton<JuridicoService>();
builder.Services.AddSingleton<FinanceiroService>();
builder.Services.AddSingleton<MailingService>();
builder.Services.AddSingleton<UsuarioService>();

await builder.Build().RunAsync();
```

- [ ] **Step 9: Build and verify**

```bash
dotnet build
```

Expected: 0 errors.

- [ ] **Step 10: Commit**

```bash
git add Services/ Models/ Program.cs
git commit -m "feat: add domain models and mock data services"
```

---

### Task 5: Dashboard Page

**Files:**
- Create: `Components/Pages/Dashboard/Dashboard.razor`

- [ ] **Step 1: Create Components/Pages/Dashboard/Dashboard.razor**

```razor
@page "/"
@inject ContribuinteService ContribuinteService
@inject CobrancaService CobrancaService
@inject JuridicoService JuridicoService
@inject AppStateService AppState

<PageTitle>Dashboard — SindERP</PageTitle>

<MudText Typo="Typo.h5" Class="mb-1" Style="font-weight:600;">Dashboard</MudText>
<MudText Typo="Typo.body2" Color="Color.Secondary" Class="mb-4">
    @(AppState.EntidadeAtiva?.Nome ?? "Visão geral do sistema")
</MudText>

<MudGrid Spacing="3">
    <MudItem xs="12" sm="6" md="3">
        <MudCard Elevation="2" Class="rounded-xl">
            <MudCardContent Class="pa-4">
                <div class="d-flex align-center gap-3">
                    <MudIcon Icon="@Icons.Material.Filled.Business"
                             Style="font-size:2.5rem;" Color="Color.Primary" />
                    <div>
                        <MudText Typo="Typo.h4" Style="font-weight:700; line-height:1.1;">
                            @_totalContribuintes.ToString("N0")
                        </MudText>
                        <MudText Typo="Typo.caption" Color="Color.Secondary">Contribuintes</MudText>
                    </div>
                </div>
            </MudCardContent>
        </MudCard>
    </MudItem>

    <MudItem xs="12" sm="6" md="3">
        <MudCard Elevation="2" Class="rounded-xl">
            <MudCardContent Class="pa-4">
                <div class="d-flex align-center gap-3">
                    <MudIcon Icon="@Icons.Material.Filled.Warning"
                             Style="font-size:2.5rem;" Color="Color.Warning" />
                    <div>
                        <MudText Typo="Typo.h4" Style="font-weight:700; line-height:1.1;">
                            @_cobrancasEmAberto.ToString("N0")
                        </MudText>
                        <MudText Typo="Typo.caption" Color="Color.Secondary">Cobranças em aberto</MudText>
                    </div>
                </div>
            </MudCardContent>
        </MudCard>
    </MudItem>

    <MudItem xs="12" sm="6" md="3">
        <MudCard Elevation="2" Class="rounded-xl">
            <MudCardContent Class="pa-4">
                <div class="d-flex align-center gap-3">
                    <MudIcon Icon="@Icons.Material.Filled.AttachMoney"
                             Style="font-size:2.5rem;" Color="Color.Success" />
                    <div>
                        <MudText Typo="Typo.h4" Style="font-weight:700; line-height:1.1;">
                            @_arrecadadoMes.ToString("C0")
                        </MudText>
                        <MudText Typo="Typo.caption" Color="Color.Secondary">Arrecadado no mês</MudText>
                    </div>
                </div>
            </MudCardContent>
        </MudCard>
    </MudItem>

    <MudItem xs="12" sm="6" md="3">
        <MudCard Elevation="2" Class="rounded-xl">
            <MudCardContent Class="pa-4">
                <div class="d-flex align-center gap-3">
                    <MudIcon Icon="@Icons.Material.Filled.Gavel"
                             Style="font-size:2.5rem;" Color="Color.Error" />
                    <div>
                        <MudText Typo="Typo.h4" Style="font-weight:700; line-height:1.1;">
                            @_processosAtivos
                        </MudText>
                        <MudText Typo="Typo.caption" Color="Color.Secondary">Processos ativos</MudText>
                    </div>
                </div>
            </MudCardContent>
        </MudCard>
    </MudItem>

    <MudItem xs="12" md="8">
        <MudCard Elevation="2" Class="rounded-xl">
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.h6" Style="font-weight:600;">Arrecadação — Últimos 6 Meses</MudText>
                </CardHeaderContent>
            </MudCardHeader>
            <MudCardContent>
                <MudChart ChartType="ChartType.Bar"
                          ChartSeries="@_chartSeries"
                          XAxisLabels="@_chartLabels"
                          Width="100%"
                          Height="280px"
                          ChartOptions="@(new ChartOptions { YAxisFormat = "C0" })" />
            </MudCardContent>
        </MudCard>
    </MudItem>

    <MudItem xs="12" md="4">
        <MudCard Elevation="2" Class="rounded-xl" Style="height:100%;">
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.h6" Style="font-weight:600;">Próximas Audiências</MudText>
                </CardHeaderContent>
            </MudCardHeader>
            <MudCardContent Class="pt-0">
                @foreach (var aud in _proximasAudiencias)
                {
                    <div class="d-flex flex-column mb-3">
                        <MudText Typo="Typo.body2" Style="font-weight:500;">@aud.NumeroProcesso</MudText>
                        <MudText Typo="Typo.caption" Color="Color.Secondary">@aud.NomeAdvogado</MudText>
                        <MudText Typo="Typo.caption" Color="Color.Primary">@aud.DataHora.ToString("dd/MM/yyyy HH:mm") — @aud.Tipo</MudText>
                    </div>
                    <MudDivider Class="mb-3" />
                }
            </MudCardContent>
        </MudCard>
    </MudItem>

    <MudItem xs="12">
        <MudCard Elevation="2" Class="rounded-xl">
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.h6" Style="font-weight:600;">Últimas Cobranças</MudText>
                </CardHeaderContent>
            </MudCardHeader>
            <MudCardContent Class="pt-0">
                <MudTable Items="_ultimasCobrancas" Dense="true" Hover="true" Striped="false" Elevation="0">
                    <HeaderContent>
                        <MudTh>Contribuinte</MudTh>
                        <MudTh>Tipo</MudTh>
                        <MudTh>Valor</MudTh>
                        <MudTh>Vencimento</MudTh>
                        <MudTh>Status</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd DataLabel="Contribuinte">
                            <MudText Typo="Typo.body2">@context.RazaoSocialContribuinte</MudText>
                        </MudTd>
                        <MudTd DataLabel="Tipo">@context.Tipo</MudTd>
                        <MudTd DataLabel="Valor">@context.Valor.ToString("C2")</MudTd>
                        <MudTd DataLabel="Vencimento">@context.Vencimento.ToString("dd/MM/yyyy")</MudTd>
                        <MudTd DataLabel="Status">
                            <MudChip T="string" Size="Size.Small" Color="@GetStatusColor(context.Status)" Variant="Variant.Filled">
                                @context.Status
                            </MudChip>
                        </MudTd>
                    </RowTemplate>
                </MudTable>
            </MudCardContent>
        </MudCard>
    </MudItem>
</MudGrid>

@code {
    private int _totalContribuintes;
    private int _cobrancasEmAberto;
    private decimal _arrecadadoMes;
    private int _processosAtivos;
    private List<Cobranca> _ultimasCobrancas = new();
    private List<Audiencia> _proximasAudiencias = new();
    private List<ChartSeries> _chartSeries = new();
    private string[] _chartLabels = Array.Empty<string>();

    protected override void OnInitialized()
    {
        var contribuintes = ContribuinteService.GetAll();
        var cobrancas = CobrancaService.GetAll();
        var audiencias = JuridicoService.GetAudiencias();

        _totalContribuintes = contribuintes.Count;
        _cobrancasEmAberto = cobrancas.Count(c => c.Status is StatusCobranca.Pendente or StatusCobranca.Vencido);
        _arrecadadoMes = cobrancas
            .Where(c => c.Status == StatusCobranca.Pago && c.DataPagamento?.Month == DateTime.Today.Month)
            .Sum(c => c.Valor);
        _processosAtivos = JuridicoService.GetProcessos().Count(p => p.Situacao == "Em andamento");
        _ultimasCobrancas = cobrancas.OrderByDescending(c => c.Vencimento).Take(5).ToList();
        _proximasAudiencias = audiencias
            .Where(a => a.Situacao == "Agendada" && a.DataHora >= DateTime.Today)
            .OrderBy(a => a.DataHora)
            .Take(4)
            .ToList();

        var months = Enumerable.Range(0, 6).Select(i => DateTime.Today.AddMonths(-5 + i)).ToList();
        _chartLabels = months.Select(m => m.ToString("MMM/yy")).ToArray();
        _chartSeries = new List<ChartSeries>
        {
            new ChartSeries
            {
                Name = "Arrecadado (R$)",
                Data = months.Select(m =>
                    (double)cobrancas
                        .Where(c => c.Status == StatusCobranca.Pago &&
                                    c.DataPagamento?.Month == m.Month &&
                                    c.DataPagamento?.Year == m.Year)
                        .Sum(c => c.Valor))
                .ToArray()
            }
        };
    }

    private Color GetStatusColor(StatusCobranca status) => status switch
    {
        StatusCobranca.Pago => Color.Success,
        StatusCobranca.Vencido => Color.Error,
        StatusCobranca.Pendente => Color.Info,
        _ => Color.Default
    };
}
```

- [ ] **Step 2: Run and verify dashboard renders with KPIs and chart**

```bash
dotnet run
```

Navigate to `/`. Verify: 4 KPI cards, bar chart, audiências list, cobranças table.

- [ ] **Step 3: Commit**

```bash
git add Components/Pages/Dashboard/
git commit -m "feat: add Dashboard page with KPIs and chart"
```

---

### Task 6: Contribuintes Page

**Files:**
- Create: `Components/Pages/Contribuintes/Contribuintes.razor`

- [ ] **Step 1: Create Components/Pages/Contribuintes/Contribuintes.razor**

```razor
@page "/contribuintes"
@inject ContribuinteService ContribuinteService

<PageTitle>Contribuintes — SindERP</PageTitle>

<div class="d-flex align-center justify-space-between mb-4">
    <div>
        <MudText Typo="Typo.h5" Style="font-weight:600;">Contribuintes</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">@_contribuintes.Count registros</MudText>
    </div>
    <MudButton Variant="Variant.Filled" Color="Color.Primary"
               StartIcon="@Icons.Material.Filled.Add">
        Novo Contribuinte
    </MudButton>
</div>

<MudCard Elevation="2" Class="rounded-xl">
    <MudCardContent Class="pt-2">
        <MudDataGrid T="Contribuinte"
                     Items="_contribuintes"
                     Filterable="true"
                     SortMode="SortMode.Multiple"
                     Pageable="true"
                     PageSize="10"
                     Hover="true"
                     RowClick="OnRowClick"
                     Class="cursor-pointer">
            <Columns>
                <PropertyColumn Property="c => c.Cnpj" Title="CNPJ" />
                <PropertyColumn Property="c => c.RazaoSocial" Title="Razão Social" />
                <PropertyColumn Property="c => c.Cnae" Title="CNAE" />
                <PropertyColumn Property="c => c.RegimeTributario" Title="Regime" />
                <PropertyColumn Property="c => c.NumeroFuncionarios" Title="Funcionários" />
                <TemplateColumn Title="Capital Social">
                    <CellTemplate>
                        @context.Item.CapitalSocial.ToString("C0")
                    </CellTemplate>
                </TemplateColumn>
                <TemplateColumn Title="Situação">
                    <CellTemplate>
                        <MudChip T="string" Size="Size.Small"
                                 Color="@(context.Item.Situacao == "Ativo" ? Color.Success : Color.Default)"
                                 Variant="Variant.Filled">
                            @context.Item.Situacao
                        </MudChip>
                    </CellTemplate>
                </TemplateColumn>
            </Columns>
        </MudDataGrid>
    </MudCardContent>
</MudCard>

<MudDrawer @bind-Open="_drawerOpen"
           Anchor="Anchor.End"
           Elevation="4"
           Width="480px"
           Variant="DrawerVariant.Temporary">
    @if (_selected is not null)
    {
        <MudDrawerHeader Class="pa-4">
            <div class="d-flex align-center justify-space-between" style="width:100%;">
                <MudText Typo="Typo.h6" Style="font-weight:600;">Detalhe do Contribuinte</MudText>
                <MudIconButton Icon="@Icons.Material.Filled.Close"
                               OnClick="@(() => _drawerOpen = false)" />
            </div>
        </MudDrawerHeader>
        <MudDivider />
        <div class="pa-4">
            <MudText Typo="Typo.subtitle1" Style="font-weight:600;" Class="mb-1">@_selected.RazaoSocial</MudText>
            <MudText Typo="Typo.body2" Color="Color.Secondary" Class="mb-4">@_selected.Cnpj</MudText>

            <MudTabs Elevation="0" Rounded="true" ApplyEffectsToContainer="true" PanelClass="pt-3">
                <MudTabPanel Text="Dados">
                    <MudGrid Spacing="2">
                        <MudItem xs="6">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">CNAE</MudText>
                            <MudText Typo="Typo.body2">@_selected.Cnae</MudText>
                        </MudItem>
                        <MudItem xs="6">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Regime Tributário</MudText>
                            <MudText Typo="Typo.body2">@_selected.RegimeTributario</MudText>
                        </MudItem>
                        <MudItem xs="6">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Capital Social</MudText>
                            <MudText Typo="Typo.body2">@_selected.CapitalSocial.ToString("C2")</MudText>
                        </MudItem>
                        <MudItem xs="6">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Funcionários</MudText>
                            <MudText Typo="Typo.body2">@_selected.NumeroFuncionarios</MudText>
                        </MudItem>
                        <MudItem xs="6">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Data Abertura</MudText>
                            <MudText Typo="Typo.body2">@_selected.DataAbertura.ToString("dd/MM/yyyy")</MudText>
                        </MudItem>
                        <MudItem xs="6">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Situação</MudText>
                            <MudChip T="string" Size="Size.Small"
                                     Color="@(_selected.Situacao == "Ativo" ? Color.Success : Color.Default)">
                                @_selected.Situacao
                            </MudChip>
                        </MudItem>
                    </MudGrid>
                </MudTabPanel>

                <MudTabPanel Text="Endereços">
                    @foreach (var end in _selected.Enderecos)
                    {
                        <MudCard Outlined="true" Class="mb-2 rounded-lg">
                            <MudCardContent Class="pa-3">
                                <MudChip T="string" Size="Size.Small" Color="Color.Primary" Class="mb-2">@end.Tipo</MudChip>
                                <MudText Typo="Typo.body2">@end.Logradouro, @end.Numero @end.Complemento</MudText>
                                <MudText Typo="Typo.body2">@end.Bairro — @end.Cidade/@end.Estado</MudText>
                                <MudText Typo="Typo.caption" Color="Color.Secondary">CEP: @end.Cep</MudText>
                            </MudCardContent>
                        </MudCard>
                    }
                </MudTabPanel>

                <MudTabPanel Text="Contatos">
                    @foreach (var contato in _selected.Contatos)
                    {
                        <div class="d-flex align-center gap-2 mb-2">
                            <MudIcon Icon="@(contato.Tipo == "Email" ? Icons.Material.Filled.Email : Icons.Material.Filled.Phone)"
                                     Size="Size.Small" Color="Color.Primary" />
                            <div>
                                <MudText Typo="Typo.body2">@contato.Valor</MudText>
                                <MudText Typo="Typo.caption" Color="Color.Secondary">@contato.Descricao</MudText>
                            </div>
                        </div>
                    }
                </MudTabPanel>

                <MudTabPanel Text="Sócios">
                    @if (_selected.Socios.Count == 0)
                    {
                        <MudText Typo="Typo.body2" Color="Color.Secondary" Class="pa-2">
                            Nenhum sócio vinculado.
                        </MudText>
                    }
                    else
                    {
                        <MudTable Items="_selected.Socios" Dense="true" Elevation="0">
                            <HeaderContent>
                                <MudTh>Matrícula</MudTh>
                                <MudTh>Início</MudTh>
                                <MudTh>Fim</MudTh>
                                <MudTh>Mensalidade</MudTh>
                            </HeaderContent>
                            <RowTemplate>
                                <MudTd>@context.Matricula</MudTd>
                                <MudTd>@context.DataInicio.ToString("dd/MM/yyyy")</MudTd>
                                <MudTd>@(context.DataFim.HasValue ? context.DataFim.Value.ToString("dd/MM/yyyy") : "—")</MudTd>
                                <MudTd>@context.ValorMensalidade.ToString("C2")</MudTd>
                            </RowTemplate>
                        </MudTable>
                    }
                </MudTabPanel>

                <MudTabPanel Text="Histórico">
                    <MudTable Items="_selected.Historico" Dense="true" Elevation="0">
                        <HeaderContent>
                            <MudTh>Mês/Ano</MudTh>
                            <MudTh>Capital Social</MudTh>
                            <MudTh>Funcionários</MudTh>
                        </HeaderContent>
                        <RowTemplate>
                            <MudTd>@context.Mes.ToString("00")/@context.Ano</MudTd>
                            <MudTd>@context.CapitalSocial.ToString("C0")</MudTd>
                            <MudTd>@context.NumeroFuncionarios</MudTd>
                        </RowTemplate>
                    </MudTable>
                </MudTabPanel>
            </MudTabs>
        </div>
    }
</MudDrawer>

@code {
    private List<Contribuinte> _contribuintes = new();
    private Contribuinte? _selected;
    private bool _drawerOpen;

    protected override void OnInitialized()
    {
        _contribuintes = ContribuinteService.GetAll();
    }

    private void OnRowClick(DataGridRowClickEventArgs<Contribuinte> args)
    {
        _selected = args.Item;
        _drawerOpen = true;
    }
}
```

- [ ] **Step 2: Run and verify DataGrid loads, row click opens drawer with tabs**

```bash
dotnet run
```

Navigate to `/contribuintes`. Click any row and verify the detail drawer opens with 4 tabs.

- [ ] **Step 3: Commit**

```bash
git add Components/Pages/Contribuintes/
git commit -m "feat: add Contribuintes page with DataGrid and detail drawer"
```

---

### Task 7: Cobranças Page

**Files:**
- Create: `Components/Pages/Cobranca/Cobrancas.razor`

- [ ] **Step 1: Create Components/Pages/Cobranca/Cobrancas.razor**

```razor
@page "/cobrancas"
@inject CobrancaService CobrancaService

<PageTitle>Cobranças — SindERP</PageTitle>

<div class="d-flex align-center justify-space-between mb-4">
    <div>
        <MudText Typo="Typo.h5" Style="font-weight:600;">Cobranças</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">@_all.Count registros</MudText>
    </div>
    <MudButton Variant="Variant.Filled" Color="Color.Primary"
               StartIcon="@Icons.Material.Filled.Add">
        Emitir Cobrança
    </MudButton>
</div>

<MudCard Elevation="2" Class="rounded-xl">
    <MudCardContent Class="pt-0">
        <MudTabs Elevation="0" Rounded="false" PanelClass="pt-2">
            <MudTabPanel Text="Todas" BadgeData="@_all.Count" BadgeColor="Color.Default">
                @CobrancaTable(_all)
            </MudTabPanel>
            <MudTabPanel Text="Pendentes" BadgeData="@_pendentes.Count" BadgeColor="Color.Info">
                @CobrancaTable(_pendentes)
            </MudTabPanel>
            <MudTabPanel Text="Vencidas" BadgeData="@_vencidas.Count" BadgeColor="Color.Error">
                @CobrancaTable(_vencidas)
            </MudTabPanel>
            <MudTabPanel Text="Pagas" BadgeData="@_pagas.Count" BadgeColor="Color.Success">
                @CobrancaTable(_pagas)
            </MudTabPanel>
        </MudTabs>
    </MudCardContent>
</MudCard>

<MudDialog @bind-IsVisible="_dialogOpen">
    <TitleContent>
        <MudText Typo="Typo.h6">Detalhe do Boleto</MudText>
    </TitleContent>
    <DialogContent>
        @if (_selectedCobranca is not null)
        {
            <MudGrid Spacing="2">
                <MudItem xs="12">
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Contribuinte</MudText>
                    <MudText Typo="Typo.body1" Style="font-weight:500;">@_selectedCobranca.RazaoSocialContribuinte</MudText>
                </MudItem>
                <MudItem xs="6">
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Tipo</MudText>
                    <MudText Typo="Typo.body1">@_selectedCobranca.Tipo</MudText>
                </MudItem>
                <MudItem xs="6">
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Ano Referência</MudText>
                    <MudText Typo="Typo.body1">@_selectedCobranca.AnoReferencia</MudText>
                </MudItem>
                <MudItem xs="4">
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Valor</MudText>
                    <MudText Typo="Typo.body1">@_selectedCobranca.Valor.ToString("C2")</MudText>
                </MudItem>
                <MudItem xs="4">
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Multa</MudText>
                    <MudText Typo="Typo.body1" Color="Color.Error">@_selectedCobranca.Multa.ToString("C2")</MudText>
                </MudItem>
                <MudItem xs="4">
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Juros</MudText>
                    <MudText Typo="Typo.body1" Color="Color.Warning">@_selectedCobranca.Juros.ToString("C2")</MudText>
                </MudItem>
                <MudItem xs="6">
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Vencimento</MudText>
                    <MudText Typo="Typo.body1">@_selectedCobranca.Vencimento.ToString("dd/MM/yyyy")</MudText>
                </MudItem>
                <MudItem xs="6">
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Status</MudText>
                    <MudChip T="string" Size="Size.Small" Color="@GetStatusColor(_selectedCobranca.Status)">
                        @_selectedCobranca.Status
                    </MudChip>
                </MudItem>
                <MudItem xs="12">
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Linha Digitável</MudText>
                    <MudTextField Value="_selectedCobranca.LinhaDigitavel"
                                  Variant="Variant.Outlined"
                                  ReadOnly="true"
                                  Adornment="Adornment.End"
                                  AdornmentIcon="@Icons.Material.Filled.ContentCopy"
                                  Margin="Margin.Dense" />
                </MudItem>
            </MudGrid>
        }
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _dialogOpen = false)">Fechar</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary"
                   StartIcon="@Icons.Material.Filled.Print">
            Emitir 2ª Via
        </MudButton>
    </DialogActions>
</MudDialog>

@code {
    private List<Cobranca> _all = new();
    private List<Cobranca> _pendentes = new();
    private List<Cobranca> _vencidas = new();
    private List<Cobranca> _pagas = new();
    private Cobranca? _selectedCobranca;
    private bool _dialogOpen;

    protected override void OnInitialized()
    {
        _all = CobrancaService.GetAll();
        _pendentes = CobrancaService.GetByStatus(StatusCobranca.Pendente);
        _vencidas = CobrancaService.GetByStatus(StatusCobranca.Vencido);
        _pagas = CobrancaService.GetByStatus(StatusCobranca.Pago);
    }

    private void OpenDetail(Cobranca cobranca)
    {
        _selectedCobranca = cobranca;
        _dialogOpen = true;
    }

    private Color GetStatusColor(StatusCobranca status) => status switch
    {
        StatusCobranca.Pago => Color.Success,
        StatusCobranca.Vencido => Color.Error,
        StatusCobranca.Pendente => Color.Info,
        _ => Color.Default
    };

    private RenderFragment CobrancaTable(List<Cobranca> items) => __builder =>
    {
        <MudTable T="Cobranca" Items="items" Dense="true" Hover="true" Elevation="0"
                  RowClassFunc="@((c, _) => "cursor-pointer")"
                  OnRowClick="@((TableRowClickEventArgs<Cobranca> r) => OpenDetail(r.Item))">
            <HeaderContent>
                <MudTh>Contribuinte</MudTh>
                <MudTh>Tipo</MudTh>
                <MudTh>Valor</MudTh>
                <MudTh>Vencimento</MudTh>
                <MudTh>Status</MudTh>
            </HeaderContent>
            <RowTemplate>
                <MudTd>@context.RazaoSocialContribuinte</MudTd>
                <MudTd>@context.Tipo</MudTd>
                <MudTd>@context.Valor.ToString("C2")</MudTd>
                <MudTd>@context.Vencimento.ToString("dd/MM/yyyy")</MudTd>
                <MudTd>
                    <MudChip T="string" Size="Size.Small" Color="@GetStatusColor(context.Status)">
                        @context.Status
                    </MudChip>
                </MudTd>
            </RowTemplate>
        </MudTable>;
    };
}
```

- [ ] **Step 2: Run and verify**

```bash
dotnet run
```

Navigate to `/cobrancas`. Verify: tabs with counts, table per tab, click opens dialog with linha digitável.

- [ ] **Step 3: Commit**

```bash
git add Components/Pages/Cobranca/
git commit -m "feat: add Cobranças page with tabs and detail dialog"
```

---

### Task 8: Jurídico Page

**Files:**
- Create: `Components/Pages/Juridico/Juridico.razor`

- [ ] **Step 1: Create Components/Pages/Juridico/Juridico.razor**

```razor
@page "/juridico"
@inject JuridicoService JuridicoService

<PageTitle>Jurídico — SindERP</PageTitle>

<div class="d-flex align-center justify-space-between mb-4">
    <MudText Typo="Typo.h5" Style="font-weight:600;">Jurídico</MudText>
    <MudButton Variant="Variant.Filled" Color="Color.Primary"
               StartIcon="@Icons.Material.Filled.Add">
        Novo Processo
    </MudButton>
</div>

<MudCard Elevation="2" Class="rounded-xl">
    <MudCardContent Class="pt-0">
        <MudTabs Elevation="0" PanelClass="pt-3">
            <MudTabPanel Text="Processos">
                <MudTable Items="_processos" Dense="true" Hover="true" Elevation="0" Striped="false">
                    <HeaderContent>
                        <MudTh>Número</MudTh>
                        <MudTh>Tipo</MudTh>
                        <MudTh>Vara</MudTh>
                        <MudTh>Tribunal</MudTh>
                        <MudTh>Advogado</MudTh>
                        <MudTh>Situação</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd><MudText Typo="Typo.body2" Style="font-family:monospace;">@context.Numero</MudText></MudTd>
                        <MudTd>@context.Tipo</MudTd>
                        <MudTd>@context.Vara</MudTd>
                        <MudTd>@context.Tribunal</MudTd>
                        <MudTd>@context.NomeAdvogado</MudTd>
                        <MudTd>
                            <MudChip T="string" Size="Size.Small"
                                     Color="@GetSituacaoColor(context.Situacao)">
                                @context.Situacao
                            </MudChip>
                        </MudTd>
                    </RowTemplate>
                </MudTable>
            </MudTabPanel>

            <MudTabPanel Text="Advogados">
                <MudTable Items="_advogados" Dense="true" Hover="true" Elevation="0">
                    <HeaderContent>
                        <MudTh>Nome</MudTh>
                        <MudTh>OAB</MudTh>
                        <MudTh>E-mail</MudTh>
                        <MudTh>Telefone</MudTh>
                        <MudTh>Processos ativos</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd>
                            <div class="d-flex align-center gap-2">
                                <MudAvatar Size="Size.Small" Color="Color.Primary">
                                    @context.Nome[0]
                                </MudAvatar>
                                @context.Nome
                            </div>
                        </MudTd>
                        <MudTd>@context.Oab</MudTd>
                        <MudTd>@context.Email</MudTd>
                        <MudTd>@context.Telefone</MudTd>
                        <MudTd>
                            <MudChip T="string" Size="Size.Small" Color="Color.Primary">@context.ProcessosAtivos</MudChip>
                        </MudTd>
                    </RowTemplate>
                </MudTable>
            </MudTabPanel>

            <MudTabPanel Text="Audiências">
                <MudTable Items="_audiencias" Dense="true" Hover="true" Elevation="0">
                    <HeaderContent>
                        <MudTh>Data/Hora</MudTh>
                        <MudTh>Processo</MudTh>
                        <MudTh>Tipo</MudTh>
                        <MudTh>Advogado</MudTh>
                        <MudTh>Local</MudTh>
                        <MudTh>Situação</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd><MudText Typo="Typo.body2" Style="white-space:nowrap;">@context.DataHora.ToString("dd/MM/yyyy HH:mm")</MudText></MudTd>
                        <MudTd><MudText Typo="Typo.body2" Style="font-family:monospace;font-size:0.75rem;">@context.NumeroProcesso</MudText></MudTd>
                        <MudTd>@context.Tipo</MudTd>
                        <MudTd>@context.NomeAdvogado</MudTd>
                        <MudTd>@context.Local</MudTd>
                        <MudTd>
                            <MudChip T="string" Size="Size.Small"
                                     Color="@GetAudSituacaoColor(context.Situacao)">
                                @context.Situacao
                            </MudChip>
                        </MudTd>
                    </RowTemplate>
                </MudTable>
            </MudTabPanel>
        </MudTabs>
    </MudCardContent>
</MudCard>

@code {
    private List<Processo> _processos = new();
    private List<Advogado> _advogados = new();
    private List<Audiencia> _audiencias = new();

    protected override void OnInitialized()
    {
        _processos = JuridicoService.GetProcessos();
        _advogados = JuridicoService.GetAdvogados();
        _audiencias = JuridicoService.GetAudiencias().OrderBy(a => a.DataHora).ToList();
    }

    private Color GetSituacaoColor(string situacao) => situacao switch
    {
        "Em andamento" => Color.Info,
        "Suspenso" => Color.Warning,
        "Encerrado" => Color.Default,
        _ => Color.Default
    };

    private Color GetAudSituacaoColor(string situacao) => situacao switch
    {
        "Agendada" => Color.Primary,
        "Realizada" => Color.Success,
        "Cancelada" => Color.Error,
        _ => Color.Default
    };
}
```

- [ ] **Step 2: Run and verify 3 tabs render with data**

```bash
dotnet run
```

Navigate to `/juridico`. Verify: Processos, Advogados, Audiências tabs all display rows.

- [ ] **Step 3: Commit**

```bash
git add Components/Pages/Juridico/
git commit -m "feat: add Jurídico page with processes, lawyers and hearings tabs"
```

---

### Task 9: Financeiro Page

**Files:**
- Create: `Components/Pages/Financeiro/Financeiro.razor`

- [ ] **Step 1: Create Components/Pages/Financeiro/Financeiro.razor**

```razor
@page "/financeiro"
@inject FinanceiroService FinanceiroService

<PageTitle>Financeiro — SindERP</PageTitle>

<div class="d-flex align-center justify-space-between mb-4">
    <MudText Typo="Typo.h5" Style="font-weight:600;">Financeiro</MudText>
    <MudButton Variant="Variant.Filled" Color="Color.Primary"
               StartIcon="@Icons.Material.Filled.Add">
        Novo Lançamento
    </MudButton>
</div>

<MudCard Elevation="2" Class="rounded-xl">
    <MudCardContent Class="pt-0">
        <MudTabs Elevation="0" PanelClass="pt-3">
            <MudTabPanel Text="Lançamentos">
                <MudTable Items="_lancamentos" Dense="true" Hover="true" Elevation="0">
                    <HeaderContent>
                        <MudTh>Data</MudTh>
                        <MudTh>Descrição</MudTh>
                        <MudTh>Categoria</MudTh>
                        <MudTh>Fornecedor</MudTh>
                        <MudTh>Conta</MudTh>
                        <MudTh>Tipo</MudTh>
                        <MudTh>Valor</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd>@context.Data.ToString("dd/MM/yyyy")</MudTd>
                        <MudTd>@context.Descricao</MudTd>
                        <MudTd>@context.Categoria</MudTd>
                        <MudTd>@context.NomeFornecedor</MudTd>
                        <MudTd><MudText Typo="Typo.caption">@context.ContaBancaria</MudText></MudTd>
                        <MudTd>
                            <MudChip T="string" Size="Size.Small"
                                     Color="@(context.Tipo == TipoLancamento.Entrada ? Color.Success : Color.Error)">
                                @context.Tipo
                            </MudChip>
                        </MudTd>
                        <MudTd>
                            <MudText Style="@($"font-weight:600; color: {(context.Tipo == TipoLancamento.Entrada ? "var(--mud-palette-success)" : "var(--mud-palette-error)")}")">
                                @((context.Tipo == TipoLancamento.Entrada ? "+" : "-") + context.Valor.ToString("C2"))
                            </MudText>
                        </MudTd>
                    </RowTemplate>
                </MudTable>
            </MudTabPanel>

            <MudTabPanel Text="Fluxo de Caixa">
                <MudTable Items="_fluxo" Dense="true" Hover="true" Elevation="0">
                    <HeaderContent>
                        <MudTh>Mês/Ano</MudTh>
                        <MudTh>Entradas</MudTh>
                        <MudTh>Saídas</MudTh>
                        <MudTh>Saldo</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd Style="font-weight:600;">@context.MesAno</MudTd>
                        <MudTd Style="color:var(--mud-palette-success);">+@context.Entradas.ToString("C2")</MudTd>
                        <MudTd Style="color:var(--mud-palette-error);">-@context.Saidas.ToString("C2")</MudTd>
                        <MudTd>
                            <MudText Style="@($"font-weight:700; color: {(context.Saldo >= 0 ? "var(--mud-palette-success)" : "var(--mud-palette-error)")}")">
                                @context.Saldo.ToString("C2")
                            </MudText>
                        </MudTd>
                    </RowTemplate>
                </MudTable>
            </MudTabPanel>

            <MudTabPanel Text="Fornecedores">
                <MudTable Items="_fornecedores" Dense="true" Hover="true" Elevation="0">
                    <HeaderContent>
                        <MudTh>Nome</MudTh>
                        <MudTh>CNPJ</MudTh>
                        <MudTh>Categoria</MudTh>
                        <MudTh>E-mail</MudTh>
                        <MudTh>Telefone</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd Style="font-weight:500;">@context.Nome</MudTd>
                        <MudTd>@context.Cnpj</MudTd>
                        <MudTd><MudChip T="string" Size="Size.Small" Color="Color.Secondary">@context.Categoria</MudChip></MudTd>
                        <MudTd>@context.Email</MudTd>
                        <MudTd>@context.Telefone</MudTd>
                    </RowTemplate>
                </MudTable>
            </MudTabPanel>
        </MudTabs>
    </MudCardContent>
</MudCard>

@code {
    private List<LancamentoFinanceiro> _lancamentos = new();
    private List<Fornecedor> _fornecedores = new();
    private List<FluxoMensal> _fluxo = new();

    protected override void OnInitialized()
    {
        _lancamentos = FinanceiroService.GetLancamentos().OrderByDescending(l => l.Data).ToList();
        _fornecedores = FinanceiroService.GetFornecedores();

        var months = Enumerable.Range(0, 6).Select(i => DateTime.Today.AddMonths(-5 + i)).ToList();
        _fluxo = months.Select(m => new FluxoMensal
        {
            MesAno = m.ToString("MMM/yyyy"),
            Entradas = _lancamentos
                .Where(l => l.Tipo == TipoLancamento.Entrada && l.Data.Month == m.Month && l.Data.Year == m.Year)
                .Sum(l => l.Valor),
            Saidas = _lancamentos
                .Where(l => l.Tipo == TipoLancamento.Saida && l.Data.Month == m.Month && l.Data.Year == m.Year)
                .Sum(l => l.Valor)
        }).Select(f => new FluxoMensal { MesAno = f.MesAno, Entradas = f.Entradas, Saidas = f.Saidas, Saldo = f.Entradas - f.Saidas })
        .ToList();
    }

    private record FluxoMensal
    {
        public string MesAno { get; init; } = "";
        public decimal Entradas { get; init; }
        public decimal Saidas { get; init; }
        public decimal Saldo { get; init; }
    }
}
```

- [ ] **Step 2: Run and verify all 3 tabs display data**

Navigate to `/financeiro`. Verify: lançamentos with color-coded values, fluxo de caixa with saldo, fornecedores table.

- [ ] **Step 3: Commit**

```bash
git add Components/Pages/Financeiro/
git commit -m "feat: add Financeiro page with transactions, cash flow and suppliers"
```

---

### Task 10: Mailing Page

**Files:**
- Create: `Components/Pages/Mailing/Mailing.razor`

- [ ] **Step 1: Create Components/Pages/Mailing/Mailing.razor**

```razor
@page "/mailing"
@inject MailingService MailingService

<PageTitle>Mailing — SindERP</PageTitle>

<div class="d-flex align-center justify-space-between mb-4">
    <MudText Typo="Typo.h5" Style="font-weight:600;">Mailing</MudText>
    <MudButton Variant="Variant.Filled" Color="Color.Primary"
               StartIcon="@Icons.Material.Filled.Add">
        Nova Campanha
    </MudButton>
</div>

<MudGrid Spacing="3">
    @foreach (var campanha in _campanhas)
    {
        <MudItem xs="12" sm="6" md="4">
            <MudCard Elevation="2" Class="rounded-xl" Style="height:100%;">
                <MudCardHeader>
                    <CardHeaderContent>
                        <div class="d-flex align-center justify-space-between">
                            <MudChip T="string" Size="Size.Small"
                                     Color="@GetStatusColor(campanha.Status)"
                                     Variant="Variant.Filled">
                                @campanha.Status
                            </MudChip>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">
                                @(campanha.DataEnvio.HasValue ? campanha.DataEnvio.Value.ToString("dd/MM/yyyy") : "Sem data")
                            </MudText>
                        </div>
                    </CardHeaderContent>
                </MudCardHeader>
                <MudCardContent Class="pt-0">
                    <MudText Typo="Typo.subtitle2" Style="font-weight:600;" Class="mb-1">
                        @campanha.Assunto
                    </MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary" Class="mb-2">
                        @campanha.Destinatarios
                    </MudText>
                    <div class="d-flex align-center gap-1">
                        <MudIcon Icon="@Icons.Material.Filled.People" Size="Size.Small" Color="Color.Secondary" />
                        <MudText Typo="Typo.caption" Color="Color.Secondary">
                            @campanha.TotalDestinatarios.ToString("N0") destinatários
                        </MudText>
                    </div>
                </MudCardContent>
                <MudCardActions>
                    <MudText Typo="Typo.caption" Color="Color.Secondary" Class="ml-2">@campanha.Criador</MudText>
                    <MudSpacer />
                    <MudIconButton Icon="@Icons.Material.Filled.MoreVert" Size="Size.Small" />
                </MudCardActions>
            </MudCard>
        </MudItem>
    }
</MudGrid>

@code {
    private List<Campanha> _campanhas = new();

    protected override void OnInitialized()
    {
        _campanhas = MailingService.GetAll().OrderByDescending(c => c.DataEnvio).ToList();
    }

    private Color GetStatusColor(StatusCampanha status) => status switch
    {
        StatusCampanha.Enviada => Color.Success,
        StatusCampanha.Agendada => Color.Info,
        StatusCampanha.Erro => Color.Error,
        _ => Color.Default
    };
}
```

- [ ] **Step 2: Run and verify card grid renders**

Navigate to `/mailing`. Verify: card grid with status chips, dates, recipient counts.

- [ ] **Step 3: Commit**

```bash
git add Components/Pages/Mailing/
git commit -m "feat: add Mailing page with campaign card grid"
```

---

### Task 11: Entidades Page

**Files:**
- Create: `Components/Pages/Entidades/Entidades.razor`
- Modify: `Components/Layout/MainLayout.razor` (wire up OpenEntityDialog)

- [ ] **Step 1: Create Components/Pages/Entidades/Entidades.razor**

```razor
@page "/entidades"
@inject EntidadeService EntidadeService
@inject AppStateService AppState

<PageTitle>Entidades — SindERP</PageTitle>

<div class="d-flex align-center justify-space-between mb-4">
    <MudText Typo="Typo.h5" Style="font-weight:600;">Entidades</MudText>
</div>

<MudGrid Spacing="3">
    @foreach (var entidade in _entidades)
    {
        <MudItem xs="12" sm="6" md="4">
            <MudCard Elevation="2" Class="rounded-xl cursor-pointer"
                     @onclick="@(() => SelectEntidade(entidade))"
                     Style="@(AppState.EntidadeAtiva?.Id == entidade.Id ? "border: 2px solid var(--mud-palette-primary);" : "")">
                <MudCardContent Class="pa-4">
                    <div class="d-flex align-center gap-3 mb-3">
                        <MudAvatar Color="Color.Primary" Size="Size.Large" Variant="Variant.Filled">
                            @entidade.Sigla[0]
                        </MudAvatar>
                        <div>
                            <MudText Typo="Typo.subtitle1" Style="font-weight:700;">@entidade.Sigla</MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">@entidade.CidadeSede — SP</MudText>
                        </div>
                        @if (AppState.EntidadeAtiva?.Id == entidade.Id)
                        {
                            <MudSpacer />
                            <MudIcon Icon="@Icons.Material.Filled.CheckCircle" Color="Color.Primary" />
                        }
                    </div>
                    <MudText Typo="Typo.body2" Class="mb-2">@entidade.Nome</MudText>
                    <MudText Typo="Typo.caption" Color="Color.Secondary" Class="mb-3">@entidade.Cnpj</MudText>
                    <div class="d-flex align-center gap-1">
                        <MudIcon Icon="@Icons.Material.Filled.Business" Size="Size.Small" Color="Color.Secondary" />
                        <MudText Typo="Typo.caption" Color="Color.Secondary">
                            @entidade.TotalContribuintes.ToString("N0") contribuintes
                        </MudText>
                    </div>
                </MudCardContent>
                <MudCardActions>
                    <MudButton Size="Size.Small" Color="Color.Primary"
                               Variant="@(AppState.EntidadeAtiva?.Id == entidade.Id ? Variant.Filled : Variant.Text)"
                               OnClick="@(() => SelectEntidade(entidade))">
                        @(AppState.EntidadeAtiva?.Id == entidade.Id ? "Ativa" : "Selecionar")
                    </MudButton>
                </MudCardActions>
            </MudCard>
        </MudItem>
    }
</MudGrid>

@code {
    private List<Entidade> _entidades = new();

    protected override void OnInitialized()
    {
        _entidades = EntidadeService.GetAll();
        if (AppState.EntidadeAtiva is null && _entidades.Count > 0)
            AppState.SetEntidade(_entidades[0]);
    }

    private void SelectEntidade(Entidade entidade)
    {
        AppState.SetEntidade(entidade);
    }
}
```

- [ ] **Step 2: Initialize the active entity on app startup**

Open `Components/App.razor` and add an `OnInitialized` that sets the first entity. Since `App.razor` doesn't inject services easily, the initialization in `Entidades.razor` above already handles it. No further change needed.

- [ ] **Step 3: Run and verify entity selection updates the topbar chip**

Navigate to `/entidades`. Click different entity cards. Verify the "SINDHOSP" / "SINDBAR" chip in the topbar updates.

- [ ] **Step 4: Commit**

```bash
git add Components/Pages/Entidades/
git commit -m "feat: add Entidades page with entity selection"
```

---

### Task 12: Usuários Page

**Files:**
- Create: `Components/Pages/Usuarios/Usuarios.razor`

- [ ] **Step 1: Create Components/Pages/Usuarios/Usuarios.razor**

```razor
@page "/usuarios"
@inject UsuarioService UsuarioService

<PageTitle>Usuários — SindERP</PageTitle>

<div class="d-flex align-center justify-space-between mb-4">
    <MudText Typo="Typo.h5" Style="font-weight:600;">Usuários</MudText>
    <MudButton Variant="Variant.Filled" Color="Color.Primary"
               StartIcon="@Icons.Material.Filled.PersonAdd">
        Novo Usuário
    </MudButton>
</div>

<MudCard Elevation="2" Class="rounded-xl">
    <MudCardContent Class="pt-2">
        <MudTable Items="_usuarios" Dense="false" Hover="true" Elevation="0">
            <HeaderContent>
                <MudTh>Usuário</MudTh>
                <MudTh>E-mail</MudTh>
                <MudTh>Entidades e Módulos</MudTh>
                <MudTh>Último Acesso</MudTh>
                <MudTh></MudTh>
            </HeaderContent>
            <RowTemplate>
                <MudTd>
                    <div class="d-flex align-center gap-3">
                        <MudAvatar Color="Color.Primary" Size="Size.Medium">
                            @string.Concat(context.Nome.Split(' ').Take(2).Select(p => p[0]))
                        </MudAvatar>
                        <MudText Typo="Typo.body2" Style="font-weight:500;">@context.Nome</MudText>
                    </div>
                </MudTd>
                <MudTd>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">@context.Email</MudText>
                </MudTd>
                <MudTd>
                    <div class="d-flex flex-wrap gap-1">
                        @foreach (var perm in context.Permissoes)
                        {
                            <MudTooltip Text="@string.Join(", ", perm.Modulos)">
                                <MudChip T="string" Size="Size.Small" Color="Color.Primary" Variant="Variant.Outlined">
                                    @perm.NomeEntidade
                                </MudChip>
                            </MudTooltip>
                        }
                    </div>
                </MudTd>
                <MudTd>
                    <MudText Typo="Typo.caption" Color="Color.Secondary">
                        @GetUltimoAcessoLabel(context.UltimoAcesso)
                    </MudText>
                </MudTd>
                <MudTd>
                    <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small" />
                </MudTd>
            </RowTemplate>
        </MudTable>
    </MudCardContent>
</MudCard>

@code {
    private List<Usuario> _usuarios = new();

    protected override void OnInitialized()
    {
        _usuarios = UsuarioService.GetAll();
    }

    private static string GetUltimoAcessoLabel(DateTime dt)
    {
        var diff = DateTime.Now - dt;
        return diff.TotalMinutes < 60
            ? $"há {(int)diff.TotalMinutes} min"
            : diff.TotalHours < 24
                ? $"há {(int)diff.TotalHours}h"
                : $"há {(int)diff.TotalDays} dias";
    }
}
```

- [ ] **Step 2: Run and verify full app navigation**

```bash
dotnet run
```

Navigate through all 8 routes. Verify:
- `/` — dashboard with KPIs, chart, table
- `/contribuintes` — DataGrid, row click opens drawer with tabs
- `/cobrancas` — tabs with colored status chips, dialog on click
- `/juridico` — 3 tabs with data
- `/financeiro` — 3 tabs including fluxo de caixa
- `/mailing` — card grid
- `/entidades` — entity cards, selection updates topbar
- `/usuarios` — table with entity chips and tooltips showing modules
- Dark mode toggle works
- Sidebar collapses on mobile

- [ ] **Step 3: Commit**

```bash
git add Components/Pages/Usuarios/
git commit -m "feat: add Usuários page — completes all 8 ERP modules"
```

---

## Post-Build Checklist

- [ ] All 8 routes navigate without errors
- [ ] Dark mode toggle in topbar works
- [ ] Entity selection on `/entidades` updates topbar chip
- [ ] Drawer on `/contribuintes` opens on row click with 4 tabs
- [ ] Dialog on `/cobrancas` shows linha digitável
- [ ] `/financeiro` Fluxo de Caixa shows 6 months with correct saldo colors
- [ ] All status chips use correct colors throughout
- [ ] Mobile: sidebar becomes drawer with hamburger toggle
