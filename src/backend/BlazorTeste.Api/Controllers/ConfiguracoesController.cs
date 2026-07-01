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
