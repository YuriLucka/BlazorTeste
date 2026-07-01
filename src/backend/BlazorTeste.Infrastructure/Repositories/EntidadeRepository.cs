using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;

namespace BlazorTeste.Infrastructure.Repositories;

public class EntidadeRepository : BaseRepository<Entidade>, IEntidadeRepository
{
    public EntidadeRepository(AppDbContext context) : base(context) { }
}
