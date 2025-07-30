using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class SeedComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "CreatedOn", "StockId", "Title" },
                values: new object[,]
                {
                    { 1, "Apple has been performing really well this quarter.", new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Great stock!" },
                    { 2, "Microsoft's dividend is very attractive for long-term investors.", new DateTime(2024, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Solid dividends" },
                    { 3, "Tesla's price swings a lot, but the growth is impressive.", new DateTime(2024, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Volatile" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
