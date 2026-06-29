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
