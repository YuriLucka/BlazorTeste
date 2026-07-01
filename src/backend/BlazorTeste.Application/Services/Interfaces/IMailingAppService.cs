using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IMailingAppService
{
    Task<IEnumerable<CampanhaDto>> GetAllAsync();
}
