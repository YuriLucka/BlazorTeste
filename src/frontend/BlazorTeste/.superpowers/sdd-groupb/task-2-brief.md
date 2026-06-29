## Task 2: Complex-model API (Negociacao, Evento, Configuracao)

**Files:**
- Modify: `BlazorTeste.Shared/Models/Negociacao.cs` — add `Id` to `ParcelaNegociacao` and `CobrancaOriginalNeg`
- Modify: `BlazorTeste.Shared/Models/Configuracao.cs` — add `ConfiguracaoEntidade` aggregate
- Modify: `BlazorTeste.Api/Data/AppDbContext.cs` — add 3 DbSets + EF config
- Modify: `BlazorTeste.Api/Data/SeedData.cs` — add Group B seed sections
- Create: `BlazorTeste.Api/Controllers/NegociacoesController.cs`
- Create: `BlazorTeste.Api/Controllers/EventosController.cs`
- Create: `BlazorTeste.Api/Controllers/ConfiguracoesController.cs`

**Interfaces:**
- Consumes: Task 1's restructured `SeedData.cs` (must exist before running)
- Produces: `GET api/negociacoes?entidadeId={int}` → `List<Negociacao>` (with parcelas + cobrancasOriginais)
- Produces: `GET api/eventos?entidadeId={int}` → `List<Evento>` (with inscricoes)
- Produces: `GET api/configuracoes/geral?entidadeId={int}` → `ConfiguracaoGeral`
- Produces: `GET api/configuracoes/cobranca?entidadeId={int}` → `ConfiguracaoCobranca`
- Produces: `GET api/configuracoes/email?entidadeId={int}` → `ConfiguracaoEmail`
- Produces: `GET api/configuracoes/banco?entidadeId={int}` → `ConfiguracaoBanco`

- [ ] **Step 1: Add `Id` to ParcelaNegociacao and CobrancaOriginalNeg**

In `BlazorTeste.Shared/Models/Negociacao.cs`, add `public int Id { get; set; }` as first property to both `ParcelaNegociacao` and `CobrancaOriginalNeg`:

```csharp
namespace BlazorTeste.Models;

public enum StatusNegociacao { EmAndamento, Concluida, Cancelada, Inadimplente }

public class Negociacao
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int ContribuinteId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public DateTime DataAbertura { get; set; }
    public DateTime? DataConclusao { get; set; }
    public decimal DebitoOriginal { get; set; }
    public decimal ValorNegociado { get; set; }
    public decimal Desconto { get; set; }
    public int NumeroParcelas { get; set; }
    public StatusNegociacao Status { get; set; }
    public string Observacoes { get; set; } = "";
    public List<ParcelaNegociacao> Parcelas { get; set; } = new();
    public List<CobrancaOriginalNeg> CobrancasOriginais { get; set; } = new();
}

public class ParcelaNegociacao
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public decimal Valor { get; set; }
    public DateTime Vencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public bool Pago { get; set; }
    public string LinhaDigitavel { get; set; } = "";
}

public class CobrancaOriginalNeg
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "";
    public int AnoReferencia { get; set; }
    public decimal ValorOriginal { get; set; }
    public decimal Multa { get; set; }
    public decimal Juros { get; set; }
}
```

- [ ] **Step 2: Add ConfiguracaoEntidade to Shared**

In `BlazorTeste.Shared/Models/Configuracao.cs`, add at the bottom:

```csharp
public class ConfiguracaoEntidade
{
    public int EntidadeId { get; set; }
    public ConfiguracaoGeral Geral { get; set; } = new();
    public ConfiguracaoCobranca Cobranca { get; set; } = new();
    public ConfiguracaoEmail Email { get; set; } = new();
    public ConfiguracaoBanco Banco { get; set; } = new();
}
```

- [ ] **Step 3: Add DbSets and EF config for Group B**

In `BlazorTeste.Api/Data/AppDbContext.cs`, add after the 3 Group A DbSets from Task 1:

```csharp
public DbSet<Negociacao> Negociacoes => Set<Negociacao>();
public DbSet<Evento> Eventos => Set<Evento>();
public DbSet<ConfiguracaoEntidade> Configuracoes => Set<ConfiguracaoEntidade>();
```

In `OnModelCreating`, add after the existing `modelBuilder.Entity<Usuario>` block:

