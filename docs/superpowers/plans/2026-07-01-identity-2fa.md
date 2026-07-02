# Migração pra ASP.NET Core Identity + 2FA (TOTP) — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Substituir a autenticação custom (`Usuario`/`AuthAppService`/`IPasswordHasher`) por ASP.NET Core Identity (`ApplicationUser : IdentityUser<Guid>`, `UserManager`), e adicionar 2FA via TOTP (app autenticador) no login.

**Architecture:** `ApplicationUser` (Domain) substitui `Usuario`; `AppDbContext` (Infrastructure) vira `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>`; `AuthAppService` (Application) usa `UserManager<ApplicationUser>` pra senha/lockout/2FA e continua emitindo JWT manualmente (igual hoje); passo intermediário de login com 2FA usa token opaco em `IMemoryCache` (não-JWT, não aceito por `[Authorize]`); setup de 2FA fica numa nova página "Minha Conta" no frontend, gerando QR via `QRCoder`.

**Tech Stack:** .NET 10 / ASP.NET Core Web API, EF Core 10 + SQL Server LocalDB, Microsoft.Extensions.Identity.Core/Stores + Microsoft.AspNetCore.Identity.EntityFrameworkCore, QRCoder, Blazor WebAssembly + MudBlazor no frontend.

## Global Constraints

- `TargetFramework` net10.0 em todos os projetos backend — não alterar.
- Versões de pacote seguem o padrão já usado no repo: `Microsoft.EntityFrameworkCore.*`/`Microsoft.Extensions.*` pinados em `10.0.9`, pacotes de auth (`Microsoft.AspNetCore.Authentication.JwtBearer`) com `10.*`. Pacotes Identity novos usam `10.*` (mesma família de versionamento do ASP.NET Core).
- Sem projeto de teste automatizado no repo — verificação é build + execução manual (`dotnet build`, `dotnet run`, `curl`, browser).
- Reset geral de senha dos usuários seed (`Senha@123`) — sem lógica de compatibilidade com hash antigo.
- Migration EF `Initial` é substituída (não incremental) — DB local será dropado e recriado.
- Fora de escopo: roles/claims do Identity, 2FA por email/SMS, recovery codes, mudanças nos 13 controllers de domínio além de `AuthController`/`UsuariosController`.

Referência: `docs/superpowers/specs/2026-07-01-identity-2fa-design.md`.

---

## Task 1: Pacotes NuGet (Identity, QRCoder, MemoryCache)

**Files:**
- Modify: `src/backend/BlazorTeste.Domain/BlazorTeste.Domain.csproj`
- Modify: `src/backend/BlazorTeste.Infrastructure/BlazorTeste.Infrastructure.csproj`
- Modify: `src/backend/BlazorTeste.Application/BlazorTeste.Application.csproj`

**Interfaces:**
- Produces: `IdentityUser<Guid>`/`IdentityRole<Guid>` disponíveis em Domain; `IdentityDbContext<,,>`/`AddEntityFrameworkStores` disponíveis em Infrastructure; `UserManager<T>`/`IMemoryCache`/`QRCodeGenerator` disponíveis em Application.

- [ ] **Step 1: Editar `BlazorTeste.Domain.csproj`**

Arquivo atual:
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

Substituir por:
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Identity.Stores" Version="10.*" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

- [ ] **Step 2: Editar `BlazorTeste.Infrastructure.csproj`**

No `ItemGroup` que já tem `Microsoft.EntityFrameworkCore.Design`/`Microsoft.EntityFrameworkCore.SqlServer`/`Microsoft.Extensions.DependencyInjection.Abstractions`, adicionar:
```xml
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.*" />
```

Resultado (`ItemGroup` completo):
```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.*" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.9">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.9" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.9" />
  </ItemGroup>
```

- [ ] **Step 3: Editar `BlazorTeste.Application.csproj`**

No `ItemGroup` que já tem `Microsoft.Extensions.Configuration.Abstractions`/`System.IdentityModel.Tokens.Jwt`/`FluentValidation`, adicionar:
```xml
    <PackageReference Include="Microsoft.Extensions.Identity.Core" Version="10.*" />
    <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="10.*" />
    <PackageReference Include="QRCoder" Version="1.*" />
```

Resultado (`ItemGroup` completo):
```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="10.0.9" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.*" />
    <PackageReference Include="FluentValidation" Version="11.*" />
    <PackageReference Include="Microsoft.Extensions.Identity.Core" Version="10.*" />
    <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="10.*" />
    <PackageReference Include="QRCoder" Version="1.*" />
  </ItemGroup>
```

- [ ] **Step 4: Restaurar pacotes**

Run: `dotnet restore` (a partir de `src/backend`)
Expected: restaura sem erro, resolve os 4 pacotes novos.

- [ ] **Step 5: Commit**

```bash
git add src/backend/BlazorTeste.Domain/BlazorTeste.Domain.csproj src/backend/BlazorTeste.Infrastructure/BlazorTeste.Infrastructure.csproj src/backend/BlazorTeste.Application/BlazorTeste.Application.csproj
git commit -m "build(backend): add ASP.NET Core Identity, QRCoder and MemoryCache packages"
```

---

## Task 2: `ApplicationUser` substitui `Usuario` (Domain)

**Files:**
- Delete: `src/backend/BlazorTeste.Domain/Entities/Usuario.cs`
- Create: `src/backend/BlazorTeste.Domain/Entities/ApplicationUser.cs`

**Interfaces:**
- Consumes: `Microsoft.AspNetCore.Identity.IdentityUser<Guid>` (do pacote `Microsoft.Extensions.Identity.Stores` adicionado na Task 1).
- Produces: `ApplicationUser` (com `Id: Guid` herdado, `Email`/`PasswordHash`/`TwoFactorEnabled`/`LockoutEnd`/`AccessFailedCount` herdados de `IdentityUser<Guid>`, mais `Nome`, `UltimoAcesso`, `RefreshToken`, `RefreshTokenExpiry`, `Permissoes`) e `PermissaoEntidade` (inalterado), usados por todas as tasks seguintes.

- [ ] **Step 1: Deletar `Usuario.cs` e criar `ApplicationUser.cs`**

Deletar `src/backend/BlazorTeste.Domain/Entities/Usuario.cs`.

Criar `src/backend/BlazorTeste.Domain/Entities/ApplicationUser.cs`:
```csharp
using Microsoft.AspNetCore.Identity;

namespace BlazorTeste.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Nome { get; set; } = "";
    public DateTime UltimoAcesso { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public List<PermissaoEntidade> Permissoes { get; set; } = new();
}

public class PermissaoEntidade
{
    public int EntidadeId { get; set; }
    public string NomeEntidade { get; set; } = "";
    public List<string> Modulos { get; set; } = new();
}
```

- [ ] **Step 2: Confirmar que só o Domain compila isolado (dependências circulares checadas depois)**

Run: `dotnet build BlazorTeste.Domain/BlazorTeste.Domain.csproj` (a partir de `src/backend`)
Expected: FAIL nesse ponto — outros projetos (`Application`, `Infrastructure`, `Api`) ainda referenciam `Usuario`/`IUsuarioRepository`/`IPasswordHasher`. Isso é esperado; essas referências são corrigidas nas próximas tasks. Não commitar ainda um estado que não compila globalmente — a Task 4 é o primeiro ponto de checkpoint de build completo.

- [ ] **Step 3: Commit**

```bash
git add src/backend/BlazorTeste.Domain/Entities
git commit -m "refactor(domain): replace Usuario with ApplicationUser : IdentityUser<Guid>"
```

---

## Task 3: `AppDbContext` vira `IdentityDbContext` + configuração EF

