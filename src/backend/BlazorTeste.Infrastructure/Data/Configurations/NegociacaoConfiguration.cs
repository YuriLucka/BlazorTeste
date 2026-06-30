using BlazorTeste.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorTeste.Infrastructure.Data.Configurations;

public class NegociacaoConfiguration : IEntityTypeConfiguration<Negociacao>
{
    public void Configure(EntityTypeBuilder<Negociacao> b)
    {
        b.OwnsMany(n => n.Parcelas, owned =>
        {
            owned.ToTable("NegociacaoParcelas");
            owned.WithOwner().HasForeignKey("NegociacaoId");
        });
        b.OwnsMany(n => n.CobrancasOriginais, owned =>
        {
            owned.ToTable("NegociacaoCobrancasOriginais");
            owned.WithOwner().HasForeignKey("NegociacaoId");
        });
    }
}
