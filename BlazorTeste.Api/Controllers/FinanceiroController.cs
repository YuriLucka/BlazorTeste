using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/financeiro")]
public class FinanceiroController(AppDbContext db) : ControllerBase
{
    [HttpGet("lancamentos")]
    public async Task<List<LancamentoFinanceiro>> GetLancamentos() =>
        await db.LancamentosFinanceiros.ToListAsync();

    [HttpGet("fornecedores")]
    public async Task<List<Fornecedor>> GetFornecedores() =>
        await db.Fornecedores.ToListAsync();
}