**Files:**
- Modify: `src/backend/BlazorTeste.Infrastructure/Data/AppDbContext.cs`
- Delete: `src/backend/BlazorTeste.Infrastructure/Data/Configurations/UsuarioConfiguration.cs`
- Create: `src/backend/BlazorTeste.Infrastructure/Data/Configurations/ApplicationUserConfiguration.cs`

**Interfaces:**
- Consumes: `ApplicationUser`, `PermissaoEntidade` (Task 2).
- Produces: `AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>` com `Permissoes` mapeado como coluna JSON (igual ao comportamento anterior de `Usuario.Permissoes`).

- [ ] **Step 1: Reescrever `AppDbContext.cs`**

Arquivo atual (`src/backend/BlazorTeste.Infrastructure/Data/AppDbContext.cs`):
```csharp
using BlazorTeste.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Entidade> Entidades => Set<Entidade>();
    public DbSet<Contribuinte> Contribuintes => Set<Contribuinte>();
    public DbSet<Cobranca> Cobrancas => Set<Cobranca>();
    public DbSet<LancamentoFinanceiro> LancamentosFinanceiros => Set<LancamentoFinanceiro>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Processo> Processos => Set<Processo>();
    public DbSet<Advogado> Advogados => Set<Advogado>();
    public DbSet<Audiencia> Audiencias => Set<Audiencia>();
    public DbSet<Campanha> Campanhas => Set<Campanha>();
    public DbSet<GuiaSindical> GuiaSindicais => Set<GuiaSindical>();
    public DbSet<RegistroBaixa> RegistrosBaixa => Set<RegistroBaixa>();
    public DbSet<Relatorio> Relatorios => Set<Relatorio>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Negociacao> Negociacoes => Set<Negociacao>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<ConfiguracaoEntidade> Configuracoes => Set<ConfiguracaoEntidade>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

Substituir por:
```csharp
using BlazorTeste.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Entidade> Entidades => Set<Entidade>();
    public DbSet<Contribuinte> Contribuintes => Set<Contribuinte>();
    public DbSet<Cobranca> Cobrancas => Set<Cobranca>();
    public DbSet<LancamentoFinanceiro> LancamentosFinanceiros => Set<LancamentoFinanceiro>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Processo> Processos => Set<Processo>();
    public DbSet<Advogado> Advogados => Set<Advogado>();
    public DbSet<Audiencia> Audiencias => Set<Audiencia>();
    public DbSet<Campanha> Campanhas => Set<Campanha>();
    public DbSet<GuiaSindical> GuiaSindicais => Set<GuiaSindical>();
    public DbSet<RegistroBaixa> RegistrosBaixa => Set<RegistroBaixa>();
    public DbSet<Relatorio> Relatorios => Set<Relatorio>();
    public DbSet<Negociacao> Negociacoes => Set<Negociacao>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<ConfiguracaoEntidade> Configuracoes => Set<ConfiguracaoEntidade>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

(`DbSet<ApplicationUser> Users` já vem herdado de `IdentityDbContext` — não precisa redeclarar.)

- [ ] **Step 2: Deletar `UsuarioConfiguration.cs` e criar `ApplicationUserConfiguration.cs`**

Deletar `src/backend/BlazorTeste.Infrastructure/Data/Configurations/UsuarioConfiguration.cs`.

Criar `src/backend/BlazorTeste.Infrastructure/Data/Configurations/ApplicationUserConfiguration.cs`:
```csharp
using System.Text.Json;
using BlazorTeste.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorTeste.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    private static readonly JsonSerializerOptions _json = new();
    private static readonly ValueComparer<List<PermissaoEntidade>> _comparer = new(
        (a, b) => JsonSerializer.Serialize(a, _json) == JsonSerializer.Serialize(b, _json),
        v => JsonSerializer.Serialize(v, _json).GetHashCode(),
        v => JsonSerializer.Deserialize<List<PermissaoEntidade>>(JsonSerializer.Serialize(v, _json), _json)!);

    public void Configure(EntityTypeBuilder<ApplicationUser> b)
    {
        b.Property(u => u.Permissoes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                v => JsonSerializer.Deserialize<List<PermissaoEntidade>>(v, _json) ?? new())
            .Metadata.SetValueComparer(_comparer);
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add src/backend/BlazorTeste.Infrastructure/Data/AppDbContext.cs src/backend/BlazorTeste.Infrastructure/Data/Configurations
git commit -m "refactor(infrastructure): AppDbContext extends IdentityDbContext<ApplicationUser>"
```

---

## Task 4: Remover repositório/hasher custom de Usuario

**Files:**
- Delete: `src/backend/BlazorTeste.Domain/Interfaces/Repositories/IUsuarioRepository.cs`
- Delete: `src/backend/BlazorTeste.Infrastructure/Repositories/UsuarioRepository.cs`
- Delete: `src/backend/BlazorTeste.Application/Security/IPasswordHasher.cs`
- Delete: `src/backend/BlazorTeste.Infrastructure/Security/PasswordHasher.cs`
- Delete: `src/backend/BlazorTeste.Infrastructure/Security/PasswordHelper.cs`
- Modify: `src/backend/BlazorTeste.Infrastructure/DependencyInjection.cs`

**Interfaces:**
- Produces: `AddInfrastructure` sem `IUsuarioRepository`/`IPasswordHasher` — autenticação de usuário passa a ser 100% via `UserManager<ApplicationUser>` (registrado na Task 9).

- [ ] **Step 1: Deletar os 5 arquivos**

```bash
git rm src/backend/BlazorTeste.Domain/Interfaces/Repositories/IUsuarioRepository.cs \
       src/backend/BlazorTeste.Infrastructure/Repositories/UsuarioRepository.cs \
       src/backend/BlazorTeste.Application/Security/IPasswordHasher.cs \
       src/backend/BlazorTeste.Infrastructure/Security/PasswordHasher.cs \
       src/backend/BlazorTeste.Infrastructure/Security/PasswordHelper.cs
```

- [ ] **Step 2: Editar `DependencyInjection.cs`**

Arquivo atual:
```csharp
using BlazorTeste.Application.Security;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using BlazorTeste.Infrastructure.Repositories;
using BlazorTeste.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTeste.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

        services.AddScoped<IContribuinteRepository, ContribuinteRepository>();
        services.AddScoped<ICobrancaRepository, CobrancaRepository>();
        services.AddScoped<IJuridicoRepository, JuridicoRepository>();
        services.AddScoped<IFinanceiroRepository, FinanceiroRepository>();
        services.AddScoped<IEventoRepository, EventoRepository>();
        services.AddScoped<IGuiaSindicalRepository, GuiaSindicalRepository>();
        services.AddScoped<INegociacaoRepository, NegociacaoRepository>();
        services.AddScoped<IBaixaCobrancaRepository, BaixaCobrancaRepository>();
        services.AddScoped<IEntidadeRepository, EntidadeRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICampanhaRepository, CampanhaRepository>();
        services.AddScoped<IConfiguracaoRepository, ConfiguracaoRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
```

Substituir por:
```csharp
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using BlazorTeste.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTeste.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

        services.AddScoped<IContribuinteRepository, ContribuinteRepository>();
        services.AddScoped<ICobrancaRepository, CobrancaRepository>();
        services.AddScoped<IJuridicoRepository, JuridicoRepository>();
        services.AddScoped<IFinanceiroRepository, FinanceiroRepository>();
        services.AddScoped<IEventoRepository, EventoRepository>();
        services.AddScoped<IGuiaSindicalRepository, GuiaSindicalRepository>();
        services.AddScoped<INegociacaoRepository, NegociacaoRepository>();
        services.AddScoped<IBaixaCobrancaRepository, BaixaCobrancaRepository>();
        services.AddScoped<IEntidadeRepository, EntidadeRepository>();
        services.AddScoped<ICampanhaRepository, CampanhaRepository>();
        services.AddScoped<IConfiguracaoRepository, ConfiguracaoRepository>();

        return services;
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add -A src/backend/BlazorTeste.Domain/Interfaces/Repositories src/backend/BlazorTeste.Infrastructure src/backend/BlazorTeste.Application/Security
git commit -m "refactor(infrastructure): remove custom Usuario repository and password hasher"
```

