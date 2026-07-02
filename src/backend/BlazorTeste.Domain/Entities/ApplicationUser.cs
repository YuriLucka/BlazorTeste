using Microsoft.AspNetCore.Identity;

namespace BlazorTeste.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Nome { get; set; } = "";
    public DateTime UltimoAcesso { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public List<PermissaoEntidade> Permissoes { get; set; } = new();
}

public class PermissaoEntidade
{
    public int EntidadeId { get; set; }
    public string NomeEntidade { get; set; } = "";
    public List<string> Modulos { get; set; } = new();
}
