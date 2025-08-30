using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _04__Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedBaseCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "BudgetContainers",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "BudgetContainers");
        }
    }
}
