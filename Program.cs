using BlazorTeste.Components;
using BlazorTeste.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddSingleton<AppStateService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5141")
});

builder.Services.AddScoped<EntidadeService>();
builder.Services.AddScoped<ContribuinteService>();
builder.Services.AddScoped<CobrancaService>();
builder.Services.AddScoped<JuridicoService>();
builder.Services.AddScoped<FinanceiroService>();
builder.Services.AddScoped<MailingService>();
builder.Services.AddScoped<UsuarioService>();

await builder.Build().RunAsync();
