using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/juridico")]
public class JuridicoController(IJuridicoAppService service) : ControllerBase
{
    [HttpGet("processos")]
    public async Task<IEnumerable<ProcessoDto>> GetProcessos([FromQuery] int entidadeId) =>
        await service.GetProcessosAsync(entidadeId);

    [HttpGet("advogados")]
    public async Task<IEnumerable<AdvogadoDto>> GetAdvogados([FromQuery] int entidadeId) =>
        await service.GetAdvogadosAsync(entidadeId);

    [HttpGet("audiencias")]
    public async Task<IEnumerable<AudienciaDto>> GetAudiencias() =>
        await service.GetAudienciasAsync();
}
