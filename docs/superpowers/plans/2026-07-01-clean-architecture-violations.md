# Clean Architecture Violations Fix

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Remover dependências diretas de `AppDbContext` nos controllers, movendo a lógica para a camada Application via interfaces.

**Architecture:** Cada controller violador recebe um AppService correspondente. Application define interfaces, Infrastructure implementa. Controllers ficam finos — só HTTP.

**Tech Stack:** .NET 10, ASP.NET Core, EF Core, JWT (System.IdentityModel.Tokens.Jwt)

## Global Constraints

- Application layer: depende apenas de Domain + abstrações do framework (`IConfiguration`)
- Infrastructure layer: implementa interfaces definidas em Application/Domain
- Controllers: injetam apenas interfaces de Application
- Sem `AppDbContext` em nenhum controller
- Seguir padrão existente: interfaces em `Services/Interfaces/`, implementações em `Services/Implementations/`
- DTOs em `Application/DTOs/`

---

### Task 1: ICampanhaRepository + CampanhaRepository

**Files:**
- Create: `src/backend/BlazorTeste.Domain/Interfaces/Repositories/ICampanhaRepository.cs`
- Create: `src/backend/BlazorTeste.Infrastructure/Repositories/CampanhaRepository.cs`
- Modify: `src/backend/BlazorTeste.Infrastructure/DependencyInjection.cs`

**Interfaces:**
- Produces: `ICampanhaRepository` com `GetByEntidadeAsync(int entidadeId)`

- [ ] **Step 1: Criar ICampanhaRepository**

```csharp
// src/backend/BlazorTeste.Domain/Interfaces/Repositories/ICampanhaRepository.cs
using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface ICampanhaRepository : IRepository<Campanha>
{
    Task<IReadOnlyList<Campanha>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
}
```

- [ ] **Step 2: Criar CampanhaRepository**

```csharp
// src/backend/BlazorTeste.Infrastructure/Repositories/CampanhaRepository.cs
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class CampanhaRepository : BaseRepository<Campanha>, ICampanhaRepository
{
    public CampanhaRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Campanha>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(c => c.EntidadeId == entidadeId).ToListAsync(cancellationToken);
}
```

- [ ] **Step 3: Registrar no DI**

Adicionar ao `DependencyInjection.cs`:
```csharp
services.AddScoped<ICampanhaRepository, CampanhaRepository>();
```

---

### Task 2: IConfiguracaoRepository + ConfiguracaoRepository

**Files:**
- Create: `src/backend/BlazorTeste.Domain/Interfaces/Repositories/IConfiguracaoRepository.cs`
- Create: `src/backend/BlazorTeste.Infrastructure/Repositories/ConfiguracaoRepository.cs`
- Modify: `src/backend/BlazorTeste.Infrastructure/DependencyInjection.cs`

**Interfaces:**
- Produces: `IConfiguracaoRepository` com `GetByEntidadeAsync(int entidadeId)`

- [ ] **Step 1: Criar IConfiguracaoRepository**

```csharp
// src/backend/BlazorTeste.Domain/Interfaces/Repositories/IConfiguracaoRepository.cs
using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IConfiguracaoRepository
{
    Task<ConfiguracaoEntidade?> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task UpdateAsync(ConfiguracaoEntidade entity, CancellationToken cancellationToken = default);
}
```

- [ ] **Step 2: Criar ConfiguracaoRepository**

```csharp
// src/backend/BlazorTeste.Infrastructure/Repositories/ConfiguracaoRepository.cs
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class ConfiguracaoRepository : IConfiguracaoRepository
{
    private readonly AppDbContext _context;

    public ConfiguracaoRepository(AppDbContext context) => _context = context;

    public async Task<ConfiguracaoEntidade?> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _context.Configuracoes.FirstOrDefaultAsync(c => c.EntidadeId == entidadeId, cancellationToken);

    public async Task UpdateAsync(ConfiguracaoEntidade entity, CancellationToken cancellationToken = default)
    {
        _context.Configuracoes.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

- [ ] **Step 3: Registrar no DI**

Adicionar ao `DependencyInjection.cs`:
```csharp
services.AddScoped<IConfiguracaoRepository, ConfiguracaoRepository>();
```

---

### Task 3: IPasswordHasher — interface em Application, impl em Infrastructure

**Files:**
- Create: `src/backend/BlazorTeste.Application/Security/IPasswordHasher.cs`
- Modify: `src/backend/BlazorTeste.Infrastructure/Security/PasswordHelper.cs` (add interface impl)
- Modify: `src/backend/BlazorTeste.Infrastructure/DependencyInjection.cs`

**Interfaces:**
- Produces: `IPasswordHasher` com `Verify(password, hash) → bool`

- [ ] **Step 1: Criar IPasswordHasher**

```csharp
// src/backend/BlazorTeste.Application/Security/IPasswordHasher.cs
namespace BlazorTeste.Application.Security;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string storedHash);
}
```

- [ ] **Step 2: Criar PasswordHasher em Infrastructure**

```csharp
// src/backend/BlazorTeste.Infrastructure/Security/PasswordHasher.cs
using BlazorTeste.Application.Security;

