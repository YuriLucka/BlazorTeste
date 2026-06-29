using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IJuridicoAppService
{
    Task<IEnumerable<ProcessoDto>> GetProcessosAsync(int entidadeId);
    Task<IEnumerable<AdvogadoDto>> GetAdvogadosAsync(int entidadeId);
    Task<IEnumerable<AudienciaDto>> GetAudienciasAsync();
}
