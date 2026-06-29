using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContribuintesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<Contribuinte>> GetAll([FromQuery] int? entidadeId = null)
    {
        var query = db.Contribuintes
            .Include(c => c.Enderecos)
            .Include(c => c.Contatos)
            .Include(c => c.Socios)
            .Include(c => c.Historico)
            .AsQueryable();
        if (entidadeId.HasValue)
            query = query.Where(c => c.EntidadeId == entidadeId.Value);
        return await query.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Contribuinte>> GetById(int id)
    {
        var c = await db.Contribuintes
            .Include(c => c.Enderecos)
            .Include(c => c.Contatos)
            .Include(c => c.Socios)
            .Include(c => c.Historico)
            .FirstOrDefaultAsync(c => c.Id == id);
        return c is null ? NotFound() : Ok(c);
    }
}