```csharp
modelBuilder.Entity<Negociacao>(b =>
{
    b.OwnsMany(n => n.Parcelas, owned =>
    {
        owned.ToTable("NegociacaoParcelas");
        owned.WithOwner().HasForeignKey("NegociacaoId");
    });
    b.OwnsMany(n => n.CobrancasOriginais, owned =>
    {
        owned.ToTable("NegociacaoCobrancasOriginais");
        owned.WithOwner().HasForeignKey("NegociacaoId");
    });
});

modelBuilder.Entity<Evento>(b =>
{
    b.OwnsMany(e => e.Inscricoes, owned =>
    {
        owned.ToTable("EventoInscricoes");
        owned.WithOwner().HasForeignKey("EventoId");
    });
});

modelBuilder.Entity<ConfiguracaoEntidade>(b =>
{
    b.HasKey(c => c.EntidadeId);
    b.OwnsOne(c => c.Geral, o => o.ToJson());
    b.OwnsOne(c => c.Cobranca, o =>
    {
        o.ToJson();
        o.OwnsMany(cc => cc.Faixas);
    });
    b.OwnsOne(c => c.Email, o => o.ToJson());
    b.OwnsOne(c => c.Banco, o => o.ToJson());
});
```

- [ ] **Step 4: Add Group B seed to SeedData.cs**

In `BlazorTeste.Api/Data/SeedData.cs`, at the bottom of `Initialize()` (after the `Relatorios` block from Task 1), add:

