using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IGuiaSindicalRepository : IRepository<GuiaSindical>
{
    Task<IReadOnlyList<GuiaSindical>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GuiaSindical>> GetByContribuinteAsync(int contribuinteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GuiaSindical>> GetByStatusAsync(StatusGuia status, CancellationToken cancellationToken = default);
}
