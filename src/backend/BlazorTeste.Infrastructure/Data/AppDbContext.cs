using BlazorTeste.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Entidade> Entidades => Set<Entidade>();
    public DbSet<Contribuinte> Contribuintes => Set<Contribuinte>();
    public DbSet<Cobranca> Cobrancas => Set<Cobranca>();
    public DbSet<LancamentoFinanceiro> LancamentosFinanceiros => Set<LancamentoFinanceiro>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Processo> Processos => Set<Processo>();
    public DbSet<Advogado> Advogados => Set<Advogado>();
    public DbSet<Audiencia> Audiencias => Set<Audiencia>();
    public DbSet<Campanha> Campanhas => Set<Campanha>();
    public DbSet<GuiaSindical> GuiaSindicais => Set<GuiaSindical>();
    public DbSet<RegistroBaixa> RegistrosBaixa => Set<RegistroBaixa>();
    public DbSet<Relatorio> Relatorios => Set<Relatorio>();
    public DbSet<Negociacao> Negociacoes => Set<Negociacao>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<ConfiguracaoEntidade> Configuracoes => Set<ConfiguracaoEntidade>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
