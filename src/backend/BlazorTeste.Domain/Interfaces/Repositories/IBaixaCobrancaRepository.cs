using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IBaixaCobrancaRepository : IRepository<RegistroBaixa>
{
    Task<IReadOnlyList<RegistroBaixa>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroBaixa>> GetByCobrancaAsync(int cobrancaId, CancellationToken cancellationToken = default);
}
