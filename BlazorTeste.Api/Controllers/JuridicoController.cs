using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/juridico")]
public class JuridicoController(AppDbContext db) : ControllerBase
{
    [HttpGet("processos")]
    public async Task<List<Processo>> GetProcessos() =>
        await db.Processos.ToListAsync();

    [HttpGet("advogados")]
    public async Task<List<Advogado>> GetAdvogados() =>
        await db.Advogados.ToListAsync();

    [HttpGet("audiencias")]
    public async Task<List<Audiencia>> GetAudiencias() =>
        await db.Audiencias.ToListAsync();
}
