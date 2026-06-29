using BlazorTeste.Api.Auth;
using BlazorTeste.Models;

namespace BlazorTeste.Api.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Entidades.Any()) return;

        var rng = new Random(42);

        // Stage 1: Entidades
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

        // Stage 2: Fornecedores + Advogados
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

        // Stage 3: Contribuintes
        var razoesSociais = new[] { "HOTEL METROPOLITANO LTDA", "RESTAURANTE BOM SABOR LTDA", "BAR E LANCHONETE PONTO CERTO EIRELI", "POUSADA VILA SERENA LTDA", "CHURRASCARIA GAÚCHA DO SUL LTDA", "HOTEL CONFORT INN LTDA", "PIZZARIA BELLA NAPOLI EIRELI", "RESTAURANTE SABOR & ARTE LTDA", "CAFETERIA AROMA & COR EIRELI", "HOTEL PARK SUITES LTDA", "BAR DO ZEZINHO EIRELI", "RESTAURANTE ORIENTAL JAPAN LTDA", "HOSTEL URBAN STAY LTDA", "HAMBURGUERIA PRIME BURGUER LTDA", "PADARIA E RESTAURANTE TRIGO LTDA", "HOTEL BUSINESS CLASS LTDA", "CHOPERIA MÜNCHNER LTDA", "RESTAURANTE FRUTOS DO MAR LTDA", "MOTEL ESTRADA REAL LTDA", "PIZZARIA NAPOLITANA EIRELI", "SORVETERIA GELATO ITALIANO LTDA", "HOTEL EXECUTIVE PLAZA LTDA", "LANCHONETE FAST & GOOD EIRELI", "RESTAURANTE CANTINA ROMANA LTDA", "BAR E PETISCARIA BOTECO BOM LTDA", "HOTEL VILLA BELLA LTDA", "SUSHERIA TOKYO GRILL LTDA", "CHURRASCARIA BRAVA BRASA LTDA", "RESTAURANTE VEGETARIANO VIVA LTDA", "CONFEITARIA DOCE ARTE LTDA", "HOTEL GOLDEN PARK LTDA", "BISTRÔ FRANÇOISE LTDA", "RESTAURANTE EMPÓRIO GOURMET LTDA", "HOTEL SEASONS LTDA", "TAQUERIA EL SOMBRERO EIRELI", "CAFETERIA PRIMA COFFEE LTDA", "RESORT BEIRA MAR LTDA", "RESTAURANTE FAMÍLIA ITALIANA LTDA", "BAR KARAOKÊ FUN TIME LTDA", "HOTEL BOUTIQUE CHARM LTDA", "COZINHA FUSION ORIENTAL LTDA", "RESTAURANTE TERRAÇO PANORÂMICO LTDA", "APART HOTEL RESIDENCES LTDA", "BOTEQUIM CENTRAL EIRELI", "RESTAURANTE GRILL MASTER LTDA", "POUSADA DO SOL LTDA", "CAFETERIA XÍCARA & CIA EIRELI", "HOTEL BUSINESS EXECUTIVE LTDA", "CHOPERIA MALTE DOURADO LTDA", "RESTAURANTE VISTA LINDA LTDA" };
        var cidades = new[] { "São Paulo", "Guarulhos", "Osasco", "São Bernardo do Campo", "Santo André", "Campinas" };
        var regimes = new[] { "Simples Nacional", "Lucro Presumido", "Lucro Real" };
        var cnaes = new[] { "5510-8/01", "5611-2/01", "5611-2/03", "5590-6/01", "5612-1/00" };

        var contribuintes = razoesSociais.Select((nome, i) => new Contribuinte
        {
            EntidadeId = entidades[rng.Next(5)].Id,
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
                new() { Tipo = "Estabelecimento", Logradouro = "Rua das Flores", Numero = rng.Next(1, 999).ToString(), Bairro = "Centro", Cidade = cidades[rng.Next(cidades.Length)], Cep = $"01{rng.Next(100, 999)}-{rng.Next(100, 999)}" },
                new() { Tipo = "Cobrança", Logradouro = "Av. Paulista", Numero = rng.Next(1, 2000).ToString(), Bairro = "Bela Vista", Cidade = "São Paulo", Cep = "01310-100" }
            },
            Contatos = new List<Contato>
            {
                new() { Tipo = "Email", Valor = $"contato@empresa{i + 1}.com.br", Descricao = "E-mail principal" },
                new() { Tipo = "Telefone", Valor = $"(11) {rng.Next(2000, 9999)}-{rng.Next(1000, 9999)}", Descricao = "Telefone comercial" }
            },
            Historico = Enumerable.Range(1, 6).Select(m => new HistoricoMensal
            {
                Mes = DateTime.Today.AddMonths(-6 + m).Month,
                Ano = DateTime.Today.AddMonths(-6 + m).Year,
                CapitalSocial = Math.Round((decimal)(rng.NextDouble() * 990000 + 10000), 2),
                NumeroFuncionarios = rng.Next(1, 200)
            }).ToList()
        }).ToList();
        db.Contribuintes.AddRange(contribuintes);
        db.SaveChanges();

        // Stage 4: Cobrancas + Lancamentos + Processos (use saved IDs)
        var tipos = Enum.GetValues<TipoCobranca>();
        var cobrancas = new List<Cobranca>();
        for (int i = 0; i < 80; i++)
        {
            var contrib = contribuintes[rng.Next(contribuintes.Count)];
            var tipo = tipos[rng.Next(tipos.Length)];
            var vencimento = DateTime.Today.AddDays(rng.Next(-180, 60));
            var valor = Math.Round((decimal)(rng.NextDouble() * 2000 + 100), 2);
            var pago = rng.Next(3) == 0;
            var vencido = !pago && vencimento < DateTime.Today;
            cobrancas.Add(new Cobranca
            {
                EntidadeId = contrib.EntidadeId,
                ContribuinteId = contrib.Id,
                RazaoSocialContribuinte = contrib.RazaoSocial,
                Tipo = tipo,
                Valor = valor,
                Multa = vencido ? Math.Round(valor * 0.02m, 2) : 0,
                Juros = vencido ? Math.Round(valor * 0.01m * Math.Max(0m, (decimal)(DateTime.Today - vencimento).TotalDays / 30), 2) : 0,
                Vencimento = vencimento,
                DataPagamento = pago ? vencimento.AddDays(rng.Next(-5, 5)) : null,
                Status = pago ? StatusCobranca.Pago : vencido ? StatusCobranca.Vencido : StatusCobranca.Pendente,
                LinhaDigitavel = $"34191.79001 01043.510047 91020.15000{rng.Next(1, 9)} {rng.Next(1, 9)} {rng.Next(10000, 99999)}0000000",
                AnoReferencia = DateTime.Today.Year
            });
        }
        db.Cobrancas.AddRange(cobrancas);

        var categorias = new[] { "Aluguel", "Folha de Pagamento", "Energia Elétrica", "Telecomunicações", "Material de Escritório", "Serviços Terceirizados", "Impostos e Taxas" };
        var contas = new[] { "Santander — CC 12345-6", "Itaú — CC 67890-1", "Bradesco — CC 11223-4" };
        db.LancamentosFinanceiros.AddRange(Enumerable.Range(1, 30).Select(i =>
        {
            var isEntrada = rng.Next(4) == 0;
            var forn = isEntrada ? null : fornecedores[rng.Next(fornecedores.Count)];
            var data = DateTime.Today.AddDays(-rng.Next(0, 180));
            return new LancamentoFinanceiro
            {
                EntidadeId = entidades[rng.Next(5)].Id,
                Data = data,
                Categoria = isEntrada ? "Arrecadação de Contribuintes" : categorias[rng.Next(categorias.Length)],
                Descricao = isEntrada ? "Recebimento de boletos — lote automático" : $"Pagamento referente a {(forn?.Categoria ?? "serviço").ToLower()}",
                FornecedorId = forn?.Id,
                NomeFornecedor = forn?.Nome ?? "",
                Valor = Math.Round((decimal)(rng.NextDouble() * (isEntrada ? 50000 : 10000) + 500), 2),
                Tipo = isEntrada ? TipoLancamento.Entrada : TipoLancamento.Saida,
                ContaBancaria = contas[rng.Next(contas.Length)],
                Realizado = data <= DateTime.Today
            };
        }));

        var tiposP = new[] { "Reclamação trabalhista", "Ação de cobrança", "Mandado de segurança", "Ação civil pública", "Execução fiscal" };
        var varas = new[] { "1ª Vara do Trabalho", "2ª Vara do Trabalho", "3ª Vara Cível", "4ª Vara Federal" };
        var tribunais = new[] { "TRT 2ª Região", "TJSP", "TRF 3ª Região" };
        var situacoes = new[] { "Em andamento", "Em andamento", "Em andamento", "Suspenso", "Encerrado" };
        var processos = Enumerable.Range(1, 15).Select(i =>
        {
            var adv = advogados[rng.Next(advogados.Count)];
            return new Processo
            {
                EntidadeId = adv.EntidadeId,
                Numero = $"{rng.Next(1000, 9999)}-{rng.Next(10, 99)}.{DateTime.Today.Year}.5.02.{rng.Next(1000, 9999)}",
                Tipo = tiposP[rng.Next(tiposP.Length)],
                Vara = varas[rng.Next(varas.Length)],
                Tribunal = tribunais[rng.Next(tribunais.Length)],
                Situacao = situacoes[rng.Next(situacoes.Length)],
                AdvogadoId = adv.Id,
                NomeAdvogado = adv.Nome,
                DataAbertura = DateTime.Today.AddDays(-rng.Next(30, 1825)),
                Descricao = "Processo referente a ação judicial conforme documentação em arquivo."
            };
        }).ToList();
        db.Processos.AddRange(processos);
        db.SaveChanges();

        // Stage 5: Audiencias (needs processo IDs)
        var tiposA = new[] { "Inicial", "Instrução", "Julgamento", "Conciliação", "Perícia" };
        var locais = new[] { "Foro Central da Barra Funda", "Foro Regional da Lapa", "TRT 2ª Região", "TJSP — Pátio do Colégio" };
        var sitAud = new[] { "Agendada", "Agendada", "Realizada", "Cancelada" };
        db.Audiencias.AddRange(Enumerable.Range(1, 20).Select(i =>
        {
            var proc = processos[rng.Next(processos.Count)];
            var adv = advogados.First(a => a.Id == proc.AdvogadoId);
            return new Audiencia
            {
                ProcessoId = proc.Id,
                NumeroProcesso = proc.Numero,
                AdvogadoId = adv.Id,
                NomeAdvogado = adv.Nome,
                DataHora = DateTime.Today.AddDays(rng.Next(-30, 60)).Date.AddHours(rng.Next(8, 17)),
                Tipo = tiposA[rng.Next(tiposA.Length)],
                Local = locais[rng.Next(locais.Length)],
                Situacao = sitAud[rng.Next(sitAud.Length)]
            };
        }));

        db.Campanhas.AddRange(
            new Campanha { EntidadeId = entidades[0].Id, Assunto = "Comunicado — Assembleia Geral Ordinária 2026", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 1243, DataEnvio = DateTime.Today.AddDays(-30), Status = StatusCampanha.Enviada, Criador = "Ana Lima" },
            new Campanha { EntidadeId = entidades[0].Id, Assunto = "Boleto de Contribuição Sindical — Vencimento Abril/2026", Destinatarios = "Contribuintes ativos", TotalDestinatarios = 1180, DataEnvio = DateTime.Today.AddDays(-5), Status = StatusCampanha.Enviada, Criador = "Carlos Silva" },
            new Campanha { EntidadeId = entidades[1].Id, Assunto = "Edital — Eleição Diretoria 2026–2029", Destinatarios = "Sócios ativos", TotalDestinatarios = 420, DataEnvio = DateTime.Today.AddDays(7), Status = StatusCampanha.Agendada, Criador = "Fernanda Costa" },
            new Campanha { EntidadeId = entidades[1].Id, Assunto = "Novo Portal do Contribuinte — Acesse já!", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 8921, DataEnvio = null, Status = StatusCampanha.Rascunho, Criador = "Pedro Martins" },
            new Campanha { EntidadeId = entidades[2].Id, Assunto = "Convenção Coletiva 2026 — Resultado das Negociações", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 562, DataEnvio = DateTime.Today.AddDays(-15), Status = StatusCampanha.Enviada, Criador = "Lucia Fernandes" },
            new Campanha { EntidadeId = entidades[0].Id, Assunto = "Lembrete — Prazo Final Contribuição Confederativa", Destinatarios = "Contribuintes com cobrança pendente", TotalDestinatarios = 287, DataEnvio = DateTime.Today.AddDays(-2), Status = StatusCampanha.Erro, Criador = "Carlos Silva" },
            new Campanha { EntidadeId = entidades[3].Id, Assunto = "Curso de Capacitação — Gestão de Eventos 2026", Destinatarios = "Sócios ativos", TotalDestinatarios = 150, DataEnvio = null, Status = StatusCampanha.Rascunho, Criador = "Beatriz Nunes" },
            new Campanha { EntidadeId = entidades[1].Id, Assunto = "Relatório Mensal — Negociações Coletivas Março/2026", Destinatarios = "Contribuintes sócios", TotalDestinatarios = 320, DataEnvio = DateTime.Today.AddDays(14), Status = StatusCampanha.Agendada, Criador = "Fernanda Costa" },
            new Campanha { EntidadeId = entidades[0].Id, Assunto = "Atualização Cadastral Obrigatória — Prazo até 30/06", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 1243, DataEnvio = DateTime.Today.AddDays(-60), Status = StatusCampanha.Enviada, Criador = "Ana Lima" },
            new Campanha { EntidadeId = entidades[2].Id, Assunto = "Benefícios para Sócios — Novidades 2026", Destinatarios = "Contribuintes", TotalDestinatarios = 562, DataEnvio = null, Status = StatusCampanha.Rascunho, Criador = "Lucia Fernandes" }
        );

        var defaultHash = PasswordHelper.Hash("Senha@123");
        db.Usuarios.AddRange(
            new Usuario { Nome = "Ana Lima", Email = "ana.lima@sindhosp.org.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddMinutes(-15), Permissoes = new() { new() { EntidadeId = entidades[0].Id, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing" } }, new() { EntidadeId = entidades[4].Id, NomeEntidade = "FETURH", Modulos = new() { "Contribuintes" } } } },
            new Usuario { Nome = "Carlos Silva", Email = "carlos.silva@sindhosp.org.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddHours(-2), Permissoes = new() { new() { EntidadeId = entidades[0].Id, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Mailing" } } } },
            new Usuario { Nome = "Fernanda Costa", Email = "fernanda.costa@sindbar.org.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddHours(-1), Permissoes = new() { new() { EntidadeId = entidades[1].Id, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Usuários" } } } },
            new Usuario { Nome = "Pedro Martins", Email = "pedro.martins@sindbar.org.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddDays(-1), Permissoes = new() { new() { EntidadeId = entidades[1].Id, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Mailing" } } } },
            new Usuario { Nome = "Admin Sistema", Email = "admin@dpi.com.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddHours(-3), Permissoes = new() { new() { EntidadeId = entidades[0].Id, NomeEntidade = "SINDHOSP", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Entidades", "Usuários" } }, new() { EntidadeId = entidades[1].Id, NomeEntidade = "SINDBAR", Modulos = new() { "Contribuintes", "Cobranças", "Jurídico", "Financeiro", "Mailing", "Entidades", "Usuários" } } } },
            new Usuario { Nome = "Lucia Fernandes", Email = "lucia.fernandes@sindetur.org.br", SenhaHash = defaultHash, UltimoAcesso = DateTime.Now.AddDays(-2), Permissoes = new() { new() { EntidadeId = entidades[2].Id, NomeEntidade = "SINDETUR", Modulos = new() { "Contribuintes", "Cobranças", "Mailing" } }, new() { EntidadeId = entidades[3].Id, NomeEntidade = "SINDEVEN", Modulos = new() { "Contribuintes" } } } }
        );

        db.SaveChanges();
    }
}
