using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IEventoRepository : IRepository<Evento>
{
    Task<IReadOnlyList<Evento>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Evento>> GetByStatusAsync(StatusEvento status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InscricaoEvento>> GetInscricoesByEventoAsync(int eventoId, CancellationToken cancellationToken = default);
}
