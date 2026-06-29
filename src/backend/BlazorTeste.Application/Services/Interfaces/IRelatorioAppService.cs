using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IRelatorioAppService
{
    Task<IEnumerable<RelatorioDto>> GetAllAsync(int entidadeId);
}