```csharp
if (!db.Negociacoes.Any() && db.Contribuintes.Any())
{
    var entidade1 = db.Entidades.OrderBy(e => e.Id).First();
    var contribs = db.Contribuintes.Where(c => c.EntidadeId == entidade1.Id).OrderBy(c => c.Id).Take(6).ToList();
    if (contribs.Count >= 4)
    {
        db.Negociacoes.AddRange(
            new Negociacao
            {
                EntidadeId = entidade1.Id, ContribuinteId = contribs[0].Id, RazaoSocialContribuinte = contribs[0].RazaoSocial,
                DataAbertura = new DateTime(2026,3,10), DebitoOriginal = 8500m, Desconto = 850m, ValorNegociado = 7650m,
                NumeroParcelas = 6, Status = StatusNegociacao.EmAndamento, Observacoes = "Negociação realizada em 10/03/2026.",
                CobrancasOriginais = new()
                {
                    new() { Tipo = "Sindical", AnoReferencia = 2024, ValorOriginal = 3200m, Multa = 64m, Juros = 105.6m },
                    new() { Tipo = "Associativa", AnoReferencia = 2024, ValorOriginal = 1800m, Multa = 36m, Juros = 59.4m },
                    new() { Tipo = "Sindical", AnoReferencia = 2023, ValorOriginal = 2800m, Multa = 56m, Juros = 92.4m }
                },
                Parcelas = new()
                {
                    new() { Numero = 1, Valor = 1275m, Vencimento = new DateTime(2026,4,10), Pago = true, DataPagamento = new DateTime(2026,4,8) },
                    new() { Numero = 2, Valor = 1275m, Vencimento = new DateTime(2026,5,10), Pago = true, DataPagamento = new DateTime(2026,5,9) },
                    new() { Numero = 3, Valor = 1275m, Vencimento = new DateTime(2026,6,10), Pago = false },
                    new() { Numero = 4, Valor = 1275m, Vencimento = new DateTime(2026,7,10), Pago = false },
                    new() { Numero = 5, Valor = 1275m, Vencimento = new DateTime(2026,8,10), Pago = false },
                    new() { Numero = 6, Valor = 1275m, Vencimento = new DateTime(2026,9,10), Pago = false }
                }
            },
            new Negociacao
            {
                EntidadeId = entidade1.Id, ContribuinteId = contribs[1].Id, RazaoSocialContribuinte = contribs[1].RazaoSocial,
                DataAbertura = new DateTime(2025,11,5), DataConclusao = new DateTime(2026,5,5),
                DebitoOriginal = 4200m, Desconto = 420m, ValorNegociado = 3780m,
                NumeroParcelas = 3, Status = StatusNegociacao.Concluida, Observacoes = "Negociação concluída com sucesso.",
                CobrancasOriginais = new()
                {
                    new() { Tipo = "Sindical", AnoReferencia = 2024, ValorOriginal = 2200m, Multa = 44m, Juros = 72.6m },
                    new() { Tipo = "Confederativa", AnoReferencia = 2024, ValorOriginal = 1200m, Multa = 24m, Juros = 39.6m }
                },
                Parcelas = new()
                {
                    new() { Numero = 1, Valor = 1260m, Vencimento = new DateTime(2025,12,5), Pago = true, DataPagamento = new DateTime(2025,12,3) },
                    new() { Numero = 2, Valor = 1260m, Vencimento = new DateTime(2026,1,5), Pago = true, DataPagamento = new DateTime(2026,1,4) },
                    new() { Numero = 3, Valor = 1260m, Vencimento = new DateTime(2026,2,5), Pago = true, DataPagamento = new DateTime(2026,2,5) }
                }
            },
            new Negociacao
            {
                EntidadeId = entidade1.Id, ContribuinteId = contribs[2].Id, RazaoSocialContribuinte = contribs[2].RazaoSocial,
                DataAbertura = new DateTime(2026,1,20), DebitoOriginal = 15000m, Desconto = 1500m, ValorNegociado = 13500m,
                NumeroParcelas = 12, Status = StatusNegociacao.Inadimplente, Observacoes = "Parcelas 3 e 4 em atraso.",
                CobrancasOriginais = new()
                {
                    new() { Tipo = "Sindical", AnoReferencia = 2024, ValorOriginal = 6500m, Multa = 130m, Juros = 214.5m },
                    new() { Tipo = "Sindical", AnoReferencia = 2023, ValorOriginal = 5800m, Multa = 116m, Juros = 191.4m },
                    new() { Tipo = "Associativa", AnoReferencia = 2024, ValorOriginal = 1800m, Multa = 36m, Juros = 59.4m }
                },
                Parcelas = Enumerable.Range(1, 12).Select(n => new ParcelaNegociacao
                {
                    Numero = n, Valor = 1125m,
                    Vencimento = new DateTime(2026, 2, 20).AddMonths(n - 1),
                    Pago = n <= 2,
                    DataPagamento = n == 1 ? new DateTime(2026,2,18) : n == 2 ? new DateTime(2026,3,19) : null
                }).ToList()
            },
            new Negociacao
            {
                EntidadeId = entidade1.Id, ContribuinteId = contribs[3].Id, RazaoSocialContribuinte = contribs[3].RazaoSocial,
                DataAbertura = new DateTime(2026,2,1), DebitoOriginal = 3100m, Desconto = 0m, ValorNegociado = 3100m,
                NumeroParcelas = 2, Status = StatusNegociacao.Cancelada, Observacoes = "Contribuinte desistiu da negociação.",
                CobrancasOriginais = new()
                {
                    new() { Tipo = "Associativa", AnoReferencia = 2025, ValorOriginal = 1600m, Multa = 32m, Juros = 52.8m },
                    new() { Tipo = "Confederativa", AnoReferencia = 2025, ValorOriginal = 1200m, Multa = 24m, Juros = 39.6m }
                },
                Parcelas = new()
                {
                    new() { Numero = 1, Valor = 1550m, Vencimento = new DateTime(2026,3,1), Pago = false },
                    new() { Numero = 2, Valor = 1550m, Vencimento = new DateTime(2026,4,1), Pago = false }
                }
            }
        );
        db.SaveChanges();
    }
}

if (!db.Eventos.Any() && db.Entidades.Any())
{
    var entidade1 = db.Entidades.OrderBy(e => e.Id).First();
    db.Eventos.AddRange(
        new Evento
        {
            EntidadeId = entidade1.Id, Nome = "Congresso Nacional de Hotelaria 2026",
            Descricao = "O maior evento do setor de hotelaria do Brasil, reunindo executivos e gestores de todo o país.",
            DataInicio = new DateTime(2026,9,15), DataFim = new DateTime(2026,9,17),
            Local = "Centro de Convenções SP", Cidade = "São Paulo", Estado = "SP",
            MaxParticipantes = 400, TotalInscritos = 187, TaxaInscricao = 890m, Status = StatusEvento.Aberto,
            Inscricoes = new()
            {
                new() { NomeParticipante = "Carlos Silva", Email = "carlos@hotelpaulista.com.br", Empresa = "Hotel Paulista S/A", Cargo = "Diretor", DataInscricao = new DateTime(2026,6,1), Pago = true, Presente = false, TipoHospedagem = "Individual" },
                new() { NomeParticipante = "Ana Costa", Email = "ana@granvia.com.br", Empresa = "Hotel Gran Via S/A", Cargo = "Gerente", DataInscricao = new DateTime(2026,6,5), Pago = true, Presente = false, TipoHospedagem = "Duplo" },
                new() { NomeParticipante = "Paulo Mendes", Email = "paulo@solmar.com.br", Empresa = "Resort Sol & Mar", Cargo = "Sócio", DataInscricao = new DateTime(2026,6,10), Pago = false, Presente = false, TipoHospedagem = "Sem hospedagem" }
            }
        },
        new Evento
        {
            EntidadeId = entidade1.Id, Nome = "Seminário Gestão Sindical 2026",
            Descricao = "Capacitação em gestão para dirigentes e colaboradores de entidades sindicais patronais.",
            DataInicio = new DateTime(2026,8,20), DataFim = new DateTime(2026,8,21),
            Local = "Hotel Blue Tree Premium", Cidade = "São Paulo", Estado = "SP",
            MaxParticipantes = 80, TotalInscritos = 72, TaxaInscricao = 350m, Status = StatusEvento.Fechado,
            Inscricoes = new()
            {
                new() { NomeParticipante = "Maria Santos", Email = "maria@sindhosp.org.br", Empresa = "SINDHOSP", Cargo = "Diretora", DataInscricao = new DateTime(2026,7,1), Pago = true, Presente = false, TipoHospedagem = "Individual" },
                new() { NomeParticipante = "João Lima", Email = "joao@sindhosp.org.br", Empresa = "SINDHOSP", Cargo = "Assessor", DataInscricao = new DateTime(2026,7,2), Pago = true, Presente = false, TipoHospedagem = "Sem hospedagem" }
            }
        },
        new Evento
        {
            EntidadeId = entidade1.Id, Nome = "Assembléia Geral Ordinária 2025",
            Descricao = "AGO anual para prestação de contas e eleição de diretoria.",
            DataInicio = new DateTime(2025,11,10), DataFim = new DateTime(2025,11,10),
            Local = "Sede da Entidade", Cidade = "São Paulo", Estado = "SP",
            MaxParticipantes = 200, TotalInscritos = 134, TaxaInscricao = 0m, Status = StatusEvento.Realizado,
            Inscricoes = new()
            {
                new() { NomeParticipante = "Roberto Souza", Email = "roberto@hotelcentral.com.br", Empresa = "Hotel Central Ltda", Cargo = "Sócio", DataInscricao = new DateTime(2025,10,20), Pago = true, Presente = true, TipoHospedagem = "Sem hospedagem" },
                new() { NomeParticipante = "Fernanda Oliveira", Email = "fernanda@buffet.com.br", Empresa = "Buffet Bom Sabor S/A", Cargo = "Representante", DataInscricao = new DateTime(2025,10,22), Pago = true, Presente = false, TipoHospedagem = "Sem hospedagem" }
            }
        },
        new Evento
        {
            EntidadeId = entidade1.Id, Nome = "Workshop Sustentabilidade no Turismo",
            Descricao = "Práticas sustentáveis para hotéis, restaurantes e meios de hospedagem.",
            DataInicio = new DateTime(2026,10,5), DataFim = new DateTime(2026,10,5),
            Local = "WTC São Paulo", Cidade = "São Paulo", Estado = "SP",
            MaxParticipantes = 120, TotalInscritos = 45, TaxaInscricao = 220m, Status = StatusEvento.Aberto,
            Inscricoes = new()
        }
    );
    db.SaveChanges();
}

if (!db.Configuracoes.Any() && db.Entidades.Any())
{
    foreach (var entidade in db.Entidades.ToList())
    {
        db.Configuracoes.Add(new ConfiguracaoEntidade
        {
            EntidadeId = entidade.Id,
            Geral = new ConfiguracaoGeral
            {
                EntidadeId = entidade.Id,
                ResponsavelTecnico = "João da Silva",
                EmailContato = $"contato@{entidade.Sigla.ToLower()}.org.br",
                Telefone = "(11) 3000-0000",
                Endereco = "Av. Paulista, 1000 - Bela Vista, São Paulo/SP"
            },
            Cobranca = new ConfiguracaoCobranca
            {
                EntidadeId = entidade.Id,
                PercentualMulta = 2m,
                PercentualJurosDia = 0.033m,
                DiasCarencia = 5,
                EmissaoAutomatica = true,
                DiaVencimento = 30,
                Faixas = new List<FaixaCobranca>
                {
                    new() { Id = 1, Descricao = "Microempresa", CapitalSocialMin = 0, CapitalSocialMax = 360000, ValorFixo = 250, PercentualCapital = 0 },
                    new() { Id = 2, Descricao = "Pequeno Porte", CapitalSocialMin = 360001, CapitalSocialMax = 1000000, ValorFixo = 0, PercentualCapital = 0.1m },
                    new() { Id = 3, Descricao = "Médio Porte", CapitalSocialMin = 1000001, CapitalSocialMax = 5000000, ValorFixo = 0, PercentualCapital = 0.08m },
                    new() { Id = 4, Descricao = "Grande Porte", CapitalSocialMin = 5000001, CapitalSocialMax = decimal.MaxValue, ValorFixo = 0, PercentualCapital = 0.06m }
                }
            },
            Email = new ConfiguracaoEmail
            {
                ServidorSmtp = $"smtp.{entidade.Sigla.ToLower()}.org.br",
                Porta = 587,
                UsarSsl = true,
                EmailRemetente = $"noreply@{entidade.Sigla.ToLower()}.org.br",
                NomeRemetente = "SindERP",
                Ativo = true
            },
            Banco = new ConfiguracaoBanco
            {
                Banco = "001 - Banco do Brasil",
                Agencia = "0001-9",
                Conta = "00001234-0",
                Cedente = entidade.Sigla,
                CodigoCedente = "123456",
                Carteira = "17"
            }
        });
    }
    db.SaveChanges();
}
```

