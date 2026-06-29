using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IGuiaSindicalAppService
{
    Task<IEnumerable<GuiaSindicalDto>> GetAllAsync(int entidadeId);
}
