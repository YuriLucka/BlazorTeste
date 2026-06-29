using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BaixaController(AppDbContext db) : ControllerBase
{
    [HttpGet("historico")]
    public async Task<List<RegistroBaixa>> GetHistorico([FromQuery] int? entidadeId = null)
    {
        var query = db.RegistrosBaixa.AsQueryable();
        if (entidadeId.HasValue) query = query.Where(r => r.EntidadeId == entidadeId.Value);
        return await query.OrderByDescending(r => r.DataProcessamento).ToListAsync();
    }
}