(O build global ainda vai falhar até a Task 8 — `AuthAppService`/`UsuarioAppService` continuam referenciando os tipos deletados. Isso é esperado; primeiro checkpoint de build completo é no fim da Task 9.)

---

## Task 5: Migration EF `Initial` (schema Identity)

**Files:**
- Delete: `src/backend/BlazorTeste.Infrastructure/Data/Migrations/20260630120620_Initial.cs`
- Delete: `src/backend/BlazorTeste.Infrastructure/Data/Migrations/20260630120620_Initial.Designer.cs`
- Delete: `src/backend/BlazorTeste.Infrastructure/Data/Migrations/AppDbContextModelSnapshot.cs`
- Create: nova migration `Initial` (gerada pelo `dotnet ef`)

**Interfaces:**
- Consumes: `AppDbContext` compilável — **esta task só pode rodar depois que Tasks 6-9 deixarem o backend inteiro compilando** (o `dotnet ef migrations add` precisa buildar o projeto Api). Ordem de execução real: trate esta task como o último passo antes da Task 12 (build/smoke test) — implemente as Tasks 6, 7, 8, 9 e 10 antes de rodar os comandos `dotnet ef` desta task.

- [ ] **Step 1: Deletar os 3 arquivos de migration antigos**

```bash
git rm src/backend/BlazorTeste.Infrastructure/Data/Migrations/20260630120620_Initial.cs \
       src/backend/BlazorTeste.Infrastructure/Data/Migrations/20260630120620_Initial.Designer.cs \
       src/backend/BlazorTeste.Infrastructure/Data/Migrations/AppDbContextModelSnapshot.cs
```

- [ ] **Step 2: Confirmar que o backend compila (pré-requisito antes de gerar migration)**

Run (a partir de `src/backend`): `dotnet build`
Expected: `Build succeeded.` — se falhar, pare aqui e complete as Tasks 6-10 primeiro (elas ajustam `AuthAppService`, `UsuarioAppService`, `Program.cs`, `SeedData.cs`, `AuthController` pra usar `ApplicationUser`/`UserManager`).

- [ ] **Step 3: Dropar o banco local e gerar a nova migration**

Run (a partir de `src/backend`):
```bash
dotnet ef database drop --project BlazorTeste.Infrastructure --startup-project BlazorTeste.Api --force
dotnet ef migrations add Initial --project BlazorTeste.Infrastructure --startup-project BlazorTeste.Api --output-dir Data/Migrations
dotnet ef database update --project BlazorTeste.Infrastructure --startup-project BlazorTeste.Api
```
Expected: banco `SindERP` (LocalDB) recriado com tabelas `AspNetUsers`, `AspNetRoles`, `AspNetUserTokens` etc. junto com todas as tabelas de domínio existentes (`Entidades`, `Contribuintes`, ...). `dotnet ef migrations add` deve gerar só uma migration `Initial` (sem erros de "pending model changes").

- [ ] **Step 4: Commit**

```bash
git add src/backend/BlazorTeste.Infrastructure/Data/Migrations
git commit -m "chore(infrastructure): regenerate Initial migration with Identity schema"
```

---

## Task 6: DTOs novos/atualizados (Application)

**Files:**
- Create: `src/backend/BlazorTeste.Application/DTOs/LoginResultDto.cs`
- Create: `src/backend/BlazorTeste.Application/DTOs/TwoFactorSetupDto.cs`
- Modify: `src/backend/BlazorTeste.Application/DTOs/UsuarioDto.cs`

**Interfaces:**
- Produces: `LoginResultDto { bool RequiresTwoFactor, string? MfaToken, AuthResultDto? Auth }`, `TwoFactorSetupDto { string SharedKey, string QrCodePngBase64 }`, `UsuarioDto.Id: Guid` — usados por `IAuthAppService`/`AuthAppService` (Task 7) e `AuthController` (Task 11).

- [ ] **Step 1: Criar `LoginResultDto.cs`**

```csharp
namespace BlazorTeste.Application.DTOs;

public class LoginResultDto
{
    public bool RequiresTwoFactor { get; set; }
    public string? MfaToken { get; set; }
    public AuthResultDto? Auth { get; set; }
}
```

- [ ] **Step 2: Criar `TwoFactorSetupDto.cs`**

```csharp
namespace BlazorTeste.Application.DTOs;

public class TwoFactorSetupDto
{
    public string SharedKey { get; set; } = "";
    public string QrCodePngBase64 { get; set; } = "";
}
```

- [ ] **Step 3: Editar `UsuarioDto.cs`**

Arquivo atual:
```csharp
namespace BlazorTeste.Application.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime UltimoAcesso { get; set; }
    public List<PermissaoEntidadeDto> Permissoes { get; set; } = new();
}
```

Substituir por:
```csharp
namespace BlazorTeste.Application.DTOs;

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime UltimoAcesso { get; set; }
    public List<PermissaoEntidadeDto> Permissoes { get; set; } = new();
}
```

- [ ] **Step 4: Commit**

```bash
git add src/backend/BlazorTeste.Application/DTOs
git commit -m "feat(application): add 2FA DTOs, UsuarioDto.Id becomes Guid"
```

---

## Task 7: `AuthAppService` reescrito com `UserManager` + 2FA

**Files:**
- Modify: `src/backend/BlazorTeste.Application/Services/Interfaces/IAuthAppService.cs`
- Modify: `src/backend/BlazorTeste.Application/Services/Implementations/AuthAppService.cs`

**Interfaces:**
- Consumes: `ApplicationUser` (Task 2), `LoginResultDto`/`TwoFactorSetupDto`/`AuthResultDto` (Task 6), `UserManager<ApplicationUser>` (registrado na Task 9), `IMemoryCache` (registrado na Task 9), `IConfiguration` (`Jwt:Key`/`Jwt:Issuer`/`Jwt:Audience`/`Jwt:AccessTokenExpiryMinutes`/`Jwt:RefreshTokenExpiryDays`, já existentes em `appsettings.json`).
- Produces: `IAuthAppService.LoginAsync(string,string): Task<LoginResultDto?>`, `.VerifyTwoFactorAsync(string,string): Task<AuthResultDto?>`, `.RefreshAsync(string): Task<AuthResultDto?>`, `.LogoutAsync(string): Task`, `.SetupTwoFactorAsync(Guid): Task<TwoFactorSetupDto>`, `.EnableTwoFactorAsync(Guid,string): Task<bool>`, `.DisableTwoFactorAsync(Guid,string): Task<bool>` — consumidos por `AuthController` (Task 11).

- [ ] **Step 1: Reescrever `IAuthAppService.cs`**

```csharp
using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IAuthAppService
{
    Task<LoginResultDto?> LoginAsync(string email, string senha);
    Task<AuthResultDto?> VerifyTwoFactorAsync(string mfaToken, string code);
    Task<AuthResultDto?> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
    Task<TwoFactorSetupDto> SetupTwoFactorAsync(Guid userId);
    Task<bool> EnableTwoFactorAsync(Guid userId, string code);
    Task<bool> DisableTwoFactorAsync(Guid userId, string code);
}
```

- [ ] **Step 2: Reescrever `AuthAppService.cs`**

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QRCoder;

namespace BlazorTeste.Application.Services.Implementations;

