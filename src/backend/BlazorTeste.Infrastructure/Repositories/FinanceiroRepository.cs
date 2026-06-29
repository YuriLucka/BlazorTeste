using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class FinanceiroRepository : BaseRepository<LancamentoFinanceiro>, IFinanceiroRepository
{
    public FinanceiroRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<LancamentoFinanceiro>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(l => l.EntidadeId == entidadeId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LancamentoFinanceiro>> GetByTipoAsync(TipoLancamento tipo, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(l => l.Tipo == tipo)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LancamentoFinanceiro>> GetByPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(l => l.Data >= inicio && l.Data <= fim)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Fornecedor>> GetFornecedoresAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _context.Fornecedores
            .Where(f => f.EntidadeId == entidadeId)
            .ToListAsync(cancellationToken);
}
