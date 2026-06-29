using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NegociacoesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<Negociacao>> GetAll([FromQuery] int? entidadeId = null)
    {
        var query = db.Negociacoes
            .Include(n => n.Parcelas)
            .Include(n => n.CobrancasOriginais)
            .AsQueryable();
        if (entidadeId.HasValue) query = query.Where(n => n.EntidadeId == entidadeId.Value);
        return await query.ToListAsync();
    }
}