public class AuthAppService(
    UserManager<ApplicationUser> userManager,
    IMemoryCache cache,
    IConfiguration config) : IAuthAppService
{
    private static readonly TimeSpan MfaTokenLifetime = TimeSpan.FromMinutes(5);

    public async Task<LoginResultDto?> LoginAsync(string email, string senha)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) return null;

        if (await userManager.IsLockedOutAsync(user)) return null;

        if (!await userManager.CheckPasswordAsync(user, senha))
        {
            await userManager.AccessFailedAsync(user);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(user);
        user.UltimoAcesso = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        if (user.TwoFactorEnabled)
        {
            var mfaToken = Guid.NewGuid().ToString("N");
            cache.Set($"mfa:{mfaToken}", user.Id, MfaTokenLifetime);
            return new LoginResultDto { RequiresTwoFactor = true, MfaToken = mfaToken };
        }

        return new LoginResultDto { Auth = await IssueTokensAsync(user) };
    }

    public async Task<AuthResultDto?> VerifyTwoFactorAsync(string mfaToken, string code)
    {
        if (!cache.TryGetValue($"mfa:{mfaToken}", out Guid userId)) return null;

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return null;

        if (await userManager.IsLockedOutAsync(user)) return null;

        var valid = await userManager.VerifyTwoFactorTokenAsync(
            user, TokenOptions.DefaultAuthenticatorProvider, code);

        if (!valid)
        {
            await userManager.AccessFailedAsync(user);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(user);
        cache.Remove($"mfa:{mfaToken}");
        return await IssueTokensAsync(user);
    }

    public async Task<AuthResultDto?> RefreshAsync(string refreshToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user is null || user.RefreshTokenExpiry <= DateTime.UtcNow) return null;
        return await IssueTokensAsync(user);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user is null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await userManager.UpdateAsync(user);
    }

    public async Task<TwoFactorSetupDto> SetupTwoFactorAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        await userManager.ResetAuthenticatorKeyAsync(user);
        var key = await userManager.GetAuthenticatorKeyAsync(user)
            ?? throw new InvalidOperationException("Falha ao gerar chave do autenticador.");

        var uri = $"otpauth://totp/SindERP:{Uri.EscapeDataString(user.Email!)}" +
                  $"?secret={key}&issuer=SindERP&digits=6";

        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        var png = qrCode.GetGraphic(10);

        return new TwoFactorSetupDto
        {
            SharedKey = key,
            QrCodePngBase64 = Convert.ToBase64String(png)
        };
    }

    public async Task<bool> EnableTwoFactorAsync(Guid userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return false;

        var valid = await userManager.VerifyTwoFactorTokenAsync(
            user, TokenOptions.DefaultAuthenticatorProvider, code);
        if (!valid) return false;

        await userManager.SetTwoFactorEnabledAsync(user, true);
        return true;
    }

    public async Task<bool> DisableTwoFactorAsync(Guid userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return false;

        var valid = await userManager.VerifyTwoFactorTokenAsync(
            user, TokenOptions.DefaultAuthenticatorProvider, code);
        if (!valid) return false;

        await userManager.SetTwoFactorEnabledAsync(user, false);
        return true;
    }

    private async Task<AuthResultDto> IssueTokensAsync(ApplicationUser user)
    {
        var (refreshToken, expiry) = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = expiry;
        await userManager.UpdateAsync(user);

        return new AuthResultDto
        {
            AccessToken = GenerateAccessToken(user),
            Nome = user.Nome,
            Email = user.Email!,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = expiry
        };
    }

    private (string token, DateTime expiry) GenerateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiry = DateTime.UtcNow.AddDays(int.Parse(config["Jwt:RefreshTokenExpiryDays"]!));
        return (token, expiry);
    }

    private string GenerateAccessToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("name", user.Nome),
            new Claim("twoFactorEnabled", user.TwoFactorEnabled ? "true" : "false"),
            new Claim("permissoes", JsonSerializer.Serialize(user.Permissoes))
        };
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(config["Jwt:AccessTokenExpiryMinutes"]!)),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add src/backend/BlazorTeste.Application/Services
git commit -m "feat(application): AuthAppService uses UserManager, adds TOTP 2FA flow"
```

---

## Task 8: `UsuarioAppService` usa `UserManager`

**Files:**
- Modify: `src/backend/BlazorTeste.Application/Services/Implementations/UsuarioAppService.cs`

**Interfaces:**
- Consumes: `UserManager<ApplicationUser>`, `UsuarioDto` (Task 6).
- Produces: `IUsuarioAppService.GetAllAsync()`/`.GetByEmailAsync(string)` sem mudança de assinatura — consumidos por `UsuariosController` (inalterado).

- [ ] **Step 1: Reescrever `UsuarioAppService.cs`**

```csharp
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Application.Services.Implementations;

public class UsuarioAppService(UserManager<ApplicationUser> userManager) : IUsuarioAppService
{
    public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
    {
        var items = await userManager.Users.ToListAsync();
        return items.Select(Map);
    }

    public async Task<UsuarioDto?> GetByEmailAsync(string email)
    {
        var u = await userManager.FindByEmailAsync(email);
        return u is null ? null : Map(u);
    }

    private static UsuarioDto Map(ApplicationUser u) => new()
    {
        Id = u.Id,
        Nome = u.Nome,
        Email = u.Email!,
        UltimoAcesso = u.UltimoAcesso,
        Permissoes = u.Permissoes.Select(p => new PermissaoEntidadeDto
        {
            EntidadeId = p.EntidadeId,
            NomeEntidade = p.NomeEntidade,
            Modulos = p.Modulos
        }).ToList()
    };
}
```

- [ ] **Step 2: Commit**

```bash
git add src/backend/BlazorTeste.Application/Services/Implementations/UsuarioAppService.cs
git commit -m "refactor(application): UsuarioAppService reads from UserManager"
```

---

## Task 9: `Program.cs` registra Identity + MemoryCache

**Files:**
- Modify: `src/backend/BlazorTeste.Api/Program.cs`

**Interfaces:**
- Consumes: `ApplicationUser` (Task 2), `AppDbContext` (Task 3).
- Produces: `UserManager<ApplicationUser>`/`IMemoryCache` disponíveis via DI pra `AuthAppService`/`UsuarioAppService`/`AuthController`; `SeedData.InitializeAsync` chamado no startup (implementado na Task 10).

- [ ] **Step 1: Editar usings e registro de serviços**

Trecho atual (topo do arquivo):
```csharp
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorTeste.Api.Filters;
using BlazorTeste.Api.Validators;
using BlazorTeste.Application.Security;
using BlazorTeste.Application.Services.Implementations;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Application.Validators;
using BlazorTeste.Infrastructure;
using BlazorTeste.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
```

Substituir por:
```csharp
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorTeste.Api.Filters;
using BlazorTeste.Api.Validators;
using BlazorTeste.Application.Services.Implementations;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Application.Validators;
using BlazorTeste.Domain.Entities;
using BlazorTeste.Infrastructure;
using BlazorTeste.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
```

Trecho atual (registro de auth):
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
```

Substituir por:
```csharp
builder.Services.AddIdentityCore<ApplicationUser>(opt =>
    {
        opt.Password.RequiredLength = 8;
        opt.Lockout.MaxFailedAccessAttempts = 5;
        opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        opt.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
```

- [ ] **Step 2: Editar bloco de seed pra usar `UserManager` e `await`**

Trecho atual:
```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    SeedData.Initialize(db, hasher);
}
```

Substituir por:
```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await SeedData.InitializeAsync(db, userManager);
}
```

- [ ] **Step 3: Build completo do backend**

Run (a partir de `src/backend`): `dotnet build`
Expected: FAIL ainda — `SeedData.cs` (Task 10) e `AuthController`/`Api/Models/Auth.cs` (Task 11) precisam ser atualizados antes do build passar. Só confirme aqui que os erros restantes são exclusivamente nesses dois arquivos (nenhum erro sobrando em `Program.cs`, `AuthAppService`, `UsuarioAppService`, `AppDbContext`).

