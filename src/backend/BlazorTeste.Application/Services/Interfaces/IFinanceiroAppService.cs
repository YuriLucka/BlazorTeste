using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IFinanceiroAppService
{
    Task<IEnumerable<LancamentoFinanceiroDto>> GetLancamentosAsync(int entidadeId);
    Task<IEnumerable<FornecedorDto>> GetFornecedoresAsync();
    Task<DashboardDto> GetDashboardAsync(int entidadeId);
}
