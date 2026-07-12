using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyMediChain.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditResultColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Result",
                table: "AuditLogs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "AuditLogs");
        }
    }
}
