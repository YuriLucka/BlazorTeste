using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IJuridicoRepository : IRepository<Processo>
{
    Task<IReadOnlyList<Processo>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Advogado>> GetAdvogadosAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Advogado>> GetAllAdvogadosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Audiencia>> GetAudienciasByProcessoAsync(int processoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Audiencia>> GetAudienciasPorPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Audiencia>> GetAllAudienciasAsync(CancellationToken cancellationToken = default);
}
