using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GuiaSindicalController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<GuiaSindical>> GetAll([FromQuery] int? entidadeId = null)
    {
        var query = db.GuiaSindicais.AsQueryable();
        if (entidadeId.HasValue) query = query.Where(g => g.EntidadeId == entidadeId.Value);
        return await query.ToListAsync();
    }
}
