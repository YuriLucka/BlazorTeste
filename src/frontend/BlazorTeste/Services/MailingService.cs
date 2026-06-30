using BlazorTeste.Models;
using System.Net.Http.Json;

namespace BlazorTeste.Services;

public class MailingService(HttpClient http)
{
    public async Task<List<Campanha>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Campanha>>("api/mailing", ApiJsonOptions.Default) ?? new();
}
