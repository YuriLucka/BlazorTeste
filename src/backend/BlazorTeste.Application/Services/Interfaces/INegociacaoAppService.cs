using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface INegociacaoAppService
{
    Task<IEnumerable<NegociacaoDto>> GetAllAsync(int entidadeId);
}
