using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class ContribuinteService
{
    private readonly List<Contribuinte> _contribuintes;

    public ContribuinteService()
    {
        _contribuintes = GenerateContribuintes();
    }

    private static List<Contribuinte> GenerateContribuintes()
    {
        var razoesSociais = new[]
        {
            "HOTEL METROPOLITANO LTDA", "RESTAURANTE BOM SABOR LTDA", "BAR E LANCHONETE PONTO CERTO EIRELI",
            "POUSADA VILA SERENA LTDA", "CHURRASCARIA GAÚCHA DO SUL LTDA", "HOTEL CONFORT INN LTDA",
            "PIZZARIA BELLA NAPOLI EIRELI", "RESTAURANTE SABOR & ARTE LTDA", "CAFETERIA AROMA & COR EIRELI",
            "HOTEL PARK SUITES LTDA", "BAR DO ZEZINHO EIRELI", "RESTAURANTE ORIENTAL JAPAN LTDA",
            "HOSTEL URBAN STAY LTDA", "HAMBURGUERIA PRIME BURGUER LTDA", "PADARIA E RESTAURANTE TRIGO LTDA",
            "HOTEL BUSINESS CLASS LTDA", "CHOPERIA MÜNCHNER LTDA", "RESTAURANTE FRUTOS DO MAR LTDA",
            "MOTEL ESTRADA REAL LTDA", "PIZZARIA NAPOLITANA EIRELI", "SORVETERIA GELATO ITALIANO LTDA",
            "HOTEL EXECUTIVE PLAZA LTDA", "LANCHONETE FAST & GOOD EIRELI", "RESTAURANTE CANTINA ROMANA LTDA",
            "BAR E PETISCARIA BOTECO BOM LTDA", "HOTEL VILLA BELLA LTDA", "SUSHERIA TOKYO GRILL LTDA",
            "CHURRASCARIA BRAVA BRASA LTDA", "RESTAURANTE VEGETARIANO VIVA LTDA", "CONFEITARIA DOCE ARTE LTDA",
            "HOTEL GOLDEN PARK LTDA", "BISTRÔ FRANÇOISE LTDA", "RESTAURANTE EMPÓRIO GOURMET LTDA",
            "HOTEL SEASONS LTDA", "TAQUERIA EL SOMBRERO EIRELI", "CAFETERIA PRIMA COFFEE LTDA",
            "RESORT BEIRA MAR LTDA", "RESTAURANTE FAMÍLIA ITALIANA LTDA", "BAR KARAOKÊ FUN TIME LTDA",
            "HOTEL BOUTIQUE CHARM LTDA", "COZINHA FUSION ORIENTAL LTDA", "RESTAURANTE TERRAÇO PANORÂMICO LTDA",
            "APART HOTEL RESIDENCES LTDA", "BOTEQUIM CENTRAL EIRELI", "RESTAURANTE GRILL MASTER LTDA",
            "POUSADA DO SOL LTDA", "CAFETERIA XÍCARA & CIA EIRELI", "HOTEL BUSINESS EXECUTIVE LTDA",
            "CHOPERIA MALTE DOURADO LTDA", "RESTAURANTE VISTA LINDA LTDA"
        };

        var cidades = new[] { "São Paulo", "Guarulhos", "Osasco", "São Bernardo do Campo", "Santo André", "Campinas" };
        var regimes = new[] { "Simples Nacional", "Lucro Presumido", "Lucro Real" };
        var cnaes = new[] { "5510-8/01", "5611-2/01", "5611-2/03", "5590-6/01", "5612-1/00" };
        var rng = new Random(42);

        return razoesSociais.Select((nome, i) => new Contribuinte
        {
            Id = i + 1,
            EntidadeId = rng.Next(1, 5),
            RazaoSocial = nome,
            Cnpj = $"{rng.Next(10, 99)}.{rng.Next(100, 999)}.{rng.Next(100, 999)}/0001-{rng.Next(10, 99)}",
            Cnae = cnaes[rng.Next(cnaes.Length)],
            CapitalSocial = Math.Round((decimal)(rng.NextDouble() * 990000 + 10000), 2),
            NumeroFuncionarios = rng.Next(1, 200),
            RegimeTributario = regimes[rng.Next(regimes.Length)],
            DataCadastro = DateTime.Today.AddDays(-rng.Next(30, 3650)),
            DataAbertura = DateTime.Today.AddDays(-rng.Next(365, 7300)),
            Situacao = rng.Next(10) < 8 ? "Ativo" : "Inativo",
            Enderecos = new List<Endereco>
            {
                new() { Id = i * 3 + 1, Tipo = "Estabelecimento", Logradouro = $"Rua das Flores", Numero = rng.Next(1, 999).ToString(), Bairro = "Centro", Cidade = cidades[rng.Next(cidades.Length)], Cep = $"01{rng.Next(100, 999)}-{rng.Next(100, 999)}" },
                new() { Id = i * 3 + 2, Tipo = "Cobrança", Logradouro = "Av. Paulista", Numero = rng.Next(1, 2000).ToString(), Bairro = "Bela Vista", Cidade = "São Paulo", Cep = "01310-100" }
            },
            Contatos = new List<Contato>
            {
                new() { Id = i * 2 + 1, Tipo = "Email", Valor = $"contato@empresa{i + 1}.com.br", Descricao = "E-mail principal" },
                new() { Id = i * 2 + 2, Tipo = "Telefone", Valor = $"(11) {rng.Next(2000, 9999)}-{rng.Next(1000, 9999)}", Descricao = "Telefone comercial" }
            },
            Historico = Enumerable.Range(1, 6).Select(m => new HistoricoMensal
            {
                Id = i * 6 + m,
                Mes = DateTime.Today.AddMonths(-6 + m).Month,
                Ano = DateTime.Today.AddMonths(-6 + m).Year,
                CapitalSocial = Math.Round((decimal)(rng.NextDouble() * 990000 + 10000), 2),
                NumeroFuncionarios = rng.Next(1, 200)
            }).ToList()
        }).ToList();
    }

    public List<Contribuinte> GetAll() => _contribuintes;
    public Contribuinte? GetById(int id) => _contribuintes.FirstOrDefault(c => c.Id == id);
    public List<Contribuinte> GetByEntidade(int entidadeId) => _contribuintes.Where(c => c.EntidadeId == entidadeId).ToList();
}
