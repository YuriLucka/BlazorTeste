using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface INegociacaoRepository : IRepository<Negociacao>
{
    Task<IReadOnlyList<Negociacao>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Negociacao>> GetByContribuinteAsync(int contribuinteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Negociacao>> GetByStatusAsync(StatusNegociacao status, CancellationToken cancellationToken = default);
}
