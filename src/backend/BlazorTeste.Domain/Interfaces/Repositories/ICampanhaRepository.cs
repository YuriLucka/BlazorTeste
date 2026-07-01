using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface ICampanhaRepository : IRepository<Campanha>
{
    Task<IReadOnlyList<Campanha>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
}
