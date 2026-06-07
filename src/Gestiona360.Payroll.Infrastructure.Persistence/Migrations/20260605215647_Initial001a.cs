using System;
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
            migrationBuilder.AddColumn<Guid>(
                name: "CostCenterId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CostCenterId",
                table: "Employees",
                column: "CostCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_CostCenters_CostCenterId",
                table: "Employees",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_CostCenters_CostCenterId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CostCenterId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "Employees");
        }
    }
}
