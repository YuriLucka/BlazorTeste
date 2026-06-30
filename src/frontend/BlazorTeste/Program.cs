using BlazorTeste.Components;
using BlazorTeste.Services;
using BlazorTeste.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

// Auth services
builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
var apiPort = baseUri.Scheme == "https" ? 7247 : 5141;
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? $"{baseUri.Scheme}://{baseUri.Host}:{apiPort}";

// HttpClient with auth handler (Bearer + credentials:include on every request; auto-refresh on 401)
builder.Services.AddScoped(sp =>
{
    var tokenService = sp.GetRequiredService<TokenService>();
    var handler = new AuthHttpMessageHandler(tokenService, new Uri(apiBaseUrl))
    {
        InnerHandler = new HttpClientHandler()
    };
    return new HttpClient(handler)
    {
        BaseAddress = new Uri(apiBaseUrl)
    };
});

builder.Services.AddSingleton<AppStateService>();
builder.Services.AddScoped<EntidadeService>();
builder.Services.AddScoped<ContribuinteService>();
builder.Services.AddScoped<CobrancaService>();
builder.Services.AddScoped<JuridicoService>();
builder.Services.AddScoped<FinanceiroService>();
builder.Services.AddScoped<MailingService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<GuiaSindicalService>();
builder.Services.AddScoped<NegociacaoService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<ConfiguracaoService>();
builder.Services.AddScoped<BaixaCobrancaService>();

var host = builder.Build();

// Restore session from refresh token cookie before first render
var tokenService = host.Services.GetRequiredService<TokenService>();
using var authClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
await tokenService.InitializeAsync(authClient);

await host.RunAsync();