- [ ] **Step 4: Commit**

```bash
git add src/backend/BlazorTeste.Api/Program.cs
git commit -m "feat(api): register ASP.NET Core Identity and IMemoryCache"
```

---

## Task 10: `SeedData.cs` — usuários seed via `UserManager`

**Files:**
- Modify: `src/backend/BlazorTeste.Infrastructure/Data/SeedData.cs`

**Interfaces:**
- Consumes: `UserManager<ApplicationUser>` (Task 9), `ApplicationUser`/`PermissaoEntidade` (Task 2).
- Produces: `SeedData.InitializeAsync(AppDbContext, UserManager<ApplicationUser>): Task` — chamado por `Program.cs` (Task 9).

- [ ] **Step 1: Editar assinatura e usings**

Trecho atual (topo do arquivo, linhas 1-9):
```csharp
using BlazorTeste.Application.Security;
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Infrastructure.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db, IPasswordHasher passwordHasher)
    {
```

Substituir por:
```csharp
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace BlazorTeste.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
```

- [ ] **Step 2: Substituir a criação de usuários seed**

Trecho atual (dentro do `if (!db.Entidades.Any())`, logo depois do bloco `db.Campanhas.AddRange(...)`):
```csharp
            var defaultHash = passwordHasher.Hash("Senha@123");
            db.Usuarios.AddRange(
                new Usuario { Nome = "Ana Lima", Email = "ana.lima@sindhosp.org.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddMinutes(-15), Permissoes = new() { new() { EntidadeId = entidades[0].Id, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing" } }, new() { EntidadeId = entidades[4].Id, NomeEntidade = "FETURH", Modulos = new() { "Contribuintes" } } } },
                new Usuario { Nome = "Carlos Silva", Email = "carlos.silva@sindhosp.org.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddHours(-2), Permissoes = new() { new() { EntidadeId = entidades[0].Id, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Mailing" } } } },
                new Usuario { Nome = "Fernanda Costa", Email = "fernanda.costa@sindbar.org.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddHours(-1), Permissoes = new() { new() { EntidadeId = entidades[1].Id, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Usuários" } } } },
                new Usuario { Nome = "Pedro Martins", Email = "pedro.martins@sindbar.org.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddDays(-1), Permissoes = new() { new() { EntidadeId = entidades[1].Id, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Mailing" } } } },
                new Usuario { Nome = "Admin Sistema", Email = "admin@dpi.com.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddHours(-3), Permissoes = new() { new() { EntidadeId = entidades[0].Id, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Entidades", "Usuários" } }, new() { EntidadeId = entidades[1].Id, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Entidades", "Usuários" } } } },
                new Usuario { Nome = "Lucia Fernandes", Email = "lucia.fernandes@sindetur.org.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddDays(-2), Permissoes = new() { new() { EntidadeId = entidades[2].Id, NomeEntidade = "SINDETUR", Modulos = new() { "Contribuintes", "Cobranças", "Mailing" } }, new() { EntidadeId = entidades[3].Id, NomeEntidade = "SINDEVEN", Modulos = new() { "Contribuintes" } } } }
            );

            db.SaveChanges();
        }
```

Substituir por:
```csharp
            db.SaveChanges();

            var usuarios = new List<ApplicationUser>
            {
                new() { UserName = "ana.lima@sindhosp.org.br", Email = "ana.lima@sindhosp.org.br", EmailConfirmed = true, Nome = "Ana Lima", UltimoAcesso = DateTime.Now.AddMinutes(-15), Permissoes = new() { new() { EntidadeId = entidades[0].Id, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing" } }, new() { EntidadeId = entidades[4].Id, NomeEntidade = "FETURH", Modulos = new() { "Contribuintes" } } } },
                new() { UserName = "carlos.silva@sindhosp.org.br", Email = "carlos.silva@sindhosp.org.br", EmailConfirmed = true, Nome = "Carlos Silva", UltimoAcesso = DateTime.Now.AddHours(-2), Permissoes = new() { new() { EntidadeId = entidades[0].Id, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Mailing" } } } },
                new() { UserName = "fernanda.costa@sindbar.org.br", Email = "fernanda.costa@sindbar.org.br", EmailConfirmed = true, Nome = "Fernanda Costa", UltimoAcesso = DateTime.Now.AddHours(-1), Permissoes = new() { new() { EntidadeId = entidades[1].Id, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Usuários" } } } },
                new() { UserName = "pedro.martins@sindbar.org.br", Email = "pedro.martins@sindbar.org.br", EmailConfirmed = true, Nome = "Pedro Martins", UltimoAcesso = DateTime.Now.AddDays(-1), Permissoes = new() { new() { EntidadeId = entidades[1].Id, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Mailing" } } } },
                new() { UserName = "admin@dpi.com.br", Email = "admin@dpi.com.br", EmailConfirmed = true, Nome = "Admin Sistema", UltimoAcesso = DateTime.Now.AddHours(-3), Permissoes = new() { new() { EntidadeId = entidades[0].Id, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Entidades", "Usuários" } }, new() { EntidadeId = entidades[1].Id, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Entidades", "Usuários" } } } },
                new() { UserName = "lucia.fernandes@sindetur.org.br", Email = "lucia.fernandes@sindetur.org.br", EmailConfirmed = true, Nome = "Lucia Fernandes", UltimoAcesso = DateTime.Now.AddDays(-2), Permissoes = new() { new() { EntidadeId = entidades[2].Id, NomeEntidade = "SINDETUR", Modulos = new() { "Contribuintes", "Cobranças", "Mailing" } }, new() { EntidadeId = entidades[3].Id, NomeEntidade = "SINDEVEN", Modulos = new() { "Contribuintes" } } } }
            };
            foreach (var usuario in usuarios)
                await userManager.CreateAsync(usuario, "Senha@123");
        }
```

(O `db.SaveChanges()` que salvava `Campanhas` + `Usuarios` juntos agora salva só `Campanhas`; os usuários são persistidos pelo `UserManager.CreateAsync`, que faz seu próprio `SaveChanges` internamente.)

- [ ] **Step 3: Build completo do backend**

Run (a partir de `src/backend`): `dotnet build`
Expected: FAIL só em `AuthController`/`Api/Models/Auth.cs` (ainda não atualizados — Task 11). Confirme que não sobrou erro em `SeedData.cs`.

- [ ] **Step 4: Commit**

```bash
git add src/backend/BlazorTeste.Infrastructure/Data/SeedData.cs
git commit -m "refactor(infrastructure): seed users via UserManager.CreateAsync"
```

---

## Task 11: `AuthController` + `Api/Models/Auth.cs` — endpoints de login/2FA

**Files:**
- Modify: `src/backend/BlazorTeste.Api/Models/Auth.cs`
- Modify: `src/backend/BlazorTeste.Api/Controllers/AuthController.cs`

**Interfaces:**
- Consumes: `IAuthAppService` (Task 7).
- Produces: `POST api/auth/login` (200 com JWT, ou 202 com `{ requiresTwoFactor, mfaToken }`, ou 401), `POST api/auth/login/2fa`, `POST api/auth/refresh`, `POST api/auth/logout` (inalterados na URL), `POST api/auth/2fa/setup` (`[Authorize]`), `POST api/auth/2fa/enable` (`[Authorize]`), `POST api/auth/2fa/disable` (`[Authorize]`) — consumidos pelo frontend (Tasks 13-15).

- [ ] **Step 1: Reescrever `Api/Models/Auth.cs`**

