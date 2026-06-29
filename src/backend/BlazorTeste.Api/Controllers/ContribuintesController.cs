using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContribuintesController(IContribuinteAppService service) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ContribuinteDto>> GetAll([FromQuery] int entidadeId) =>
        await service.GetAllAsync(entidadeId);

    [HttpGet("{id}")]
    public async Task<ActionResult<ContribuinteDto>> GetById(int id)
    {
        var c = await service.GetByIdAsync(id);
        return c is null ? NotFound() : Ok(c);
    }
}
