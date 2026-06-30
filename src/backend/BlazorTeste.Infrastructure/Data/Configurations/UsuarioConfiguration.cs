using System.Text.Json;
using BlazorTeste.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorTeste.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    private static readonly JsonSerializerOptions _json = new();
    private static readonly ValueComparer<List<PermissaoEntidade>> _comparer = new(
        (a, b) => JsonSerializer.Serialize(a, _json) == JsonSerializer.Serialize(b, _json),
        v => JsonSerializer.Serialize(v, _json).GetHashCode(),
        v => JsonSerializer.Deserialize<List<PermissaoEntidade>>(JsonSerializer.Serialize(v, _json), _json)!);

    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.Property(u => u.Permissoes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                v => JsonSerializer.Deserialize<List<PermissaoEntidade>>(v, _json) ?? new())
            .Metadata.SetValueComparer(_comparer);
    }
}
