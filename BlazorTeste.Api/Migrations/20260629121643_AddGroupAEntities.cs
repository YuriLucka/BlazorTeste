using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorTeste.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupAEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuiaSindicais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadeId = table.Column<int>(type: "int", nullable: false),
                    ContribuinteId = table.Column<int>(type: "int", nullable: false),
                    RazaoSocialContribuinte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnoReferencia = table.Column<int>(type: "int", nullable: false),
                    ValorBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Multa = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Juros = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Vencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LinhaDigitavel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuiaSindicais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosBaixa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadeId = table.Column<int>(type: "int", nullable: false),
                    CobrancaId = table.Column<int>(type: "int", nullable: false),
                    RazaoSocialContribuinte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoCobranca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorOriginal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorPago = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataProcessamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoBaixa = table.Column<int>(type: "int", nullable: false),
                    OperadorResponsavel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosBaixa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relatorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<int>(type: "int", nullable: false),
                    Formato = table.Column<int>(type: "int", nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relatorios", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuiaSindicais");

            migrationBuilder.DropTable(
                name: "RegistrosBaixa");

            migrationBuilder.DropTable(
                name: "Relatorios");
        }
    }
}
