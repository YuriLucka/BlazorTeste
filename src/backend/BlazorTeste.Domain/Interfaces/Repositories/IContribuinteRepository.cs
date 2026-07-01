using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IContribuinteRepository : IRepository<Contribuinte>
{
    Task<IReadOnlyList<Contribuinte>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
}
