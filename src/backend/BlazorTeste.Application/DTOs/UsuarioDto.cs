using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Application.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime UltimoAcesso { get; set; }
    public List<PermissaoEntidade> Permissoes { get; set; } = new();
}
