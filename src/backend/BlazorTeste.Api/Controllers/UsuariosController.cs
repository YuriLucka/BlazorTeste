using BlazorTeste.Domain.Entities;
using BlazorTeste.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsuariosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<Usuario>> GetAll() =>
        await db.Usuarios.ToListAsync();

    [HttpGet("by-email")]
    public async Task<ActionResult<Usuario>> GetByEmail([FromQuery] string email)
    {
        var u = await db.Usuarios.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == email.ToLower());
        return u is null ? NotFound() : Ok(u);
    }
}
