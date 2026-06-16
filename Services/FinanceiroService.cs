using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class FinanceiroService
{
    private readonly List<Fornecedor> _fornecedores;
    private readonly List<LancamentoFinanceiro> _lancamentos;

    public FinanceiroService()
    {
        _fornecedores = new List<Fornecedor>
        {
            new() { Id = 1, EntidadeId = 1, Nome = "Telecom Solutions LTDA", Cnpj = "11.222.333/0001-44", Categoria = "Telecomunicações", Email = "financeiro@telecomsolutions.com.br", Telefone = "(11) 3000-1000" },
            new() { Id = 2, EntidadeId = 1, Nome = "Limpeza Total Serviços EIRELI", Cnpj = "22.333.444/0001-55", Categoria = "Serviços Gerais", Email = "contato@limpezatotal.com.br", Telefone = "(11) 3000-2000" },
            new() { Id = 3, EntidadeId = 2, Nome = "TI Soluções Corporativas LTDA", Cnpj = "33.444.555/0001-66", Categoria = "Tecnologia", Email = "suporte@tisolucoes.com.br", Telefone = "(11) 3000-3000" },
            new() { Id = 4, EntidadeId = 2, Nome = "Segurança Preventiva LTDA", Cnpj = "44.555.666/0001-77", Categoria = "Segurança", Email = "operacional@segprev.com.br", Telefone = "(11) 3000-4000" },
            new() { Id = 5, EntidadeId = 1, Nome = "Assessoria Contábil ABC LTDA", Cnpj = "55.666.777/0001-88", Categoria = "Contabilidade", Email = "contabilidade@assessoriaabc.com.br", Telefone = "(11) 3000-5000" }
        };

        var rng = new Random(42);
        var categorias = new[] { "Aluguel", "Folha de Pagamento", "Energia Elétrica", "Telecomunicações", "Material de Escritório", "Serviços Terceirizados", "Impostos e Taxas" };
        var contas = new[] { "Santander — CC 12345-6", "Itaú — CC 67890-1", "Bradesco — CC 11223-4" };

        _lancamentos = Enumerable.Range(1, 30).Select(i =>
        {
            var isEntrada = rng.Next(4) == 0;
            var fornecedor = isEntrada ? null : _fornecedores[rng.Next(_fornecedores.Count)];
            var data = DateTime.Today.AddDays(-rng.Next(0, 180));
            return new LancamentoFinanceiro
            {
                Id = i,
                EntidadeId = rng.Next(1, 6),
                Data = data,
                Categoria = isEntrada ? "Arrecadação de Contribuintes" : categorias[rng.Next(categorias.Length)],
                Descricao = isEntrada ? "Recebimento de boletos — lote automático" : $"Pagamento referente a {(fornecedor?.Categoria ?? "serviço").ToLower()}",
                FornecedorId = fornecedor?.Id,
                NomeFornecedor = fornecedor?.Nome ?? "",
                Valor = Math.Round((decimal)(rng.NextDouble() * (isEntrada ? 50000 : 10000) + 500), 2),
                Tipo = isEntrada ? TipoLancamento.Entrada : TipoLancamento.Saida,
                ContaBancaria = contas[rng.Next(contas.Length)],
                Realizado = data <= DateTime.Today
            };
        }).ToList();
    }

    public List<LancamentoFinanceiro> GetLancamentos() => _lancamentos;
    public List<Fornecedor> GetFornecedores() => _fornecedores;
}
