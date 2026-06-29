using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IEntidadeAppService
{
    Task<IEnumerable<EntidadeDto>> GetAllAsync();
    Task<EntidadeDto?> GetByIdAsync(int id);
}
