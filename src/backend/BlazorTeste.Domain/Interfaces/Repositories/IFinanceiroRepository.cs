using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IFinanceiroRepository : IRepository<LancamentoFinanceiro>
{
    Task<IReadOnlyList<LancamentoFinanceiro>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LancamentoFinanceiro>> GetByTipoAsync(TipoLancamento tipo, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LancamentoFinanceiro>> GetByPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Fornecedor>> GetFornecedoresAsync(int entidadeId, CancellationToken cancellationToken = default);
}
