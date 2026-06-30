using System.Text.Json;
using BlazorTeste.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorTeste.Infrastructure.Data.Configurations;

public class EntidadeConfiguration : IEntityTypeConfiguration<Entidade>
{
    private static readonly JsonSerializerOptions _json = new();
    private static readonly ValueComparer<List<string>> _listComparer = new(
        (a, b) => a != null && b != null && a.SequenceEqual(b),
        v => v.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
        v => v.ToList());

    public void Configure(EntityTypeBuilder<Entidade> b)
    {
        b.Property(e => e.Cnaes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                v => JsonSerializer.Deserialize<List<string>>(v, _json) ?? new())
            .Metadata.SetValueComparer(_listComparer);
        b.Property(e => e.Cidades)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                v => JsonSerializer.Deserialize<List<string>>(v, _json) ?? new())
            .Metadata.SetValueComparer(_listComparer);
    }
}
