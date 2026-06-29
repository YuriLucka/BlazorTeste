using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class EventoRepository : BaseRepository<Evento>, IEventoRepository
{
    public EventoRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Evento>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(e => e.EntidadeId == entidadeId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Evento>> GetByStatusAsync(StatusEvento status, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(e => e.Status == status)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<InscricaoEvento>> GetInscricoesByEventoAsync(int eventoId, CancellationToken cancellationToken = default)
    {
        var evento = await _dbSet
            .Where(e => e.Id == eventoId)
            .Select(e => e.Inscricoes)
            .FirstOrDefaultAsync(cancellationToken);

        return evento ?? [];
    }
}
