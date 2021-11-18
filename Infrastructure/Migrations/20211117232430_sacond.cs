using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class sacond : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Advertisements",
                columns: new[] { "Id", "DatePosted", "Description", "Photo", "Price", "Title" },
                values: new object[] { 1, null, "stive jobs apple", null, 11231, "apple" });

            migrationBuilder.InsertData(
                table: "Advertisements",
                columns: new[] { "Id", "DatePosted", "Description", "Photo", "Price", "Title" },
                values: new object[] { 2, null, "ramsar porteghal", null, 35, "orage" });

            migrationBuilder.InsertData(
                table: "Advertisements",
                columns: new[] { "Id", "DatePosted", "Description", "Photo", "Price", "Title" },
                values: new object[] { 3, null, "good for family", null, 12321, "Pride" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Advertisements",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Advertisements",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Advertisements",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
