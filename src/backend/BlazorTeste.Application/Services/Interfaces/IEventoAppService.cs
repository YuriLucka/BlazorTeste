using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IEventoAppService
{
    Task<IEnumerable<EventoDto>> GetAllAsync(int entidadeId);
}
