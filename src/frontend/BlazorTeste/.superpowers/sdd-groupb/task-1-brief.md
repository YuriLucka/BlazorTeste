## Task 1: Simple-model API (GuiaSindical, RegistroBaixa, Relatorio)

**Files:**
- Modify: `BlazorTeste.Api/Data/AppDbContext.cs`
- Modify: `BlazorTeste.Api/Data/SeedData.cs`
- Create: `BlazorTeste.Api/Controllers/GuiaSindicalController.cs`
- Create: `BlazorTeste.Api/Controllers/BaixaController.cs`
- Create: `BlazorTeste.Api/Controllers/RelatoriosController.cs`

**Interfaces:**
- Produces: `GET api/guiasindical?entidadeId={int}` → `List<GuiaSindical>`
- Produces: `GET api/baixa/historico?entidadeId={int}` → `List<RegistroBaixa>`
- Produces: `GET api/relatorios` → `List<Relatorio>`

- [ ] **Step 1: Restructure SeedData guard and add Group A seed**

Replace `BlazorTeste.Api/Data/SeedData.cs` with:

```csharp
using BlazorTeste.Api.Auth;
using BlazorTeste.Models;

namespace BlazorTeste.Api.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (!db.Entidades.Any())
        {
            var rng = new Random(42);

            var entidades = new List<Entidade>
            {
                new() { Nome = "Sindicato de Hotéis e Meios de Hospedagem", Sigla = "SINDHOSP", Cnpj = "12.345.678/0001-90", CidadeSede = "São Paulo", TotalContribuintes = 1243, Cnaes = new() { "5510-8/01", "5590-6/01", "5590-6/02" }, Cidades = new() { "São Paulo", "Guarulhos", "Osasco" } },
                new() { Nome = "Sindicato de Bares e Restaurantes", Sigla = "SINDBAR", Cnpj = "23.456.789/0001-01", CidadeSede = "São Paulo", TotalContribuintes = 8921, Cnaes = new() { "5611-2/01", "5611-2/03", "5612-1/00" }, Cidades = new() { "São Paulo", "São Bernardo do Campo", "Santo André" } },
                new() { Nome = "Sindicato de Empresas de Turismo", Sigla = "SINDETUR", Cnpj = "34.567.890/0001-12", CidadeSede = "São Paulo", TotalContribuintes = 562, Cnaes = new() { "7911-2/00", "7912-1/00", "7990-2/00" }, Cidades = new() { "São Paulo", "Campinas", "Sorocaba" } },
                new() { Nome = "Sindicato de Promotores de Eventos", Sigla = "SINDEVEN", Cnpj = "45.678.901/0001-23", CidadeSede = "São Paulo", TotalContribuintes = 387, Cnaes = new() { "8230-0/01", "8230-0/02" }, Cidades = new() { "São Paulo", "Barueri", "Cotia" } },
                new() { Nome = "Federação Estadual de Turismo e Hospitalidade", Sigla = "FETURH", Cnpj = "56.789.012/0001-34", CidadeSede = "São Paulo", TotalContribuintes = 4, Cnaes = new(), Cidades = new() { "São Paulo" } }
            };
            db.Entidades.AddRange(entidades);
            db.SaveChanges();

            var fornecedores = new List<Fornecedor>
            {
                new() { EntidadeId = entidades[0].Id, Nome = "Telecom Solutions LTDA", Cnpj = "11.222.333/0001-44", Categoria = "Telecomunicações", Email = "financeiro@telecomsolutions.com.br", Telefone = "(11) 3000-1000" },
                new() { EntidadeId = entidades[0].Id, Nome = "Limpeza Total Serviços EIRELI", Cnpj = "22.333.444/0001-55", Categoria = "Serviços Gerais", Email = "contato@limpezatotal.com.br", Telefone = "(11) 3000-2000" },
                new() { EntidadeId = entidades[1].Id, Nome = "TI Soluções Corporativas LTDA", Cnpj = "33.444.555/0001-66", Categoria = "Tecnologia", Email = "suporte@tisolucoes.com.br", Telefone = "(11) 3000-3000" },
                new() { EntidadeId = entidades[1].Id, Nome = "Segurança Preventiva LTDA", Cnpj = "44.555.666/0001-77", Categoria = "Segurança", Email = "operacional@segprev.com.br", Telefone = "(11) 3000-4000" },
                new() { EntidadeId = entidades[0].Id, Nome = "Assessoria Contábil ABC LTDA", Cnpj = "55.666.777/0001-88", Categoria = "Contabilidade", Email = "contabilidade@assessoriaabc.com.br", Telefone = "(11) 3000-5000" }
            };
            db.Fornecedores.AddRange(fornecedores);

            var advogados = new List<Advogado>
            {
                new() { EntidadeId = entidades[0].Id, Nome = "Dr. Carlos Eduardo Mendes", Oab = "OAB/SP 123.456", Email = "carlos.mendes@jurmendes.adv.br", Telefone = "(11) 3456-7890", ProcessosAtivos = 5 },
                new() { EntidadeId = entidades[0].Id, Nome = "Dra. Fernanda Lima Costa", Oab = "OAB/SP 234.567", Email = "fernanda.costa@fclima.adv.br", Telefone = "(11) 4567-8901", ProcessosAtivos = 3 },
                new() { EntidadeId = entidades[1].Id, Nome = "Dr. Ricardo Alves Pinheiro", Oab = "OAB/SP 345.678", Email = "r.pinheiro@rapinheiro.adv.br", Telefone = "(11) 5678-9012", ProcessosAtivos = 7 },
                new() { EntidadeId = entidades[1].Id, Nome = "Dra. Amanda Souza Ferreira", Oab = "OAB/SP 456.789", Email = "amanda.ferreira@asferreira.adv.br", Telefone = "(11) 6789-0123", ProcessosAtivos = 4 },
                new() { EntidadeId = entidades[2].Id, Nome = "Dr. Marcelo Torres Braga", Oab = "OAB/SP 567.890", Email = "marcelo.braga@mtbraga.adv.br", Telefone = "(11) 7890-1234", ProcessosAtivos = 2 },
                new() { EntidadeId = entidades[2].Id, Nome = "Dra. Patrícia Gomes Vieira", Oab = "OAB/SP 678.901", Email = "patricia.vieira@pgvieira.adv.br", Telefone = "(11) 8901-2345", ProcessosAtivos = 6 },
                new() { EntidadeId = entidades[3].Id, Nome = "Dr. Roberto Nascimento Silva", Oab = "OAB/SP 789.012", Email = "roberto.silva@rnsilva.adv.br", Telefone = "(11) 9012-3456", ProcessosAtivos = 1 },
                new() { EntidadeId = entidades[0].Id, Nome = "Dra. Juliana Castro Moreira", Oab = "OAB/SP 890.123", Email = "juliana.moreira@jcmoreira.adv.br", Telefone = "(11) 0123-4567", ProcessosAtivos = 3 }
            };
            db.Advogados.AddRange(advogados);
            db.SaveChanges();

            var razoesSociais = new[] { "HOTEL METROPOLITANO LTDA", "RESTAURANTE BOM SABOR LTDA", "BAR E LANCHONETE PONTO CERTO EIRELI", "POUSADA VILA SERENA LTDA", "CHURRASCARIA GAÚCHA DO SUL LTDA", "HOTEL CONFORT INN LTDA", "PIZZARIA BELLA NAPOLI EIRELI", "RESTAURANTE SABOR & ARTE LTDA", "CAFETERIA AROMA & COR EIRELI", "HOTEL PARK SUITES LTDA", "BAR DO ZEZINHO EIRELI", "RESTAURANTE ORIENTAL JAPAN LTDA", "HOSTEL URBAN STAY LTDA", "HAMBURGUERIA PRIME BURGUER LTDA", "PADARIA E RESTAURANTE TRIGO LTDA", "HOTEL BUSINESS CLASS LTDA", "CHOPERIA MÜNCHNER LTDA", "RESTAURANTE FRUTOS DO MAR LTDA", "MOTEL ESTRADA REAL LTDA", "PIZZARIA NAPOLITANA EIRELI", "SORVETERIA GELATO ITALIANO LTDA", "HOTEL EXECUTIVE PLAZA LTDA", "LANCHONETE FAST & GOOD EIRELI", "RESTAURANTE CANTINA ROMANA LTDA", "BAR E PETISCARIA BOTECO BOM LTDA", "HOTEL VILLA BELLA LTDA", "SUSHERIA TOKYO GRILL LTDA", "CHURRASCARIA BRAVA BRASA LTDA", "RESTAURANTE VEGETARIANO VIVA LTDA", "CONFEITARIA DOCE ARTE LTDA", "HOTEL GOLDEN PARK LTDA", "BISTRÔ FRANÇOISE LTDA", "RESTAURANTE EMPÓRIO GOURMET LTDA", "HOTEL SEASONS LTDA", "TAQUERIA EL SOMBRERO EIRELI", "CAFETERIA PRIMA COFFEE LTDA", "RESORT BEIRA MAR LTDA", "RESTAURANTE FAMÍLIA ITALIANA LTDA", "BAR KARAOKÊ FUN TIME LTDA", "HOTEL BOUTIQUE CHARM LTDA", "COZINHA FUSION ORIENTAL LTDA", "RESTAURANTE TERRAÇO PANORÂMICO LTDA", "APART HOTEL RESIDENCES LTDA", "BOTEQUIM CENTRAL EIRELI", "RESTAURANTE GRILL MASTER LTDA", "POUSADA DO SOL LTDA", "CAFETERIA XÍCARA & CIA EIRELI", "HOTEL BUSINESS EXECUTIVE LTDA", "CHOPERIA MALTE DOURADO LTDA", "RESTAURANTE VISTA LINDA LTDA" };
            var cidades = new[] { "São Paulo", "Guarulhos", "Osasco", "São Bernardo do Campo", "Santo André", "Campinas" };
            var regimes = new[] { "Simples Nacional", "Lucro Presumido", "Lucro Real" };
            var cnaes = new[] { "5510-8/01", "5611-2/01", "5611-2/03", "5590-6/01", "5612-1/00" };

            var contribuintes = razoesSociais.Select((nome, i) => new Contribuinte
            {
                EntidadeId = entidades[rng.Next(5)].Id,
                RazaoSocial = nome,
                NomeFantasia = nome.Split(' ')[0] + " " + nome.Split(' ')[1],
                Cnpj = $"{rng.Next(10, 99)}.{rng.Next(100, 999)}.{rng.Next(100, 999)}/0001-{rng.Next(10, 99)}",
                Email = $"financeiro@{nome.Split(' ')[0].ToLower()}.com.br",
                Telefone = $"(11) {rng.Next(3000, 9999)}-{rng.Next(1000, 9999)}",
                Cnae = cnaes[rng.Next(cnaes.Length)],
                Cidade = cidades[rng.Next(cidades.Length)],
                RegimeTributario = regimes[rng.Next(regimes.Length)],
                CapitalSocial = rng.Next(50, 5000) * 1000m,
                DataFiliacao = DateTime.Today.AddDays(-rng.Next(180, 3650)),
                Ativo = rng.Next(10) > 1,
                Enderecos = new() { new() { Tipo = "Principal", Logradouro = $"Rua {nome.Split(' ')[0]}", Numero = $"{rng.Next(1, 999)}", Bairro = "Centro", Cidade = cidades[rng.Next(cidades.Length)], Estado = "SP", Cep = $"{rng.Next(01000, 09999):D5}-{rng.Next(0, 999):D3}" } },
                Contatos = new() { new() { Nome = $"Responsável {nome.Split(' ')[0]}", Cargo = "Gestor", Email = $"gestor@{nome.Split(' ')[0].ToLower()}.com.br", Telefone = $"(11) {rng.Next(9, 9)}{rng.Next(1000, 9999)}-{rng.Next(1000, 9999)}" } },
                Socios = new() { new() { Nome = $"Sócio de {nome.Split(' ')[0]}", Cpf = $"{rng.Next(100, 999)}.{rng.Next(100, 999)}.{rng.Next(100, 999)}-{rng.Next(10, 99)}", Participacao = 100m } },
                Historico = Enumerable.Range(0, 6).Select(m => new HistoricoMensal { Mes = DateTime.Today.AddMonths(-m).Month, Ano = DateTime.Today.AddMonths(-m).Year, Adimplente = rng.Next(10) > 2 }).ToList()
            }).ToList();
            db.Contribuintes.AddRange(contribuintes);
            db.SaveChanges();

            var cobrancas = contribuintes.SelectMany((c, ci) => new[]
            {
                new Cobranca { EntidadeId = c.EntidadeId, ContribuinteId = c.Id, RazaoSocialContribuinte = c.RazaoSocial, Tipo = "Sindical", Competencia = $"{DateTime.Today.Year}/01", Valor = c.CapitalSocial * 0.001m, Vencimento = new DateTime(DateTime.Today.Year, 4, 30), Status = (StatusCobranca)rng.Next(5), DataEmissao = new DateTime(DateTime.Today.Year, 1, 15), Nossonumero = $"00000{ci + 1:D5}" },
                new Cobranca { EntidadeId = c.EntidadeId, ContribuinteId = c.Id, RazaoSocialContribuinte = c.RazaoSocial, Tipo = "Associativa", Competencia = $"{DateTime.Today.Year}/01", Valor = 480m, Vencimento = new DateTime(DateTime.Today.Year, 4, 30), Status = (StatusCobranca)rng.Next(5), DataEmissao = new DateTime(DateTime.Today.Year, 1, 15), Nossonumero = $"10000{ci + 1:D5}" }
            }).ToList();
            db.Cobrancas.AddRange(cobrancas);

            var lancamentos = fornecedores.SelectMany(f => new[]
            {
                new LancamentoFinanceiro { EntidadeId = f.EntidadeId, Descricao = $"Pagamento {f.Nome}", Tipo = "Despesa", Categoria = f.Categoria, Valor = rng.Next(500, 5000) * 1m, DataLancamento = DateTime.Today.AddDays(-rng.Next(1, 30)), DataVencimento = DateTime.Today.AddDays(rng.Next(5, 60)), Status = "Pago", FornecedorId = f.Id },
                new LancamentoFinanceiro { EntidadeId = f.EntidadeId, Descricao = $"Receita associativa {f.Nome}", Tipo = "Receita", Categoria = "Associativa", Valor = rng.Next(300, 3000) * 1m, DataLancamento = DateTime.Today.AddDays(-rng.Next(1, 15)), DataVencimento = DateTime.Today.AddDays(rng.Next(1, 30)), Status = "Previsto" }
            }).ToList();
            db.LancamentosFinanceiros.AddRange(lancamentos);

            var processos = advogados.SelectMany((a, ai) => new[]
            {
                new Processo { EntidadeId = a.EntidadeId, Numero = $"{rng.Next(1000, 9999)}-{rng.Next(10, 99)}.{DateTime.Today.Year}.{rng.Next(1, 9)}.{rng.Next(10, 99)}.{rng.Next(1000, 9999)}", ContribuinteId = contribuintes.Where(c => c.EntidadeId == a.EntidadeId).Skip(ai % 3).FirstOrDefault()?.Id ?? contribuintes[0].Id, Descricao = "Cobrança de débitos sindicais", Fase = "Em andamento", DataAbertura = DateTime.Today.AddDays(-rng.Next(30, 365)), AdvogadoId = a.Id, ValorCausa = rng.Next(1000, 20000) * 1m }
            }).ToList();
            db.Processos.AddRange(processos);

            var audiencias = processos.Take(5).Select((p, pi) => new Audiencia { EntidadeId = p.EntidadeId, ProcessoId = p.Id, Data = DateTime.Today.AddDays(rng.Next(1, 90)), Tipo = new[] { "Inicial", "Instrução", "Julgamento" }[pi % 3], Local = "Fórum Central de São Paulo", Descricao = "Audiência de instrução e julgamento" }).ToList();
            db.Audiencias.AddRange(audiencias);

            var campanhas = entidades.Take(3).SelectMany((e, ei) => new[]
            {
                new Campanha { EntidadeId = e.Id, Nome = $"Campanha Associativa {DateTime.Today.Year}", Tipo = "Email", DataEnvio = DateTime.Today.AddDays(-rng.Next(1, 30)), TotalDestinatarios = rng.Next(100, 500), TotalEnviados = rng.Next(80, 450), TotalAbertos = rng.Next(30, 200), TotalClicados = rng.Next(10, 100), Status = "Concluída" }
            }).ToList();
            db.Campanhas.AddRange(campanhas);

            var usuarios = new List<Usuario>
            {
                new() { Nome = "Admin Sistema", Email = "admin@dpi.com.br", UltimoAcesso = DateTime.Now, SenhaHash = PasswordHelper.Hash("Senha@123"), Permissoes = entidades.Select(e => new PermissaoEntidade { EntidadeId = e.Id, Perfil = "Admin" }).ToList() },
                new() { Nome = "Ana Lima", Email = "ana.lima@sindhosp.org.br", UltimoAcesso = DateTime.Now.AddHours(-2), SenhaHash = PasswordHelper.Hash("Senha@123"), Permissoes = new() { new() { EntidadeId = entidades[0].Id, Perfil = "Operador" } } },
                new() { Nome = "Carlos Silva", Email = "carlos.silva@sindhosp.org.br", UltimoAcesso = DateTime.Now.AddDays(-1), SenhaHash = PasswordHelper.Hash("Senha@123"), Permissoes = new() { new() { EntidadeId = entidades[0].Id, Perfil = "Financeiro" } } },
                new() { Nome = "Fernanda Costa", Email = "fernanda.costa@sindbar.org.br", UltimoAcesso = DateTime.Now.AddDays(-2), SenhaHash = PasswordHelper.Hash("Senha@123"), Permissoes = new() { new() { EntidadeId = entidades[1].Id, Perfil = "Operador" } } },
                new() { Nome = "Pedro Martins", Email = "pedro.martins@sindbar.org.br", UltimoAcesso = DateTime.Now.AddDays(-3), SenhaHash = PasswordHelper.Hash("Senha@123"), Permissoes = new() { new() { EntidadeId = entidades[1].Id, Perfil = "Financeiro" } } },
                new() { Nome = "Lucia Fernandes", Email = "lucia.fernandes@sindetur.org.br", UltimoAcesso = DateTime.Now.AddDays(-5), SenhaHash = PasswordHelper.Hash("Senha@123"), Permissoes = new() { new() { EntidadeId = entidades[2].Id, Perfil = "Admin" } } }
            };
            db.Usuarios.AddRange(usuarios);
            db.SaveChanges();
        }

        // Group A — simple tables (Task 1)
        if (!db.GuiaSindicais.Any() && db.Contribuintes.Any())
        {
            var entidade1 = db.Entidades.OrderBy(e => e.Id).First();
            var contribs = db.Contribuintes.Where(c => c.EntidadeId == entidade1.Id).OrderBy(c => c.Id).Take(8).ToList();
            var cnpjs = new[] { "12.345.678/0001-90", "98.765.432/0001-10", "11.222.333/0001-44", "44.555.666/0001-77", "55.666.777/0001-88", "66.777.888/0001-99", "77.888.999/0001-00", "88.999.000/0001-11" };
            var guias = contribs.Select((c, i) => new GuiaSindical
            {
                EntidadeId = entidade1.Id,
                ContribuinteId = c.Id,
                RazaoSocialContribuinte = c.RazaoSocial,
                Cnpj = cnpjs[i % cnpjs.Length],
                AnoReferencia = i < 6 ? 2026 : 2025,
                ValorBase = new[] { 1250m, 2800m, 480m, 3600m, 8900m, 1100m, 5200m, 750m }[i],
                Multa = new[] { 0m, 0m, 9.60m, 0m, 0m, 22m, 0m, 0m }[i],
                Juros = new[] { 0m, 0m, 15.84m, 0m, 0m, 36.30m, 0m, 0m }[i],
                Vencimento = new[] { new DateTime(2026,4,30), new DateTime(2026,4,30), new DateTime(2026,3,31), new DateTime(2026,5,31), new DateTime(2026,6,30), new DateTime(2025,4,30), new DateTime(2025,4,30), new DateTime(2026,7,31) }[i],
                DataPagamento = new DateTime?[] { null, new DateTime(2026,4,12), null, new DateTime(2026,5,10), null, null, new DateTime(2025,4,20), null }[i],
                Status = new[] { StatusGuia.Pendente, StatusGuia.Paga, StatusGuia.Vencida, StatusGuia.Paga, StatusGuia.Pendente, StatusGuia.Vencida, StatusGuia.Paga, StatusGuia.Pendente }[i],
                LinhaDigitavel = $"00190.00009 02000.{i+1:D6} 00000.{i+1:D6} 1 {(i < 6 ? 2026 : 2025)}{i+1:D10}",
                NumeroDocumento = $"SND{(i < 6 ? 2026 : 2025)}{i+1:D6}",
                DataEmissao = new DateTime(i < 6 ? 2026 : 2025, 1, 15)
            }).ToList();
            db.GuiaSindicais.AddRange(guias);
            db.SaveChanges();
        }

        if (!db.RegistrosBaixa.Any() && db.Cobrancas.Any())
        {
            var entidade1 = db.Entidades.OrderBy(e => e.Id).First();
            var cobrancas = db.Cobrancas.Where(c => c.EntidadeId == entidade1.Id).OrderBy(c => c.Id).Take(6).ToList();
            var razoesSociais = new[] { "Hotel Paulista S/A", "Restaurante Bella Vista Ltda", "Hotel Gran Via S/A", "Bar do Zé ME", "Pousada das Flores Ltda", "Buffet Bom Sabor S/A" };
            var tipos = new[] { "Associativa", "Sindical", "Guia Sindical", "Confederativa", "Associativa", "Sindical" };
            var valores = new[] { 480m, 1250m, 8900m, 320m, 480m, 2800m };
            var diasAtras = new[] { 0, 0, 1, 2, 3, 5 };
            var tipoBaixas = new[] { TipoBaixa.Manual, TipoBaixa.ArquivoRetorno, TipoBaixa.ArquivoRetorno, TipoBaixa.Manual, TipoBaixa.Automatica, TipoBaixa.ArquivoRetorno };
            var operadores = new[] { "Maria Santos", "João Silva", "João Silva", "Maria Santos", "Sistema", "João Silva" };
            var today = DateTime.Today;
            var registros = cobrancas.Select((c, i) => new RegistroBaixa
            {
                EntidadeId = entidade1.Id,
                CobrancaId = c.Id,
                RazaoSocialContribuinte = razoesSociais[i],
                TipoCobranca = tipos[i],
                ValorOriginal = valores[i],
                ValorPago = valores[i],
                DataPagamento = today.AddDays(-diasAtras[i]),
                DataProcessamento = today.AddDays(-diasAtras[i]),
                TipoBaixa = tipoBaixas[i],
                OperadorResponsavel = operadores[i]
            }).ToList();
            db.RegistrosBaixa.AddRange(registros);
            db.SaveChanges();
        }

        if (!db.Relatorios.Any())
        {
            db.Relatorios.AddRange(
                new Relatorio { Nome = "Arrecadação por Período", Descricao = "Total arrecadado por tipo de cobrança em um período", Categoria = CategoriaRelatorio.Cobrancas, Formato = FormatoRelatorio.Ambos, Icone = "ReceiptLong" },
                new Relatorio { Nome = "Inadimplência", Descricao = "Contribuintes com cobranças vencidas e valores em aberto", Categoria = CategoriaRelatorio.Cobrancas, Formato = FormatoRelatorio.Ambos, Icone = "Warning" },
                new Relatorio { Nome = "Boletos Emitidos", Descricao = "Relação de todos os boletos emitidos no período", Categoria = CategoriaRelatorio.Cobrancas, Formato = FormatoRelatorio.PDF, Icone = "Receipt" },
                new Relatorio { Nome = "Conciliação Bancária", Descricao = "Conferência entre retorno bancário e baixas realizadas", Categoria = CategoriaRelatorio.Cobrancas, Formato = FormatoRelatorio.Excel, Icone = "AccountBalance" },
                new Relatorio { Nome = "Contribuintes Ativos", Descricao = "Listagem completa de contribuintes ativos com dados cadastrais", Categoria = CategoriaRelatorio.Contribuintes, Formato = FormatoRelatorio.Ambos, Icone = "Business" },
                new Relatorio { Nome = "Contribuintes por Cidade", Descricao = "Distribuição geográfica por município", Categoria = CategoriaRelatorio.Contribuintes, Formato = FormatoRelatorio.Ambos, Icone = "LocationCity" },
                new Relatorio { Nome = "Mala Direta", Descricao = "Etiquetas e relação de endereços para correspondência", Categoria = CategoriaRelatorio.Contribuintes, Formato = FormatoRelatorio.PDF, Icone = "Mail" },
                new Relatorio { Nome = "Fluxo de Caixa", Descricao = "Entradas e saídas financeiras por período", Categoria = CategoriaRelatorio.Financeiro, Formato = FormatoRelatorio.Ambos, Icone = "TrendingUp" },
                new Relatorio { Nome = "DRE Simplificado", Descricao = "Demonstrativo de resultado por categoria de receita/despesa", Categoria = CategoriaRelatorio.Financeiro, Formato = FormatoRelatorio.PDF, Icone = "Assessment" },
                new Relatorio { Nome = "Contas a Pagar", Descricao = "Títulos a pagar por fornecedor e vencimento", Categoria = CategoriaRelatorio.Financeiro, Formato = FormatoRelatorio.Ambos, Icone = "Payments" },
                new Relatorio { Nome = "Processos em Andamento", Descricao = "Relação de processos jurídicos ativos por advogado", Categoria = CategoriaRelatorio.Juridico, Formato = FormatoRelatorio.Ambos, Icone = "Gavel" },
                new Relatorio { Nome = "Pauta de Audiências", Descricao = "Agenda de audiências por período e advogado", Categoria = CategoriaRelatorio.Juridico, Formato = FormatoRelatorio.PDF, Icone = "Event" },
                new Relatorio { Nome = "Devedores Jurídico", Descricao = "Contribuintes com processos de cobrança judicial", Categoria = CategoriaRelatorio.Juridico, Formato = FormatoRelatorio.Excel, Icone = "GavelRounded" },
                new Relatorio { Nome = "Inscrições por Evento", Descricao = "Lista de inscritos com situação de pagamento e presença", Categoria = CategoriaRelatorio.Eventos, Formato = FormatoRelatorio.Ambos, Icone = "Groups" },
                new Relatorio { Nome = "Faturamento de Eventos", Descricao = "Receita por evento e tipo de inscrição", Categoria = CategoriaRelatorio.Eventos, Formato = FormatoRelatorio.Excel, Icone = "AttachMoney" },
                new Relatorio { Nome = "Lista de Hotel", Descricao = "Relação de hospedagem por evento e participante", Categoria = CategoriaRelatorio.Eventos, Formato = FormatoRelatorio.PDF, Icone = "Hotel" },
                new Relatorio { Nome = "Guias Emitidas", Descricao = "Relação de Guias Sindicais emitidas por período", Categoria = CategoriaRelatorio.GuiaSindical, Formato = FormatoRelatorio.Ambos, Icone = "Description" },
                new Relatorio { Nome = "Guias Pagas", Descricao = "Guias Sindicais pagas com data e valor de pagamento", Categoria = CategoriaRelatorio.GuiaSindical, Formato = FormatoRelatorio.Ambos, Icone = "CheckCircle" },
                new Relatorio { Nome = "Evasão Sindical", Descricao = "Contribuintes que não emitiram Guia Sindical no ano", Categoria = CategoriaRelatorio.GuiaSindical, Formato = FormatoRelatorio.Excel, Icone = "TrendingDown" }
            );
            db.SaveChanges();
        }
    }
}
```

