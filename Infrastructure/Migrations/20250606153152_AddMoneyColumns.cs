using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _04__Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoneyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentAmount_Amount",
                table: "SavingGoals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CurrentAmount_Currency",
                table: "SavingGoals",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<decimal>(
                name: "TargetAmount_Amount",
                table: "SavingGoals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TargetAmount_Currency",
                table: "SavingGoals",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount_Amount",
                table: "PlannedIncomes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "PlannedIncomes",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount_Amount",
                table: "PlannedExpenses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "PlannedExpenses",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualAmount_Amount",
                table: "Incomes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ActualAmount_Currency",
                table: "Incomes",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount_Amount",
                table: "Expenses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "Expenses",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualAmount_Amount",
                table: "BudgetItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ActualAmount_Currency",
                table: "BudgetItems",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<decimal>(
                name: "PlannedAmount_Amount",
                table: "BudgetItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PlannedAmount_Currency",
                table: "BudgetItems",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAmount_Amount",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "CurrentAmount_Currency",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "TargetAmount_Amount",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "TargetAmount_Currency",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "Amount_Amount",
                table: "PlannedIncomes");

            migrationBuilder.DropColumn(
                name: "Amount_Currency",
                table: "PlannedIncomes");

            migrationBuilder.DropColumn(
                name: "Amount_Amount",
                table: "PlannedExpenses");

            migrationBuilder.DropColumn(
                name: "Amount_Currency",
                table: "PlannedExpenses");

            migrationBuilder.DropColumn(
                name: "ActualAmount_Amount",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "ActualAmount_Currency",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "Amount_Amount",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Amount_Currency",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ActualAmount_Amount",
                table: "BudgetItems");

            migrationBuilder.DropColumn(
                name: "ActualAmount_Currency",
                table: "BudgetItems");

            migrationBuilder.DropColumn(
                name: "PlannedAmount_Amount",
                table: "BudgetItems");

            migrationBuilder.DropColumn(
                name: "PlannedAmount_Currency",
                table: "BudgetItems");
        }
    }
}
