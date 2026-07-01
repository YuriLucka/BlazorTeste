using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class CobrancaRepository : BaseRepository<Cobranca>, ICobrancaRepository
{
    public CobrancaRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Cobranca>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(c => c.EntidadeId == entidadeId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Cobranca>> GetByStatusAsync(StatusCobranca status, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(c => c.Status == status)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Cobranca>> GetVencidasAsync(CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(c => c.Status == StatusCobranca.Pendente && c.Vencimento < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
}
