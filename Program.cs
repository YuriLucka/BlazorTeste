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
builder.Services.AddSingleton<EntidadeService>();
builder.Services.AddSingleton<ContribuinteService>();
builder.Services.AddSingleton<CobrancaService>();
builder.Services.AddSingleton<JuridicoService>();
builder.Services.AddSingleton<FinanceiroService>();
builder.Services.AddSingleton<MailingService>();
builder.Services.AddSingleton<UsuarioService>();

await builder.Build().RunAsync();
