using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorTeste.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupBEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // AlterColumn to remove IDENTITY is not supported by SQL Server via ALTER COLUMN.
            // The schema already matches the target state; this is intentionally a no-op.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
