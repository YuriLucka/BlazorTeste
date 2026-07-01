using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class MailingAppService(ICampanhaRepository repo) : IMailingAppService
{
    public async Task<IEnumerable<CampanhaDto>> GetAllAsync()
    {
        var items = await repo.GetAllAsync();
        return items.Select(c => new CampanhaDto
        {
            Id = c.Id,
            EntidadeId = c.EntidadeId,
            Assunto = c.Assunto,
            Destinatarios = c.Destinatarios,
            TotalDestinatarios = c.TotalDestinatarios,
            DataEnvio = c.DataEnvio,
            Status = c.Status.ToString(),
            Criador = c.Criador
        });
    }
}
