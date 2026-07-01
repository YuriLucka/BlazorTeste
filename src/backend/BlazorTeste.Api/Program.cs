using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorTeste.Api.Filters;
using BlazorTeste.Api.Validators;
using BlazorTeste.Application.Security;
using BlazorTeste.Application.Services.Implementations;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Application.Validators;
using BlazorTeste.Infrastructure;
using BlazorTeste.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

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
builder.Services.AddScoped<IAuthAppService, AuthAppService>();
builder.Services.AddScoped<IUsuarioAppService, UsuarioAppService>();
builder.Services.AddScoped<IMailingAppService, MailingAppService>();

builder.Services.AddValidatorsFromAssemblyContaining<GerarRelatorioRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

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
            "https://localhost:7130",
            "http://192.168.10.104:5233",
            "https://192.168.10.104:7130")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()));

builder.Services.AddControllers(opt => opt.Filters.Add<FluentValidationFilter>()).AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

builder.Services.AddOpenApi(opt =>
{
    opt.AddDocumentTransformer((doc, _, _) =>
    {
        doc.Info.Title = "SindERP API";
        doc.Info.Version = "v1";
        doc.Info.Description = "API do Sistema de Gestão Sindical SindERP";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    SeedData.Initialize(db, hasher);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.Title = "SindERP API";
        opt.Theme = ScalarTheme.BluePlanet;
        opt.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
