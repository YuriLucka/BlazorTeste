using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface ICobrancaAppService
{
    Task<IEnumerable<CobrancaDto>> GetAllAsync(int entidadeId);
    Task<IEnumerable<CobrancaDto>> GetByStatusAsync(int entidadeId, string status);
    Task<IEnumerable<CobrancaDto>> GetVencidasAsync(int entidadeId);
}
