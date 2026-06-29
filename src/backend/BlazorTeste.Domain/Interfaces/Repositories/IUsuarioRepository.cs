using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Usuario?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
