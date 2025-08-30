using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _04__Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedBankAndCashBalances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Medium",
                table: "Incomes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Medium",
                table: "Expenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialBankBalance_Amount",
                table: "BudgetContainers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "InitialBankBalance_Currency",
                table: "BudgetContainers",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<decimal>(
                name: "InitialCashBalance_Amount",
                table: "BudgetContainers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "InitialCashBalance_Currency",
                table: "BudgetContainers",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BudgetContainerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    From = table.Column<int>(type: "INTEGER", nullable: false),
                    To = table.Column<int>(type: "INTEGER", nullable: false),
                    TransferAmount_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransferAmount_Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfers_BudgetContainers_BudgetContainerId",
                        column: x => x.BudgetContainerId,
                        principalTable: "BudgetContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_BudgetContainerId",
                table: "Transfers",
                column: "BudgetContainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropColumn(
                name: "Medium",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "Medium",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "InitialBankBalance_Amount",
                table: "BudgetContainers");

            migrationBuilder.DropColumn(
                name: "InitialBankBalance_Currency",
                table: "BudgetContainers");

            migrationBuilder.DropColumn(
                name: "InitialCashBalance_Amount",
                table: "BudgetContainers");

            migrationBuilder.DropColumn(
                name: "InitialCashBalance_Currency",
                table: "BudgetContainers");
        }
    }
}
