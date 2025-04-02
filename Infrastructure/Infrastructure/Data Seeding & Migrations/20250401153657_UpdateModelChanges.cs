using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _04__Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Incomes",
                newName: "ActualAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActualAmount",
                table: "Incomes",
                newName: "Amount");
        }
    }
}
