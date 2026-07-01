using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Domain.Interfaces.Repositories;

public interface IGuiaSindicalRepository : IRepository<GuiaSindical>
{
    Task<IReadOnlyList<GuiaSindical>> GetByEntidadeAsync(int entidadeId, CancellationToken cancellationToken = default);
}
