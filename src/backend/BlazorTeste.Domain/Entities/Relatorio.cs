using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Entities;

public class Relatorio
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Descricao { get; set; } = "";
    public CategoriaRelatorio Categoria { get; set; }
    public FormatoRelatorio Formato { get; set; }
    public string Icone { get; set; } = "";
}
