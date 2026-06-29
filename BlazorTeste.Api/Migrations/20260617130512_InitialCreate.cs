using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorTeste.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Advogados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadeId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Oab = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessosAtivos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advogados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audiencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessoId = table.Column<int>(type: "int", nullable: false),
                    NumeroProcesso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdvogadoId = table.Column<int>(type: "int", nullable: false),
                    NomeAdvogado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Local = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Situacao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audiencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campanhas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadeId = table.Column<int>(type: "int", nullable: false),
                    Assunto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destinatarios = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalDestinatarios = table.Column<int>(type: "int", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Criador = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campanhas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cobrancas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadeId = table.Column<int>(type: "int", nullable: false),
                    ContribuinteId = table.Column<int>(type: "int", nullable: false),
                    RazaoSocialContribuinte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Multa = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Juros = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Vencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LinhaDigitavel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnoReferencia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cobrancas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contribuintes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadeId = table.Column<int>(type: "int", nullable: false),
                    RazaoSocial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cnae = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapitalSocial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumeroFuncionarios = table.Column<int>(type: "int", nullable: false),
                    RegimeTributario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Situacao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contribuintes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sigla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CidadeSede = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalContribuintes = table.Column<int>(type: "int", nullable: false),
                    Cnaes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidades = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadeId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LancamentosFinanceiros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadeId = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FornecedorId = table.Column<int>(type: "int", nullable: true),
                    NomeFornecedor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    ContaBancaria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Realizado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LancamentosFinanceiros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Processos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadeId = table.Column<int>(type: "int", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vara = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tribunal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Situacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdvogadoId = table.Column<int>(type: "int", nullable: false),
                    NomeAdvogado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UltimoAcesso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Permissoes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContribuinteId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contatos", x => new { x.ContribuinteId, x.Id });
                    table.ForeignKey(
                        name: "FK_Contatos_Contribuintes_ContribuinteId",
                        column: x => x.ContribuinteId,
                        principalTable: "Contribuintes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContribuinteId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logradouro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => new { x.ContribuinteId, x.Id });
                    table.ForeignKey(
                        name: "FK_Enderecos_Contribuintes_ContribuinteId",
                        column: x => x.ContribuinteId,
                        principalTable: "Contribuintes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoMensais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContribuinteId = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    CapitalSocial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumeroFuncionarios = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoMensais", x => new { x.ContribuinteId, x.Id });
                    table.ForeignKey(
                        name: "FK_HistoricoMensais_Contribuintes_ContribuinteId",
                        column: x => x.ContribuinteId,
                        principalTable: "Contribuintes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Socios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContribuinteId = table.Column<int>(type: "int", nullable: false),
                    Matricula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValorMensalidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Socios", x => new { x.ContribuinteId, x.Id });
                    table.ForeignKey(
                        name: "FK_Socios_Contribuintes_ContribuinteId",
                        column: x => x.ContribuinteId,
                        principalTable: "Contribuintes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advogados");

            migrationBuilder.DropTable(
                name: "Audiencias");

            migrationBuilder.DropTable(
                name: "Campanhas");

            migrationBuilder.DropTable(
                name: "Cobrancas");

            migrationBuilder.DropTable(
                name: "Contatos");

            migrationBuilder.DropTable(
                name: "Enderecos");

            migrationBuilder.DropTable(
                name: "Entidades");

            migrationBuilder.DropTable(
                name: "Fornecedores");

            migrationBuilder.DropTable(
                name: "HistoricoMensais");

            migrationBuilder.DropTable(
                name: "LancamentosFinanceiros");

            migrationBuilder.DropTable(
                name: "Processos");

            migrationBuilder.DropTable(
                name: "Socios");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Contribuintes");
        }
    }
}
