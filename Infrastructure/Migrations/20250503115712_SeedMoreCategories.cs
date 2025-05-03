using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _04__Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CategoryMappings",
                columns: new[] { "Id", "CategoryName", "GroupName" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Utilities", "Necessities" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Transportation", "Necessities" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Healthcare", "Necessities" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "Clothing", "Discretionary" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Education", "Discretionary" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "Miscellaneous", "Discretionary" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CategoryMappings",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "CategoryMappings",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "CategoryMappings",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "CategoryMappings",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "CategoryMappings",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "CategoryMappings",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"));
        }
    }
}
