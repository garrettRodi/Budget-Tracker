using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _04__Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetContainers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Frequency = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetContainers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: false),
                    GroupName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BudgetItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BudgetContainerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    PlannedAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetItems_BudgetContainers_BudgetContainerId",
                        column: x => x.BudgetContainerId,
                        principalTable: "BudgetContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Incomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BudgetContainerId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incomes_BudgetContainers_BudgetContainerId",
                        column: x => x.BudgetContainerId,
                        principalTable: "BudgetContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlannedIncomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BudgetContainerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedIncomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlannedIncomes_BudgetContainers_BudgetContainerId",
                        column: x => x.BudgetContainerId,
                        principalTable: "BudgetContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavingGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GoalName = table.Column<string>(type: "TEXT", nullable: false),
                    TargetAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BudgetContainerId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavingGoals_BudgetContainers_BudgetContainerId",
                        column: x => x.BudgetContainerId,
                        principalTable: "BudgetContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    BudgetContainerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SavingGoalId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_BudgetContainers_BudgetContainerId",
                        column: x => x.BudgetContainerId,
                        principalTable: "BudgetContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_SavingGoals_SavingGoalId",
                        column: x => x.SavingGoalId,
                        principalTable: "SavingGoals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlannedExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SavingGoalId = table.Column<Guid>(type: "TEXT", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_PlannedExpenses_SavingGoals_SavingGoalId",
                        column: x => x.SavingGoalId,
                        principalTable: "SavingGoals",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Food" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Rent" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Entertainment" }
                });

            migrationBuilder.InsertData(
                table: "CategoryMappings",
                columns: new[] { "Id", "CategoryName", "GroupName" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Food", "Necessities" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Rent", "Necessities" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Entertainment", "Discretionary" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Investments", "Savings" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Utilities", "Necessities" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Transportation", "Necessities" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Healthcare", "Necessities" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "Clothing", "Discretionary" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Education", "Discretionary" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "Miscellaneous", "Discretionary" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_BudgetContainerId",
                table: "BudgetItems",
                column: "BudgetContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BudgetContainerId",
                table: "Expenses",
                column: "BudgetContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_SavingGoalId",
                table: "Expenses",
                column: "SavingGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_BudgetContainerId",
                table: "Incomes",
                column: "BudgetContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedExpenses_BudgetContainerId",
                table: "PlannedExpenses",
                column: "BudgetContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedExpenses_SavingGoalId",
                table: "PlannedExpenses",
                column: "SavingGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedIncomes_BudgetContainerId",
                table: "PlannedIncomes",
                column: "BudgetContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingGoals_BudgetContainerId",
                table: "SavingGoals",
                column: "BudgetContainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetItems");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "CategoryMappings");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Incomes");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "PlannedExpenses");

            migrationBuilder.DropTable(
                name: "PlannedIncomes");

            migrationBuilder.DropTable(
                name: "SavingGoals");

            migrationBuilder.DropTable(
                name: "BudgetContainers");
        }
    }
}
