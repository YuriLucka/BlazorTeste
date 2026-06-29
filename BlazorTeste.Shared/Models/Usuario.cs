namespace BlazorTeste.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string SenhaHash { get; set; } = "";
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public DateTime UltimoAcesso { get; set; }
    public List<PermissaoEntidade> Permissoes { get; set; } = new();
}

public class PermissaoEntidade
{
    public int EntidadeId { get; set; }
    public string NomeEntidade { get; set; } = "";
    public List<string> Modulos { get; set; } = new();
}
