using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface ICobrancaRepository : IRepository<Cobranca>
{
    Task<IReadOnlyList<Cobranca>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cobranca>> GetByContribuinteAsync(int contribuinteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cobranca>> GetByStatusAsync(StatusCobranca status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cobranca>> GetVencidasAsync(CancellationToken cancellationToken = default);
}
