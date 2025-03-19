using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _04__Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStaticSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "CategoryMappings",
                columns: new[] { "Id", "CategoryName", "GroupName" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Food", "Necessities" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Rent", "Necessities" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Entertainment", "Discretionary" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Investments", "Savings" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryMappings");
        }
    }
}
