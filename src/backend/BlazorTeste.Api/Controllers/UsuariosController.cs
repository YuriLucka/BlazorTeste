using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsuariosController(IUsuarioAppService service) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<UsuarioDto>> GetAll() =>
        await service.GetAllAsync();

    [HttpGet("by-email")]
    public async Task<ActionResult<UsuarioDto>> GetByEmail([FromQuery] string email)
    {
        var u = await service.GetByEmailAsync(email);
        return u is null ? NotFound() : Ok(u);
    }
}
