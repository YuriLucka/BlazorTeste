using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IConfiguracaoRepository
{
    Task<ConfiguracaoEntidade?> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task UpdateAsync(ConfiguracaoEntidade entity, CancellationToken cancellationToken = default);
}
