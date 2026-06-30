using BlazorTeste.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorTeste.Infrastructure.Data.Configurations;

public class EventoConfiguration : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> b)
    {
        b.OwnsMany(e => e.Inscricoes, owned =>
        {
            owned.ToTable("EventoInscricoes");
            owned.WithOwner().HasForeignKey("EventoId");
        });
    }
}
