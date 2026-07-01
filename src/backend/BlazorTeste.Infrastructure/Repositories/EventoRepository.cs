using BlazorTeste.Domain.Entities;
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
}
