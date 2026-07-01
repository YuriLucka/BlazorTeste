using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class ContribuinteRepository : BaseRepository<Contribuinte>, IContribuinteRepository
{
    public ContribuinteRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Contribuinte>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(c => c.EntidadeId == entidadeId)
            .ToListAsync(cancellationToken);
}
