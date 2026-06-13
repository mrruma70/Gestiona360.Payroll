using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestiona360.Payroll.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial001a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoBarra",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoBarra",
                table: "Employees");
        }
    }
}
