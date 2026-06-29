using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using BlazorTeste.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConfiguracoesController(IConfiguracaoAppService service, AppDbContext db) : ControllerBase
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
        var dto = await service.GetBancoAsync(entidadeId);
        if (dto is null) return NotFound();
        return Ok(new ConfiguracaoBanco
        {
            Banco = dto.Banco,
            Agencia = dto.Agencia,
            Conta = dto.Conta,
            Cedente = dto.Cedente,
            CodigoCedente = dto.CodigoCedente,
            Carteira = dto.Carteira
        });
    }
}
