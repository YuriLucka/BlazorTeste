using BlazorTeste.Application.Security;
using BlazorTeste.Domain.Interfaces.Repositories;
using BlazorTeste.Infrastructure.Data;
using BlazorTeste.Infrastructure.Repositories;
using BlazorTeste.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTeste.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

        services.AddScoped<IContribuinteRepository, ContribuinteRepository>();
        services.AddScoped<ICobrancaRepository, CobrancaRepository>();
        services.AddScoped<IJuridicoRepository, JuridicoRepository>();
        services.AddScoped<IFinanceiroRepository, FinanceiroRepository>();
        services.AddScoped<IEventoRepository, EventoRepository>();
        services.AddScoped<IGuiaSindicalRepository, GuiaSindicalRepository>();
        services.AddScoped<INegociacaoRepository, NegociacaoRepository>();
        services.AddScoped<IBaixaCobrancaRepository, BaixaCobrancaRepository>();
        services.AddScoped<IEntidadeRepository, EntidadeRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICampanhaRepository, CampanhaRepository>();
        services.AddScoped<IConfiguracaoRepository, ConfiguracaoRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
