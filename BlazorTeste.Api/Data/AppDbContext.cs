using System.Text.Json;
using BlazorTeste.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
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
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    private static readonly JsonSerializerOptions _json = new();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entidade>(b =>
        {
            b.Property(e => e.Cnaes)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _json),
                    v => JsonSerializer.Deserialize<List<string>>(v, _json) ?? new());
            b.Property(e => e.Cidades)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _json),
                    v => JsonSerializer.Deserialize<List<string>>(v, _json) ?? new());
        });

        modelBuilder.Entity<Contribuinte>(b =>
        {
            b.OwnsMany(c => c.Enderecos, owned =>
            {
                owned.ToTable("Enderecos");
                owned.WithOwner().HasForeignKey("ContribuinteId");
            });
            b.OwnsMany(c => c.Contatos, owned =>
            {
                owned.ToTable("Contatos");
                owned.WithOwner().HasForeignKey("ContribuinteId");
            });
            b.OwnsMany(c => c.Socios, owned =>
            {
                owned.ToTable("Socios");
                owned.WithOwner().HasForeignKey("ContribuinteId");
                owned.Ignore(s => s.ContribuinteId);
            });
            b.OwnsMany(c => c.Historico, owned =>
            {
                owned.ToTable("HistoricoMensais");
                owned.WithOwner().HasForeignKey("ContribuinteId");
            });
        });

        modelBuilder.Entity<Usuario>(b =>
        {
            b.Property(u => u.Permissoes)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _json),
                    v => JsonSerializer.Deserialize<List<PermissaoEntidade>>(v, _json) ?? new());
        });
    }
}
