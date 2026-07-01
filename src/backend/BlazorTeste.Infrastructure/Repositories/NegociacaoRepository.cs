using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class NegociacaoRepository : BaseRepository<Negociacao>, INegociacaoRepository
{
    public NegociacaoRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Negociacao>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(n => n.EntidadeId == entidadeId)
            .ToListAsync(cancellationToken);
}
