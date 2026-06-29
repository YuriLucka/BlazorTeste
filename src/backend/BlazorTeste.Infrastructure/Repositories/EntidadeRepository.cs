using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class EntidadeRepository : BaseRepository<Entidade>, IEntidadeRepository
{
    public EntidadeRepository(AppDbContext context) : base(context) { }

    public async Task<Entidade?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
        => await _dbSet
            .FirstOrDefaultAsync(e => e.Cnpj == cnpj, cancellationToken);
}
