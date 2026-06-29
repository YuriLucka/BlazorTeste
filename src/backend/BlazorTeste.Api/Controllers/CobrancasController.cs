using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CobrancasController(ICobrancaAppService service) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<CobrancaDto>> GetAll(
        [FromQuery] int entidadeId,
        [FromQuery] string? status = null)
    {
        if (!string.IsNullOrEmpty(status))
            return await service.GetByStatusAsync(entidadeId, status);
        return await service.GetAllAsync(entidadeId);
    }

    [HttpGet("vencidas")]
    public async Task<IEnumerable<CobrancaDto>> GetVencidas([FromQuery] int entidadeId) =>
        await service.GetVencidasAsync(entidadeId);
}
