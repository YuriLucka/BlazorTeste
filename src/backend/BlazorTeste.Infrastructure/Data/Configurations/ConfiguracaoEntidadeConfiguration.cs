using BlazorTeste.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorTeste.Infrastructure.Data.Configurations;

public class ConfiguracaoEntidadeConfiguration : IEntityTypeConfiguration<ConfiguracaoEntidade>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoEntidade> b)
    {
        b.HasKey(c => c.EntidadeId);
        b.Property(c => c.EntidadeId).ValueGeneratedNever();
        b.OwnsOne(c => c.Geral, o => o.ToJson());
        b.OwnsOne(c => c.Cobranca, o =>
        {
            o.ToJson();
            o.OwnsMany(cc => cc.Faixas);
        });
        b.OwnsOne(c => c.Email, o => o.ToJson());
        b.OwnsOne(c => c.Banco, o => o.ToJson());
    }
}