- [ ] **Step 5: Create NegociacoesController**

Create `BlazorTeste.Api/Controllers/NegociacoesController.cs`:

```csharp
using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NegociacoesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<Negociacao>> GetAll([FromQuery] int? entidadeId = null)
    {
        var query = db.Negociacoes
            .Include(n => n.Parcelas)
            .Include(n => n.CobrancasOriginais)
            .AsQueryable();
        if (entidadeId.HasValue) query = query.Where(n => n.EntidadeId == entidadeId.Value);
        return await query.ToListAsync();
    }
}
```

- [ ] **Step 6: Create EventosController**

Create `BlazorTeste.Api/Controllers/EventosController.cs`:

```csharp
using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<Evento>> GetAll([FromQuery] int? entidadeId = null)
    {
        var query = db.Eventos
            .Include(e => e.Inscricoes)
            .AsQueryable();
        if (entidadeId.HasValue) query = query.Where(e => e.EntidadeId == entidadeId.Value);
        return await query.ToListAsync();
    }
}
```

- [ ] **Step 7: Create ConfiguracoesController**

Create `BlazorTeste.Api/Controllers/ConfiguracoesController.cs`:

```csharp
using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConfiguracoesController(AppDbContext db) : ControllerBase
{
    [HttpGet("geral")]
    public async Task<ActionResult<ConfiguracaoGeral>> GetGeral([FromQuery] int entidadeId)
    {
        var config = await db.Configuracoes.FirstOrDefaultAsync(c => c.EntidadeId == entidadeId);
        if (config is null) return NotFound();
        return config.Geral;
    }

    [HttpGet("cobranca")]
    public async Task<ActionResult<ConfiguracaoCobranca>> GetCobranca([FromQuery] int entidadeId)
    {
        var config = await db.Configuracoes.FirstOrDefaultAsync(c => c.EntidadeId == entidadeId);
        if (config is null) return NotFound();
        return config.Cobranca;
    }

    [HttpGet("email")]
    public async Task<ActionResult<ConfiguracaoEmail>> GetEmail([FromQuery] int entidadeId)
    {
        var config = await db.Configuracoes.FirstOrDefaultAsync(c => c.EntidadeId == entidadeId);
        if (config is null) return NotFound();
        return config.Email;
    }

    [HttpGet("banco")]
    public async Task<ActionResult<ConfiguracaoBanco>> GetBanco([FromQuery] int entidadeId)
    {
        var config = await db.Configuracoes.FirstOrDefaultAsync(c => c.EntidadeId == entidadeId);
        if (config is null) return NotFound();
        return config.Banco;
    }
}
```

- [ ] **Step 8: Create migration**

Run from `C:\Users\yrodrigues\source\repos\BlazorTeste\BlazorTeste.Api`:

```
dotnet ef migrations add AddGroupBEntities
```

Expected: creates migration with `CreateTable` for `Negociacoes`, `NegociacaoParcelas`, `NegociacaoCobrancasOriginais`, `Eventos`, `EventoInscricoes`, `Configuracoes`.

- [ ] **Step 9: Build to verify 0 errors**

Run from `C:\Users\yrodrigues\source\repos\BlazorTeste`:

```
dotnet build
```

Expected: `Build succeeded. 0 Warning(s). 0 Error(s)`

- [ ] **Step 10: Commit**

```
git -C "C:\Users\yrodrigues\source\repos\BlazorTeste" add -A
git -C "C:\Users\yrodrigues\source\repos\BlazorTeste" commit -m "feat: add Negociacao, Evento, Configuracao API endpoints"
```

---

