using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Repositories;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context) { }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<Usuario?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        => await _dbSet
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
}
