using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _04__Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlannedIncomeAndExpenseServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "PlannedIncomes");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PlannedIncomes",
                newName: "Source");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Source",
                table: "PlannedIncomes",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "PlannedIncomes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
