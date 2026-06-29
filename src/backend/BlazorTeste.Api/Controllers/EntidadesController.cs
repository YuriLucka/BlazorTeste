using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EntidadesController(IEntidadeAppService service) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<EntidadeDto>> GetAll() =>
        await service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<EntidadeDto>> GetById(int id)
    {
        var entity = await service.GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }
}
