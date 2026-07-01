using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class BaixaCobrancaRepository : BaseRepository<RegistroBaixa>, IBaixaCobrancaRepository
{
    public BaixaCobrancaRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<RegistroBaixa>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(r => r.EntidadeId == entidadeId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RegistroBaixa>> GetByCobrancaAsync(int cobrancaId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(r => r.CobrancaId == cobrancaId)
            .ToListAsync(cancellationToken);
}
