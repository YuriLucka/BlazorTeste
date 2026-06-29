using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IEntidadeRepository : IRepository<Entidade>
{
    Task<Entidade?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
}
