using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface INegociacaoRepository : IRepository<Negociacao>
{
    Task<IReadOnlyList<Negociacao>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
}
