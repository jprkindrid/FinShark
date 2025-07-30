using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class SeedStocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "CompanyName", "Industry", "LastDiv", "MarketCap", "Price", "Symbol" },
                values: new object[,]
                {
                    { 1, "Apple Inc.", "Technology", 0.24m, 3000000000000L, 195.34m, "AAPL" },
                    { 2, "Microsoft Corporation", "Technology", 0.68m, 2800000000000L, 410.22m, "MSFT" },
                    { 3, "Tesla, Inc.", "Automotive", 0.00m, 800000000000L, 250.12m, "TSLA" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
