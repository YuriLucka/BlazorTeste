using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class ConfiguracaoRepository : IConfiguracaoRepository
{
    private readonly AppDbContext _context;

    public ConfiguracaoRepository(AppDbContext context) => _context = context;

    public async Task<ConfiguracaoEntidade?> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default)
        => await _context.Configuracoes.FirstOrDefaultAsync(c => c.EntidadeId == entidadeId, cancellationToken);

    public async Task UpdateAsync(ConfiguracaoEntidade entity, CancellationToken cancellationToken = default)
    {
        _context.Configuracoes.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
