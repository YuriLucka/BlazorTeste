namespace BlazorTeste.Application.DTOs;

public class ProcessarArquivoDto
{
    public int CobrancasBaixadas { get; set; }
    public int Erros { get; set; }
    public string Mensagem { get; set; } = "";
}
