namespace BlazorTeste.Application.DTOs;

public class PermissaoEntidadeDto
{
    public int EntidadeId { get; set; }
    public string NomeEntidade { get; set; } = "";
    public List<string> Modulos { get; set; } = new();
}
