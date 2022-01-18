using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "photo2",
                table: "Advertisements",
                newName: "Photo2");

            migrationBuilder.RenameColumn(
                name: "photo1",
                table: "Advertisements",
                newName: "Photo1");

            migrationBuilder.RenameColumn(
                name: "listviewPhoto",
                table: "Advertisements",
                newName: "ListViewPhoto");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Photo2",
                table: "Advertisements",
                newName: "photo2");

            migrationBuilder.RenameColumn(
                name: "Photo1",
                table: "Advertisements",
                newName: "photo1");

            migrationBuilder.RenameColumn(
                name: "ListViewPhoto",
                table: "Advertisements",
                newName: "listviewPhoto");
        }
    }
}
