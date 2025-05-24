using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _04__Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlannedExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlannedExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BudgetContainerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Period = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlannedExpenses_BudgetContainers_BudgetContainerId",
                        column: x => x.BudgetContainerId,
                        principalTable: "BudgetContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlannedExpenses_BudgetContainerId",
                table: "PlannedExpenses",
                column: "BudgetContainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlannedExpenses");
        }
    }
}
