using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class GuiaSindicalRepository : BaseRepository<GuiaSindical>, IGuiaSindicalRepository
{
    public GuiaSindicalRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<GuiaSindical>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(g => g.EntidadeId == entidadeId)
            .ToListAsync(cancellationToken);
}