```csharp
namespace BlazorTeste.Api.Models;

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Senha { get; set; } = "";
}

public class LoginResponse
{
    public bool RequiresTwoFactor { get; set; }
    public string? MfaToken { get; set; }
    public string AccessToken { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
}

public class TwoFactorVerifyRequest
{
    public string MfaToken { get; set; } = "";
    public string Code { get; set; } = "";
}

public class TwoFactorCodeRequest
{
    public string Code { get; set; } = "";
}

public class TwoFactorSetupResponse
{
    public string SharedKey { get; set; } = "";
    public string QrCodePngBase64 { get; set; } = "";
}
```

- [ ] **Step 2: Reescrever `AuthController.cs`**

```csharp
using System.Security.Claims;
using BlazorTeste.Api.Models;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthAppService authService) : ControllerBase
{
    private const string CookieName = "refreshToken";

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var result = await authService.LoginAsync(req.Email, req.Senha);
        if (result is null) return Unauthorized("E-mail ou senha inválidos.");

        if (result.RequiresTwoFactor)
            return Accepted(new LoginResponse { RequiresTwoFactor = true, MfaToken = result.MfaToken });

        SetRefreshCookie(result.Auth!.RefreshToken, result.Auth.RefreshTokenExpiry);
        return Ok(new LoginResponse
        {
            AccessToken = result.Auth.AccessToken,
            Nome = result.Auth.Nome,
            Email = result.Auth.Email
        });
    }

    [HttpPost("login/2fa")]
    public async Task<ActionResult<LoginResponse>> LoginTwoFactor([FromBody] TwoFactorVerifyRequest req)
    {
        var result = await authService.VerifyTwoFactorAsync(req.MfaToken, req.Code);
        if (result is null) return BadRequest("Código inválido ou sessão de verificação expirada.");

        SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiry);
        return Ok(new LoginResponse
        {
            AccessToken = result.AccessToken,
            Nome = result.Nome,
            Email = result.Email
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh()
    {
        var cookie = Request.Cookies[CookieName];
        if (string.IsNullOrEmpty(cookie)) return Unauthorized();

        var result = await authService.RefreshAsync(cookie);
        if (result is null) return Unauthorized();

        SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiry);
        return Ok(new LoginResponse
        {
            AccessToken = result.AccessToken,
            Nome = result.Nome,
            Email = result.Email
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var cookie = Request.Cookies[CookieName];
        if (!string.IsNullOrEmpty(cookie))
            await authService.LogoutAsync(cookie);

        var isHttps = Request.IsHttps;
        Response.Cookies.Delete(CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax
        });
        return NoContent();
    }

    [Authorize]
    [HttpPost("2fa/setup")]
    public async Task<ActionResult<TwoFactorSetupResponse>> SetupTwoFactor()
    {
        var setup = await authService.SetupTwoFactorAsync(CurrentUserId());
        return Ok(new TwoFactorSetupResponse
        {
            SharedKey = setup.SharedKey,
            QrCodePngBase64 = setup.QrCodePngBase64
        });
    }

    [Authorize]
    [HttpPost("2fa/enable")]
    public async Task<IActionResult> EnableTwoFactor([FromBody] TwoFactorCodeRequest req)
    {
        var ok = await authService.EnableTwoFactorAsync(CurrentUserId(), req.Code);
        return ok ? NoContent() : BadRequest("Código inválido.");
    }

    [Authorize]
    [HttpPost("2fa/disable")]
    public async Task<IActionResult> DisableTwoFactor([FromBody] TwoFactorCodeRequest req)
    {
        var ok = await authService.DisableTwoFactorAsync(CurrentUserId(), req.Code);
        return ok ? NoContent() : BadRequest("Código inválido.");
    }

    private Guid CurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.Parse(raw!);
    }

    private void SetRefreshCookie(string token, DateTime expiry)
    {
        var isHttps = Request.IsHttps;
        Response.Cookies.Append(CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = expiry
        });
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add src/backend/BlazorTeste.Api/Models/Auth.cs src/backend/BlazorTeste.Api/Controllers/AuthController.cs
git commit -m "feat(api): AuthController exposes 2FA login step and setup/enable/disable endpoints"
```

---

## Task 12: Build + smoke test do backend (rodar Task 5 aqui se ainda não rodou)

**Files:** nenhum (só verificação).

- [ ] **Step 1: Build completo**

Run (a partir de `src/backend`): `dotnet build`
Expected: `Build succeeded.` — 0 erros.

- [ ] **Step 2: Se a Task 5 ainda não foi executada, rodar agora**

Run (a partir de `src/backend`):
```bash
dotnet ef database drop --project BlazorTeste.Infrastructure --startup-project BlazorTeste.Api --force
dotnet ef migrations add Initial --project BlazorTeste.Infrastructure --startup-project BlazorTeste.Api --output-dir Data/Migrations
dotnet ef database update --project BlazorTeste.Infrastructure --startup-project BlazorTeste.Api
```
Expected: migration `Initial` única, banco recriado, seed roda sem erro no próximo `dotnet run`.

- [ ] **Step 3: Rodar a API e testar login sem 2FA**

Run: `dotnet run --project BlazorTeste.Api` (deixar rodando; anotar a porta HTTPS de `appsettings`/`launchSettings`, ex. `https://localhost:7247`)

Em outro terminal:
```bash
curl -k -X POST https://localhost:7247/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@dpi.com.br","senha":"Senha@123"}'
```
Expected: `200 OK` com JSON `{ "requiresTwoFactor": false, "mfaToken": null, "accessToken": "eyJ...", "nome": "Admin Sistema", "email": "admin@dpi.com.br" }`.

- [ ] **Step 4: Testar credenciais inválidas**

```bash
curl -k -i -X POST https://localhost:7247/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@dpi.com.br","senha":"errada"}'
```
Expected: `401 Unauthorized`.

Parar o `dotnet run` (Ctrl+C) depois de confirmar.

- [ ] **Step 5: Commit (se algum ajuste foi necessário nos steps acima)**

Só commitar se algo precisou de correção; caso contrário, não há mudança pra commitar nesta task.

---

## Task 13: Frontend — Models atualizados (2FA + `Usuario.Id: Guid`)

**Files:**
- Modify: `src/frontend/BlazorTeste/Models/Auth.cs`
- Modify: `src/frontend/BlazorTeste/Models/Domain.cs`

**Interfaces:**
- Produces: `LoginResponse { RequiresTwoFactor, MfaToken, AccessToken, Nome, Email }`, `TwoFactorVerifyRequest`, `TwoFactorCodeRequest`, `TwoFactorSetupResponse`, `Usuario.Id: Guid` — usados por `Login.razor` (Task 14) e `MinhaConta.razor` (Task 15).

- [ ] **Step 1: Reescrever `Models/Auth.cs`**

```csharp
namespace BlazorTeste.Models;

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Senha { get; set; } = "";
}

public class LoginResponse
{
    public bool RequiresTwoFactor { get; set; }
    public string? MfaToken { get; set; }
    public string AccessToken { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
}

public class TwoFactorVerifyRequest
{
    public string MfaToken { get; set; } = "";
    public string Code { get; set; } = "";
}

public class TwoFactorCodeRequest
{
    public string Code { get; set; } = "";
}

public class TwoFactorSetupResponse
{
    public string SharedKey { get; set; } = "";
    public string QrCodePngBase64 { get; set; } = "";
}
```

- [ ] **Step 2: Editar `Models/Domain.cs`**

Trecho atual (linhas ~401-408):
```csharp
public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime UltimoAcesso { get; set; }
    public List<PermissaoEntidade> Permissoes { get; set; } = new();
}
```

Substituir por:
```csharp
public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime UltimoAcesso { get; set; }
    public List<PermissaoEntidade> Permissoes { get; set; } = new();
}
```

- [ ] **Step 3: Build do frontend**

