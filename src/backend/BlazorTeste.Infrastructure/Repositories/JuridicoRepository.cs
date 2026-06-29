using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class JuridicoRepository : BaseRepository<Processo>, IJuridicoRepository
{
    public JuridicoRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Processo>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(p => p.EntidadeId == entidadeId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Advogado>> GetAdvogadosAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _context.Advogados
            .Where(a => a.EntidadeId == entidadeId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Advogado>> GetAllAdvogadosAsync(CancellationToken cancellationToken = default)
        => await _context.Advogados.ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Audiencia>> GetAudienciasByProcessoAsync(int processoId, CancellationToken cancellationToken = default)
        => await _context.Audiencias
            .Where(a => a.ProcessoId == processoId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Audiencia>> GetAudienciasPorPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default)
        => await _context.Audiencias
            .Where(a => a.DataHora >= inicio && a.DataHora <= fim)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Audiencia>> GetAllAudienciasAsync(CancellationToken cancellationToken = default)
        => await _context.Audiencias.ToListAsync(cancellationToken);
}