namespace BlazorTeste.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => PasswordHelper.Hash(password);
    public bool Verify(string password, string storedHash) => PasswordHelper.Verify(password, storedHash);
}
```

- [ ] **Step 3: Registrar no DI**

```csharp
services.AddScoped<IPasswordHasher, PasswordHasher>();
```

---

### Task 4: IMailingAppService + CampanhaDto + MailingAppService + refactor MailingController

**Files:**
- Create: `src/backend/BlazorTeste.Application/DTOs/CampanhaDto.cs`
- Create: `src/backend/BlazorTeste.Application/Services/Interfaces/IMailingAppService.cs`
- Create: `src/backend/BlazorTeste.Application/Services/Implementations/MailingAppService.cs`
- Modify: `src/backend/BlazorTeste.Api/Controllers/MailingController.cs`
- Modify: `src/backend/BlazorTeste.Api/Program.cs`

- [ ] **Step 1: Criar CampanhaDto**

```csharp
// src/backend/BlazorTeste.Application/DTOs/CampanhaDto.cs
namespace BlazorTeste.Application.DTOs;

public class CampanhaDto
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Assunto { get; set; } = "";
    public string Destinatarios { get; set; } = "";
    public int TotalDestinatarios { get; set; }
    public DateTime? DataEnvio { get; set; }
    public string Status { get; set; } = "";
    public string Criador { get; set; } = "";
}
```

- [ ] **Step 2: Criar IMailingAppService**

```csharp
// src/backend/BlazorTeste.Application/Services/Interfaces/IMailingAppService.cs
using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IMailingAppService
{
    Task<IEnumerable<CampanhaDto>> GetAllAsync();
}
```

- [ ] **Step 3: Criar MailingAppService**

```csharp
// src/backend/BlazorTeste.Application/Services/Implementations/MailingAppService.cs
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class MailingAppService(ICampanhaRepository repo) : IMailingAppService
{
    public async Task<IEnumerable<CampanhaDto>> GetAllAsync()
    {
        var items = await repo.GetAllAsync();
        return items.Select(c => new CampanhaDto
        {
            Id = c.Id,
            EntidadeId = c.EntidadeId,
            Assunto = c.Assunto,
            Destinatarios = c.Destinatarios,
            TotalDestinatarios = c.TotalDestinatarios,
            DataEnvio = c.DataEnvio,
            Status = c.Status.ToString(),
            Criador = c.Criador
        });
    }
}
```

- [ ] **Step 4: Refatorar MailingController**

```csharp
// src/backend/BlazorTeste.Api/Controllers/MailingController.cs
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MailingController(IMailingAppService service) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<CampanhaDto>> GetAll() =>
        await service.GetAllAsync();
}
```

- [ ] **Step 5: Registrar no Program.cs**

```csharp
builder.Services.AddScoped<IMailingAppService, MailingAppService>();
```

---

### Task 5: IUsuarioAppService + UsuarioDto + UsuarioAppService + refactor UsuariosController

**Files:**
- Create: `src/backend/BlazorTeste.Application/DTOs/UsuarioDto.cs`
- Create: `src/backend/BlazorTeste.Application/Services/Interfaces/IUsuarioAppService.cs`
- Create: `src/backend/BlazorTeste.Application/Services/Implementations/UsuarioAppService.cs`
- Modify: `src/backend/BlazorTeste.Api/Controllers/UsuariosController.cs`
- Modify: `src/backend/BlazorTeste.Api/Program.cs`

- [ ] **Step 1: Criar UsuarioDto** (sem SenhaHash, RefreshToken)

```csharp
// src/backend/BlazorTeste.Application/DTOs/UsuarioDto.cs
using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Application.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime UltimoAcesso { get; set; }
    public List<PermissaoEntidade> Permissoes { get; set; } = new();
}
```

- [ ] **Step 2: Criar IUsuarioAppService**

```csharp
// src/backend/BlazorTeste.Application/Services/Interfaces/IUsuarioAppService.cs
using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IUsuarioAppService
{
    Task<IEnumerable<UsuarioDto>> GetAllAsync();
    Task<UsuarioDto?> GetByEmailAsync(string email);
}
```

- [ ] **Step 3: Criar UsuarioAppService**

```csharp
// src/backend/BlazorTeste.Application/Services/Implementations/UsuarioAppService.cs
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class UsuarioAppService(IUsuarioRepository repo) : IUsuarioAppService
{
    public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
    {
        var items = await repo.GetAllAsync();
        return items.Select(Map);
    }

    public async Task<UsuarioDto?> GetByEmailAsync(string email)
    {
        var u = await repo.GetByEmailAsync(email);
        return u is null ? null : Map(u);
    }

    private static UsuarioDto Map(BlazorTeste.Domain.Entities.Usuario u) => new()
    {
        Id = u.Id,
        Nome = u.Nome,
        Email = u.Email,
        UltimoAcesso = u.UltimoAcesso,
        Permissoes = u.Permissoes
    };
}
```

- [ ] **Step 4: Refatorar UsuariosController**

```csharp
// src/backend/BlazorTeste.Api/Controllers/UsuariosController.cs
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsuariosController(IUsuarioAppService service) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<UsuarioDto>> GetAll() =>
        await service.GetAllAsync();

    [HttpGet("by-email")]
    public async Task<ActionResult<UsuarioDto>> GetByEmail([FromQuery] string email)
    {
        var u = await service.GetByEmailAsync(email);
        return u is null ? NotFound() : Ok(u);
    }
}
```

- [ ] **Step 5: Registrar no Program.cs**

```csharp
builder.Services.AddScoped<IUsuarioAppService, UsuarioAppService>();
```

---

### Task 6: IAuthAppService + AuthResultDto + AuthAppService + refactor AuthController

**Files:**
- Create: `src/backend/BlazorTeste.Application/DTOs/AuthResultDto.cs`
- Create: `src/backend/BlazorTeste.Application/Services/Interfaces/IAuthAppService.cs`
- Create: `src/backend/BlazorTeste.Application/Services/Implementations/AuthAppService.cs`
- Modify: `src/backend/BlazorTeste.Api/Controllers/AuthController.cs`
- Modify: `src/backend/BlazorTeste.Api/Program.cs`

- [ ] **Step 1: Criar AuthResultDto**

```csharp
// src/backend/BlazorTeste.Application/DTOs/AuthResultDto.cs
namespace BlazorTeste.Application.DTOs;

