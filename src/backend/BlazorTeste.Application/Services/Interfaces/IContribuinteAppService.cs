using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IContribuinteAppService
{
    Task<IEnumerable<ContribuinteDto>> GetAllAsync(int entidadeId);
    Task<ContribuinteDto?> GetByIdAsync(int id);
}
