using System;
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
            migrationBuilder.DropColumn(
                name: "TerminationReason",
                table: "Terminations");

            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "OldValue",
                table: "PersonalActions");

            migrationBuilder.AlterColumn<int>(
                name: "TerminationType",
                table: "Terminations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentPDF",
                table: "Terminations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Guid>(
                name: "LinkedPersonalActionId",
                table: "Terminations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PersonalActions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "ActionType",
                table: "PersonalActions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "AppliedInPayrollDate",
                table: "PersonalActions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AppliedInPayrollRecordId",
                table: "PersonalActions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BatchReference",
                table: "PersonalActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CausalDescription",
                table: "PersonalActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAppliedInPayroll",
                table: "PersonalActions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "NewBaseSalary",
                table: "PersonalActions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewContractTypeId",
                table: "PersonalActions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NewCostCenterId",
                table: "PersonalActions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewEmploymentStatus",
                table: "PersonalActions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NewJobGradeId",
                table: "PersonalActions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NewShiftId",
                table: "PersonalActions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldBaseSalary",
                table: "PersonalActions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldContractTypeId",
                table: "PersonalActions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OldCostCenterId",
                table: "PersonalActions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldEmploymentStatus",
                table: "PersonalActions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OldJobGradeId",
                table: "PersonalActions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OldShiftId",
                table: "PersonalActions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PayrollGroupId",
                table: "PersonalActions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TargetPayrollPeriodId",
                table: "PersonalActions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "SuspensionType",
                table: "EmployeeSuspensions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "LinkedPersonalActionId",
                table: "EmployeeSuspensions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Terminations_LinkedPersonalActionId",
                table: "Terminations",
                column: "LinkedPersonalActionId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalActions_AppliedInPayrollRecordId",
                table: "PersonalActions",
                column: "AppliedInPayrollRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalActions_PayrollGroupId",
                table: "PersonalActions",
                column: "PayrollGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalActions_TargetPayrollPeriodId",
                table: "PersonalActions",
                column: "TargetPayrollPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSuspensions_LinkedPersonalActionId",
                table: "EmployeeSuspensions",
                column: "LinkedPersonalActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSuspensions_PersonalActions_LinkedPersonalActionId",
                table: "EmployeeSuspensions",
                column: "LinkedPersonalActionId",
                principalTable: "PersonalActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalActions_PayrollGroups_PayrollGroupId",
                table: "PersonalActions",
                column: "PayrollGroupId",
                principalTable: "PayrollGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalActions_PayrollPeriods_TargetPayrollPeriodId",
                table: "PersonalActions",
                column: "TargetPayrollPeriodId",
                principalTable: "PayrollPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalActions_PayrollRecords_AppliedInPayrollRecordId",
                table: "PersonalActions",
                column: "AppliedInPayrollRecordId",
                principalTable: "PayrollRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Terminations_PersonalActions_LinkedPersonalActionId",
                table: "Terminations",
                column: "LinkedPersonalActionId",
                principalTable: "PersonalActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSuspensions_PersonalActions_LinkedPersonalActionId",
                table: "EmployeeSuspensions");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalActions_PayrollGroups_PayrollGroupId",
                table: "PersonalActions");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalActions_PayrollPeriods_TargetPayrollPeriodId",
                table: "PersonalActions");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalActions_PayrollRecords_AppliedInPayrollRecordId",
                table: "PersonalActions");

            migrationBuilder.DropForeignKey(
                name: "FK_Terminations_PersonalActions_LinkedPersonalActionId",
                table: "Terminations");

            migrationBuilder.DropIndex(
                name: "IX_Terminations_LinkedPersonalActionId",
                table: "Terminations");

            migrationBuilder.DropIndex(
                name: "IX_PersonalActions_AppliedInPayrollRecordId",
                table: "PersonalActions");

            migrationBuilder.DropIndex(
                name: "IX_PersonalActions_PayrollGroupId",
                table: "PersonalActions");

            migrationBuilder.DropIndex(
                name: "IX_PersonalActions_TargetPayrollPeriodId",
                table: "PersonalActions");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeSuspensions_LinkedPersonalActionId",
                table: "EmployeeSuspensions");

            migrationBuilder.DropColumn(
                name: "LinkedPersonalActionId",
                table: "Terminations");

            migrationBuilder.DropColumn(
                name: "AppliedInPayrollDate",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "AppliedInPayrollRecordId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "BatchReference",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "CausalDescription",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "IsAppliedInPayroll",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "NewBaseSalary",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "NewContractTypeId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "NewCostCenterId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "NewEmploymentStatus",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "NewJobGradeId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "NewShiftId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "OldBaseSalary",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "OldContractTypeId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "OldCostCenterId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "OldEmploymentStatus",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "OldJobGradeId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "OldShiftId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "PayrollGroupId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "TargetPayrollPeriodId",
                table: "PersonalActions");

            migrationBuilder.DropColumn(
                name: "LinkedPersonalActionId",
                table: "EmployeeSuspensions");

            migrationBuilder.AlterColumn<string>(
                name: "TerminationType",
                table: "Terminations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentPDF",
                table: "Terminations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "TerminationReason",
                table: "Terminations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PersonalActions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "PersonalActions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "PersonalActions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OldValue",
                table: "PersonalActions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SuspensionType",
                table: "EmployeeSuspensions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
