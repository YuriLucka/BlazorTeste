using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MailingController(IMailingAppService service) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<CampanhaDto>> GetAll() =>
        await service.GetAllAsync();
}
