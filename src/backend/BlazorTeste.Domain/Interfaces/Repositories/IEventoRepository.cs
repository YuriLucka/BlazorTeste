using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IEventoRepository : IRepository<Evento>
{
    Task<IReadOnlyList<Evento>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
}
