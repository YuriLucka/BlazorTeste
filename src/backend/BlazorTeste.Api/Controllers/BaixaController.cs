using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BaixaController(IBaixaCobrancaAppService service) : ControllerBase
{
    [HttpGet("historico")]
    public async Task<IEnumerable<CobrancaDto>> GetHistorico([FromQuery] int entidadeId) =>
        await service.GetVencidasAsync(entidadeId);

    [HttpPost("{cobrancaId}/baixar")]
    public async Task<ActionResult<BaixaCobrancaDto>> Baixar(int cobrancaId, [FromQuery] string tipo)
    {
        var result = await service.BaixarAsync(cobrancaId, tipo);
        return Ok(result);
    }

    [HttpPost("processar-arquivo")]
    public async Task<ProcessarArquivoDto> ProcessarArquivo([FromQuery] int entidadeId) =>
        await service.ProcessarArquivoAsync(entidadeId);
}
