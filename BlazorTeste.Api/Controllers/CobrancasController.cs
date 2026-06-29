using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CobrancasController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<Cobranca>> GetAll(
        [FromQuery] StatusCobranca? status = null,
        [FromQuery] int? contribuinteId = null)
    {
        var query = db.Cobrancas.AsQueryable();
        if (status.HasValue) query = query.Where(c => c.Status == status.Value);
        if (contribuinteId.HasValue) query = query.Where(c => c.ContribuinteId == contribuinteId.Value);
        return await query.ToListAsync();
    }
}
