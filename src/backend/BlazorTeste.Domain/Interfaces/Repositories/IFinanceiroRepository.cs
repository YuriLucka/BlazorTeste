using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IFinanceiroRepository : IRepository<LancamentoFinanceiro>
{
    Task<IReadOnlyList<LancamentoFinanceiro>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Fornecedor>> GetFornecedoresAsync(int entidadeId, CancellationToken cancellationToken = default);
}
