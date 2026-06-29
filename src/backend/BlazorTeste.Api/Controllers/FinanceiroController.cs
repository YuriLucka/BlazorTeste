using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/financeiro")]
public class FinanceiroController(IFinanceiroAppService service) : ControllerBase
{
    [HttpGet("lancamentos")]
    public async Task<IEnumerable<LancamentoFinanceiroDto>> GetLancamentos([FromQuery] int entidadeId) =>
        await service.GetLancamentosAsync(entidadeId);

    [HttpGet("fornecedores")]
    public async Task<IEnumerable<FornecedorDto>> GetFornecedores() =>
        await service.GetFornecedoresAsync();

    [HttpGet("dashboard")]
    public async Task<DashboardDto> GetDashboard([FromQuery] int entidadeId) =>
        await service.GetDashboardAsync(entidadeId);
}
