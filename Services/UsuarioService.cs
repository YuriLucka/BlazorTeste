using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class UsuarioService
{
    private readonly List<Usuario> _usuarios = new()
    {
        new() { Id = 1, Nome = "Ana Lima", Email = "ana.lima@sindhosp.org.br", UltimoAcesso = DateTime.Now.AddMinutes(-15), Permissoes = new() { new() { EntidadeId = 1, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing" } }, new() { EntidadeId = 5, NomeEntidade = "FETURH", Modulos = new() { "Contribuintes" } } } },
        new() { Id = 2, Nome = "Carlos Silva", Email = "carlos.silva@sindhosp.org.br", UltimoAcesso = DateTime.Now.AddHours(-2), Permissoes = new() { new() { EntidadeId = 1, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Mailing" } } } },
        new() { Id = 3, Nome = "Fernanda Costa", Email = "fernanda.costa@sindbar.org.br", UltimoAcesso = DateTime.Now.AddHours(-1), Permissoes = new() { new() { EntidadeId = 2, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Usuários" } } } },
        new() { Id = 4, Nome = "Pedro Martins", Email = "pedro.martins@sindbar.org.br", UltimoAcesso = DateTime.Now.AddDays(-1), Permissoes = new() { new() { EntidadeId = 2, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Mailing" } } } },
        new() { Id = 5, Nome = "Admin Sistema", Email = "admin@dpi.com.br", UltimoAcesso = DateTime.Now.AddHours(-3), Permissoes = new() { new() { EntidadeId = 1, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Entidades", "Usuários" } }, new() { EntidadeId = 2, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Entidades", "Usuários" } } } },
        new() { Id = 6, Nome = "Lucia Fernandes", Email = "lucia.fernandes@sindetur.org.br", UltimoAcesso = DateTime.Now.AddDays(-2), Permissoes = new() { new() { EntidadeId = 3, NomeEntidade = "SINDETUR", Modulos = new() { "Contribuintes", "Cobranças", "Mailing" } }, new() { EntidadeId = 4, NomeEntidade = "SINDEVEN", Modulos = new() { "Contribuintes" } } } }
    };

    public List<Usuario> GetAll() => _usuarios;
}
