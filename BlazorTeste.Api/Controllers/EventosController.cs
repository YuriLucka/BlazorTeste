using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<Evento>> GetAll([FromQuery] int? entidadeId = null)
    {
        var query = db.Eventos
            .Include(e => e.Inscricoes)
            .AsQueryable();
        if (entidadeId.HasValue) query = query.Where(e => e.EntidadeId == entidadeId.Value);
        return await query.ToListAsync();
    }
}
