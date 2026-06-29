using BlazorTeste.Domain.Entities;
using BlazorTeste.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MailingController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<Campanha>> GetAll() =>
        await db.Campanhas.ToListAsync();
}