- [ ] **Step 2: Add DbSets for Group A to AppDbContext**

In `BlazorTeste.Api/Data/AppDbContext.cs`, add after the `Campanhas` DbSet line:

```csharp
public DbSet<GuiaSindical> GuiaSindicais => Set<GuiaSindical>();
public DbSet<RegistroBaixa> RegistrosBaixa => Set<RegistroBaixa>();
public DbSet<Relatorio> Relatorios => Set<Relatorio>();
```

- [ ] **Step 3: Create GuiaSindicalController**

Create `BlazorTeste.Api/Controllers/GuiaSindicalController.cs`:

```csharp
using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GuiaSindicalController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<GuiaSindical>> GetAll([FromQuery] int? entidadeId = null)
    {
        var query = db.GuiaSindicais.AsQueryable();
        if (entidadeId.HasValue) query = query.Where(g => g.EntidadeId == entidadeId.Value);
        return await query.ToListAsync();
    }
}
```

- [ ] **Step 4: Create BaixaController**

Create `BlazorTeste.Api/Controllers/BaixaController.cs`:

```csharp
using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BaixaController(AppDbContext db) : ControllerBase
{
    [HttpGet("historico")]
    public async Task<List<RegistroBaixa>> GetHistorico([FromQuery] int? entidadeId = null)
    {
        var query = db.RegistrosBaixa.AsQueryable();
        if (entidadeId.HasValue) query = query.Where(r => r.EntidadeId == entidadeId.Value);
        return await query.OrderByDescending(r => r.DataProcessamento).ToListAsync();
    }
}
```

