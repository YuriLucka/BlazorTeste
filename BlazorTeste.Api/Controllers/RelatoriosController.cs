using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelatoriosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<Relatorio>> GetAll() =>
        await db.Relatorios.OrderBy(r => r.Categoria).ThenBy(r => r.Nome).ToListAsync();
}