Run (a partir de `src/frontend/BlazorTeste`): `dotnet build`
Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add src/frontend/BlazorTeste/Models/Auth.cs src/frontend/BlazorTeste/Models/Domain.cs
git commit -m "feat(frontend): add 2FA models, Usuario.Id becomes Guid"
```

---

## Task 14: `Login.razor` — segundo passo (código 2FA)

**Files:**
- Modify: `src/frontend/BlazorTeste/Components/Pages/Auth/Login.razor`

**Interfaces:**
- Consumes: `LoginRequest`/`LoginResponse`/`TwoFactorVerifyRequest` (Task 13), `TokenService` (existente).

- [ ] **Step 1: Reescrever `Login.razor`**

```razor
@page "/login"
@attribute [Microsoft.AspNetCore.Authorization.AllowAnonymous]
@layout LoginLayout
@inject TokenService TokenService
@inject NavigationManager Navigation
@inject HttpClient Http
@using BlazorTeste.Services.Auth
@using BlazorTeste.Models
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.WebAssembly.Http
@using System.Net.Http.Json

<PageTitle>Login — SindERP</PageTitle>

<div style="display:flex; justify-content:center; align-items:center; min-height:100vh;
            background:linear-gradient(135deg, #1565C0 0%, #0D47A1 60%, #01579B 100%);">
    <MudCard Elevation="8" Class="rounded-xl" Style="width:100%; max-width:420px; margin:1rem;">
        <MudCardContent Class="pa-8">
            <div class="d-flex flex-column align-center mb-6">
                <MudAvatar Color="Color.Primary" Size="Size.Large" Style="width:64px; height:64px; font-size:2rem;" Class="mb-3">
                    <MudIcon Icon="@Icons.Material.Filled.AccountBalance" Size="Size.Large" />
                </MudAvatar>
                <MudText Typo="Typo.h5" Color="Color.Primary" Style="font-weight:700; letter-spacing:.5px;">SindERP</MudText>
                <MudText Typo="Typo.body2" Color="Color.Secondary">Sistema de Gestão Sindical</MudText>
            </div>

            @if (_erro != null)
            {
                <MudAlert Severity="Severity.Error" Class="mb-4" Dense="true">@_erro</MudAlert>
            }

            @if (!_aguardandoCodigo)
            {
                <MudTextField @bind-Value="_email"
                              Label="E-mail"
                              Variant="Variant.Outlined"
                              InputType="InputType.Email"
                              Adornment="Adornment.Start"
                              AdornmentIcon="@Icons.Material.Filled.Email"
                              Class="mb-3"
                              FullWidth="true" />

                <MudTextField @bind-Value="_senha"
                              Label="Senha"
                              Variant="Variant.Outlined"
                              InputType="InputType.Password"
                              Adornment="Adornment.Start"
                              AdornmentIcon="@Icons.Material.Filled.Lock"
                              Class="mb-5"
                              FullWidth="true" />

                <MudButton Variant="Variant.Filled"
                           Color="Color.Primary"
                           FullWidth="true"
                           Size="Size.Large"
                           OnClick="Entrar"
                           Disabled="_loading"
                           Class="mb-4">
                    @(_loading ? "Entrando..." : "Entrar")
                </MudButton>

                <MudAlert Severity="Severity.Info" Dense="true" Variant="Variant.Outlined">
                    <MudText Typo="Typo.caption">
                        Senha padrão de todos os usuários seed: <b>Senha@123</b>
                    </MudText>
                </MudAlert>
            }
            else
            {
                <MudText Typo="Typo.body2" Class="mb-4">
                    Digite o código de 6 dígitos do seu app autenticador.
                </MudText>

                <MudTextField @bind-Value="_codigo"
                              Label="Código de verificação"
                              Variant="Variant.Outlined"
                              InputType="InputType.Text"
                              Adornment="Adornment.Start"
                              AdornmentIcon="@Icons.Material.Filled.Security"
                              Class="mb-5"
                              FullWidth="true" />

                <MudButton Variant="Variant.Filled"
                           Color="Color.Primary"
                           FullWidth="true"
                           Size="Size.Large"
                           OnClick="VerificarCodigo"
                           Disabled="_loading"
                           Class="mb-2">
                    @(_loading ? "Verificando..." : "Verificar")
                </MudButton>

                <MudButton Variant="Variant.Text"
                           FullWidth="true"
                           OnClick="@(() => { _aguardandoCodigo = false; _erro = null; })">
                    Voltar
                </MudButton>
            }
        </MudCardContent>
    </MudCard>
</div>

@code {
    private string _email = "";
    private string _senha = "";
    private string _codigo = "";
    private string? _mfaToken;
    private bool _aguardandoCodigo;
    private string? _erro;
    private bool _loading;

    [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthStateTask;
        if (state.User.Identity?.IsAuthenticated == true)
            Navigation.NavigateTo("/", replace: true);
    }

    private async Task Entrar()
    {
        _erro = null;

        if (string.IsNullOrWhiteSpace(_email) || string.IsNullOrWhiteSpace(_senha))
        {
            _erro = "Informe e-mail e senha.";
            return;
        }

        _loading = true;
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/login");
            req.Content = JsonContent.Create(new LoginRequest { Email = _email.Trim(), Senha = _senha });
            var response = await Http.SendAsync(req);

            if (response.StatusCode is not (System.Net.HttpStatusCode.OK or System.Net.HttpStatusCode.Accepted))
            {
                _erro = "E-mail ou senha inválidos.";
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null)
            {
                _erro = "E-mail ou senha inválidos.";
                return;
            }

            if (result.RequiresTwoFactor)
            {
                _mfaToken = result.MfaToken;
                _aguardandoCodigo = true;
                return;
            }

            TokenService.SetToken(result.AccessToken);
            Navigation.NavigateTo("/", replace: true);
        }
        catch
        {
            _erro = "Não foi possível conectar ao servidor.";
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task VerificarCodigo()
    {
        _erro = null;

        if (string.IsNullOrWhiteSpace(_codigo) || _mfaToken is null)
        {
            _erro = "Informe o código de verificação.";
            return;
        }

        _loading = true;
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/login/2fa");
            req.Content = JsonContent.Create(new TwoFactorVerifyRequest { MfaToken = _mfaToken, Code = _codigo.Trim() });
            var response = await Http.SendAsync(req);

            if (!response.IsSuccessStatusCode)
            {
                _erro = "Código inválido ou sessão de verificação expirada.";
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            TokenService.SetToken(result!.AccessToken);
            Navigation.NavigateTo("/", replace: true);
        }
        catch
        {
            _erro = "Não foi possível conectar ao servidor.";
        }
        finally
        {
            _loading = false;
        }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add src/frontend/BlazorTeste/Components/Pages/Auth/Login.razor
git commit -m "feat(frontend): Login.razor supports TOTP 2FA second step"
```

---

## Task 15: Página "Minha Conta" (setup 2FA) + link no menu

**Files:**
- Create: `src/frontend/BlazorTeste/Components/Pages/Auth/MinhaConta.razor`
- Modify: `src/frontend/BlazorTeste/Components/Layout/MainLayout.razor`

**Interfaces:**
- Consumes: `TwoFactorSetupResponse`/`TwoFactorCodeRequest` (Task 13), `HttpClient` injetado (já passa pelo `AuthHttpMessageHandler`, adiciona `Authorization: Bearer` automaticamente).

- [ ] **Step 1: Criar `MinhaConta.razor`**

```razor
@page "/minha-conta"
@inject HttpClient Http
@inject ISnackbar Snackbar
@using BlazorTeste.Models
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Authorization

<PageTitle>Minha Conta — SindERP</PageTitle>

<MudText Typo="Typo.h5" Style="font-weight:600;" Class="mb-4">Minha Conta</MudText>

<MudCard Elevation="2" Class="rounded-xl" Style="max-width:520px;">
    <MudCardContent Class="pa-6">
        <MudText Typo="Typo.h6" Class="mb-1">Segurança</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary" Class="mb-4">
            Autenticação de dois fatores (2FA) via app autenticador (Google Authenticator, Microsoft Authenticator, Authy).
        </MudText>

        @if (_carregando)
        {
            <MudProgressCircular Indeterminate="true" Size="Size.Small" />
        }
        else if (!_duasEtapasAtiva && _qrCodeBase64 is null)
        {
            <MudAlert Severity="Severity.Info" Dense="true" Class="mb-4">2FA está desativado.</MudAlert>
            <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Security" OnClick="IniciarAtivacao">
                Ativar 2FA
            </MudButton>
        }
        else if (_qrCodeBase64 is not null)
        {
            <MudText Typo="Typo.body2" Class="mb-3">Escaneie o QR code no seu app autenticador e digite o código gerado pra confirmar.</MudText>
            <div class="d-flex justify-center mb-3">
                <img src="@($"data:image/png;base64,{_qrCodeBase64}")" alt="QR code 2FA" width="200" height="200" />
            </div>
            <MudText Typo="Typo.caption" Color="Color.Secondary" Class="mb-3">
                Chave manual: <code>@_sharedKey</code>
            </MudText>
            <MudTextField @bind-Value="_codigo" Label="Código de verificação" Variant="Variant.Outlined" Class="mb-3" FullWidth="true" />
            <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="ConfirmarAtivacao" Disabled="_processando">
                Confirmar ativação
            </MudButton>
        }
        else
        {
            <MudAlert Severity="Severity.Success" Dense="true" Class="mb-4">2FA está ativado.</MudAlert>
            <MudTextField @bind-Value="_codigo" Label="Código de verificação (pra desativar)" Variant="Variant.Outlined" Class="mb-3" FullWidth="true" />
            <MudButton Variant="Variant.Outlined" Color="Color.Error" OnClick="Desativar" Disabled="_processando">
                Desativar 2FA
            </MudButton>
        }
    </MudCardContent>
</MudCard>

@code {
    [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = default!;

    private bool _carregando = true;
    private bool _processando;
    private bool _duasEtapasAtiva;
    private string? _qrCodeBase64;
    private string _sharedKey = "";
    private string _codigo = "";

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthStateTask;
        _duasEtapasAtiva = state.User.FindFirst("twoFactorEnabled")?.Value == "true";
        _carregando = false;
    }

    private async Task IniciarAtivacao()
    {
        _processando = true;
        try
        {
            var response = await Http.PostAsync("api/auth/2fa/setup", null);
            if (!response.IsSuccessStatusCode)
            {
                Snackbar.Add("Não foi possível iniciar a ativação do 2FA.", Severity.Error);
                return;
            }
            var setup = await response.Content.ReadFromJsonAsync<TwoFactorSetupResponse>();
            _qrCodeBase64 = setup!.QrCodePngBase64;
            _sharedKey = setup.SharedKey;
        }
        finally
        {
            _processando = false;
        }
    }

    private async Task ConfirmarAtivacao()
    {
        if (string.IsNullOrWhiteSpace(_codigo))
        {
            Snackbar.Add("Informe o código de verificação.", Severity.Warning);
            return;
        }

        _processando = true;
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/2fa/enable")
            {
                Content = JsonContent.Create(new TwoFactorCodeRequest { Code = _codigo.Trim() })
            };
            var response = await Http.SendAsync(req);
            if (!response.IsSuccessStatusCode)
            {
                Snackbar.Add("Código inválido.", Severity.Error);
                return;
            }

            _qrCodeBase64 = null;
            _duasEtapasAtiva = true;
            _codigo = "";
            Snackbar.Add("2FA ativado com sucesso.", Severity.Success);
        }
        finally
        {
            _processando = false;
        }
    }

    private async Task Desativar()
    {
        if (string.IsNullOrWhiteSpace(_codigo))
        {
            Snackbar.Add("Informe o código de verificação.", Severity.Warning);
            return;
        }

        _processando = true;
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/2fa/disable")
            {
                Content = JsonContent.Create(new TwoFactorCodeRequest { Code = _codigo.Trim() })
            };
            var response = await Http.SendAsync(req);
            if (!response.IsSuccessStatusCode)
            {
                Snackbar.Add("Código inválido.", Severity.Error);
                return;
            }

            _duasEtapasAtiva = false;
            _codigo = "";
            Snackbar.Add("2FA desativado.", Severity.Success);
        }
        finally
        {
            _processando = false;
        }
    }
}
```

**Nota:** o estado inicial de "2FA ativado/desativado" vem da claim `twoFactorEnabled` do JWT (adicionada em `GenerateAccessToken` na Task 7), que reflete o valor no momento do login/refresh. Se o usuário ativar/desativar 2FA nesta página, o componente já atualiza `_duasEtapasAtiva` localmente (sem precisar de um novo login pra refletir na UI) — a claim só fica potencialmente desatualizada se o usuário abrir a página em outra aba/sessão sem ter feito login de novo ali. Aceitável: o próximo refresh de token corrige.

- [ ] **Step 2: Wirar o menu "Perfil" em `MainLayout.razor`**

Trecho atual:
```razor
                <MudMenuItem Icon="@Icons.Material.Filled.Person">Perfil</MudMenuItem>
```

Substituir por:
```razor
                <MudMenuItem Icon="@Icons.Material.Filled.Person" OnClick="@(() => Navigation.NavigateTo("/minha-conta"))">Perfil</MudMenuItem>
```

- [ ] **Step 3: Build do frontend**

Run (a partir de `src/frontend/BlazorTeste`): `dotnet build`
Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add src/frontend/BlazorTeste/Components/Pages/Auth/MinhaConta.razor src/frontend/BlazorTeste/Components/Layout/MainLayout.razor
git commit -m "feat(frontend): add Minha Conta page for 2FA setup/enable/disable"
```

---

## Task 16: Verificação manual end-to-end

**Files:** nenhum (só verificação, conforme seção "Testes / Verificação" do spec).

- [ ] **Step 1: Subir backend e frontend**

Terminal 1 (a partir de `src/backend`): `dotnet run --project BlazorTeste.Api`
Terminal 2 (a partir de `src/frontend/BlazorTeste`): `dotnet run`

- [ ] **Step 2: Login sem 2FA (regressão)**

No browser, abrir o frontend, logar com `admin@dpi.com.br` / `Senha@123`. Expected: entra direto, sem tela de código.

- [ ] **Step 3: Ativar 2FA**

Ir em "Minha Conta" (menu do avatar → Perfil), clicar "Ativar 2FA", escanear o QR com um app autenticador real (Google Authenticator/Authy), digitar o código gerado, confirmar. Expected: mensagem "2FA ativado com sucesso."

- [ ] **Step 4: Logout e login com 2FA**

Fazer logout, logar de novo com o mesmo usuário. Expected: tela pede código de 6 dígitos após senha; código certo do app loga normalmente; código errado mostra "Código inválido ou sessão de verificação expirada."

- [ ] **Step 5: Lockout**

Errar o código 5 vezes seguidas. Expected: mesmo digitando o código certo depois, login continua bloqueado por ~15 min (`IsLockedOutAsync` retorna true → mensagem genérica).

- [ ] **Step 6: Desativar 2FA**

Voltar em "Minha Conta", digitar código válido atual, clicar "Desativar 2FA". Expected: mensagem de sucesso; próximo login não pede mais código.

- [ ] **Step 7: `mfaToken` expirado**

Ativar 2FA de novo, fazer login (chegar na tela de código), esperar mais de 5 minutos (ou reiniciar a API, que limpa o `IMemoryCache`) antes de digitar o código. Expected: "Código inválido ou sessão de verificação expirada."

Nenhum commit nesta task — é checklist de verificação manual do fluxo completo.