- [ ] **Step 5: Create RelatoriosController**

Create `BlazorTeste.Api/Controllers/RelatoriosController.cs`:

```csharp
using BlazorTeste.Api.Data;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelatoriosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<Relatorio>> GetAll() =>
        await db.Relatorios.OrderBy(r => r.Categoria).ThenBy(r => r.Nome).ToListAsync();
}
```

- [ ] **Step 6: Create migration**

Run from `C:\Users\yrodrigues\source\repos\BlazorTeste\BlazorTeste.Api`:

```
dotnet ef migrations add AddGroupAEntities
```

Expected: creates `Migrations/YYYYMMDDHHMMSS_AddGroupAEntities.cs` with 3 new `CreateTable` calls for `GuiaSindicais`, `RegistrosBaixa`, `Relatorios`.

- [ ] **Step 7: Build to verify 0 errors**

Run from `C:\Users\yrodrigues\source\repos\BlazorTeste`:

```
dotnet build
```

Expected: `Build succeeded. 0 Warning(s). 0 Error(s)`

- [ ] **Step 8: Commit**

```
git -C "C:\Users\yrodrigues\source\repos\BlazorTeste" add -A
git -C "C:\Users\yrodrigues\source\repos\BlazorTeste" commit -m "feat: add GuiaSindical, RegistroBaixa, Relatorio API endpoints"
```

---