public class AuthResultDto
{
    public string AccessToken { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime RefreshTokenExpiry { get; set; }
}
```

- [ ] **Step 2: Criar IAuthAppService**

```csharp
// src/backend/BlazorTeste.Application/Services/Interfaces/IAuthAppService.cs
using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IAuthAppService
{
    Task<AuthResultDto?> LoginAsync(string email, string senha);
    Task<AuthResultDto?> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
```

- [ ] **Step 3: Criar AuthAppService**

```csharp
// src/backend/BlazorTeste.Application/Services/Implementations/AuthAppService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Security;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlazorTeste.Application.Services.Implementations;

public class AuthAppService(
    IUsuarioRepository usuarioRepo,
    IPasswordHasher passwordHasher,
    IConfiguration config) : IAuthAppService
{
    public async Task<AuthResultDto?> LoginAsync(string email, string senha)
    {
        var user = await usuarioRepo.GetByEmailAsync(email.ToLower());
        if (user is null || !passwordHasher.Verify(senha, user.SenhaHash))
            return null;

        user.UltimoAcesso = DateTime.UtcNow;
        var (refreshToken, expiry) = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = expiry;
        await usuarioRepo.UpdateAsync(user);

        return new AuthResultDto
        {
            AccessToken = GenerateAccessToken(user),
            Nome = user.Nome,
            Email = user.Email,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = expiry
        };
    }

    public async Task<AuthResultDto?> RefreshAsync(string refreshToken)
    {
        var user = await usuarioRepo.GetByRefreshTokenAsync(refreshToken);
        if (user is null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return null;

        var (newToken, expiry) = GenerateRefreshToken();
        user.RefreshToken = newToken;
        user.RefreshTokenExpiry = expiry;
        await usuarioRepo.UpdateAsync(user);

        return new AuthResultDto
        {
            AccessToken = GenerateAccessToken(user),
            Nome = user.Nome,
            Email = user.Email,
            RefreshToken = newToken,
            RefreshTokenExpiry = expiry
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = await usuarioRepo.GetByRefreshTokenAsync(refreshToken);
        if (user is null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await usuarioRepo.UpdateAsync(user);
    }

    private (string token, DateTime expiry) GenerateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiry = DateTime.UtcNow.AddDays(config.GetValue<int>("Jwt:RefreshTokenExpiryDays"));
        return (token, expiry);
    }

    private string GenerateAccessToken(BlazorTeste.Domain.Entities.Usuario user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Nome),
            new Claim("permissoes", System.Text.Json.JsonSerializer.Serialize(user.Permissoes))
        };
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(config.GetValue<int>("Jwt:AccessTokenExpiryMinutes")),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

- [ ] **Step 4: Refatorar AuthController** (só HTTP — cookies + mapeamento)

```csharp
// src/backend/BlazorTeste.Api/Controllers/AuthController.cs
using BlazorTeste.Api.Models;
using BlazorTeste.Application.Services.Interfaces;
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

- [ ] **Step 5: Registrar no Program.cs**

```csharp
builder.Services.AddScoped<IAuthAppService, AuthAppService>();
```

---

### Task 7: Extend IConfiguracaoAppService + refactor ConfiguracoesController

**Files:**
- Modify: `src/backend/BlazorTeste.Application/Services/Interfaces/IConfiguracaoAppService.cs`
- Modify: `src/backend/BlazorTeste.Application/Services/Implementations/ConfiguracaoAppService.cs`
- Modify: `src/backend/BlazorTeste.Api/Controllers/ConfiguracoesController.cs`

- [ ] **Step 1: Estender IConfiguracaoAppService**

```csharp
// src/backend/BlazorTeste.Application/Services/Interfaces/IConfiguracaoAppService.cs
using BlazorTeste.Application.DTOs;
using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IConfiguracaoAppService
{
    Task<ConfiguracaoBancoDto?> GetBancoAsync(int entidadeId);
    Task<ConfiguracaoGeral?> GetGeralAsync(int entidadeId);
    Task<ConfiguracaoCobranca?> GetCobrancaAsync(int entidadeId);
    Task<ConfiguracaoEmail?> GetEmailAsync(int entidadeId);
}
```

- [ ] **Step 2: Atualizar ConfiguracaoAppService**

```csharp
// src/backend/BlazorTeste.Application/Services/Implementations/ConfiguracaoAppService.cs
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class ConfiguracaoAppService(IConfiguracaoRepository repo) : IConfiguracaoAppService
{
    public async Task<ConfiguracaoBancoDto?> GetBancoAsync(int entidadeId)
    {
        var config = await repo.GetByEntidadeAsync(entidadeId);
        if (config is null) return null;
        return new ConfiguracaoBancoDto
        {
            Banco = config.Banco.Banco,
            Agencia = config.Banco.Agencia,
            Conta = config.Banco.Conta,
            Cedente = config.Banco.Cedente,
            CodigoCedente = config.Banco.CodigoCedente,
            Carteira = config.Banco.Carteira
        };
    }

    public async Task<ConfiguracaoGeral?> GetGeralAsync(int entidadeId)
    {
        var config = await repo.GetByEntidadeAsync(entidadeId);
        return config?.Geral;
    }

    public async Task<ConfiguracaoCobranca?> GetCobrancaAsync(int entidadeId)
    {
        var config = await repo.GetByEntidadeAsync(entidadeId);
        return config?.Cobranca;
    }

    public async Task<ConfiguracaoEmail?> GetEmailAsync(int entidadeId)
    {
        var config = await repo.GetByEntidadeAsync(entidadeId);
        return config?.Email;
    }
}
```

- [ ] **Step 3: Refatorar ConfiguracoesController** (remover AppDbContext)

```csharp
// src/backend/BlazorTeste.Api/Controllers/ConfiguracoesController.cs
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConfiguracoesController(IConfiguracaoAppService service) : ControllerBase
{
    [HttpGet("geral")]
    public async Task<ActionResult<ConfiguracaoGeral>> GetGeral([FromQuery] int entidadeId)
    {
        var result = await service.GetGeralAsync(entidadeId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("cobranca")]
    public async Task<ActionResult<ConfiguracaoCobranca>> GetCobranca([FromQuery] int entidadeId)
    {
        var result = await service.GetCobrancaAsync(entidadeId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("email")]
    public async Task<ActionResult<ConfiguracaoEmail>> GetEmail([FromQuery] int entidadeId)
    {
        var result = await service.GetEmailAsync(entidadeId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("banco")]
    public async Task<ActionResult<ConfiguracaoBancoDto>> GetBanco([FromQuery] int entidadeId)
    {
        var dto = await service.GetBancoAsync(entidadeId);
        return dto is null ? NotFound() : Ok(dto);
    }
}
```

---

### Task 8: Verificar build

- [ ] **Step 1: Build**

```
dotnet build src/backend/BlazorTeste.Api/BlazorTeste.Api.csproj
```

Expected: Build succeeded, 0 errors.

- [ ] **Step 2: Commit**

```
git add src/backend/
git commit -m "refactor(api): remove AppDbContext from controllers — full Clean Architecture compliance"
```
