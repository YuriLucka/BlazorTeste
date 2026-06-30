using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelatoriosController(IRelatorioAppService service) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<RelatorioDto>> GetAll([FromQuery] int entidadeId) =>
        await service.GetAllAsync(entidadeId);

    [HttpPost("{id}/gerar")]
    public async Task<GerarRelatorioResultDto> Gerar(int id, [FromBody] GerarRelatorioRequest request) =>
        await service.GerarAsync(id, request);
}
