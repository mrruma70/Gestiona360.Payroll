using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestiona360.Payroll.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial001b : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "Municipalities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Municipalities");
        }
    }
}
