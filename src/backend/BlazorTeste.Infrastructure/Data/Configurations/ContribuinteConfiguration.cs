using BlazorTeste.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorTeste.Infrastructure.Data.Configurations;

public class ContribuinteConfiguration : IEntityTypeConfiguration<Contribuinte>
{
    public void Configure(EntityTypeBuilder<Contribuinte> b)
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
            owned.WithOwner().HasForeignKey(s => s.ContribuinteId);
        });
        b.OwnsMany(c => c.Historico, owned =>
        {
            owned.ToTable("HistoricoMensais");
            owned.WithOwner().HasForeignKey("ContribuinteId");
        });
    }
}
