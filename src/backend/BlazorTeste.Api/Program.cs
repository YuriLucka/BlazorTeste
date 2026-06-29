using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorTeste.Api.Data;
using BlazorTeste.Application.Services.Implementations;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Infrastructure;
using BlazorTeste.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddInfrastructure(connectionString);

builder.Services.AddScoped<IContribuinteAppService, ContribuinteAppService>();
builder.Services.AddScoped<ICobrancaAppService, CobrancaAppService>();
builder.Services.AddScoped<IJuridicoAppService, JuridicoAppService>();
builder.Services.AddScoped<IFinanceiroAppService, FinanceiroAppService>();
builder.Services.AddScoped<IEntidadeAppService, EntidadeAppService>();
builder.Services.AddScoped<IEventoAppService, EventoAppService>();
builder.Services.AddScoped<IGuiaSindicalAppService, GuiaSindicalAppService>();
builder.Services.AddScoped<INegociacaoAppService, NegociacaoAppService>();
builder.Services.AddScoped<IBaixaCobrancaAppService, BaixaCobrancaAppService>();
builder.Services.AddScoped<IRelatorioAppService, RelatorioAppService>();
builder.Services.AddScoped<IConfiguracaoAppService, ConfiguracaoAppService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p
        .WithOrigins(
            "http://localhost:5233",
            "https://localhost:7130")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()));

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(db);
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
