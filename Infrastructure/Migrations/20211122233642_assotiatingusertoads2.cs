using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class assotiatingusertoads2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advertisements_AspNetUsers_UserNameId",
                table: "Advertisements");

            migrationBuilder.DropIndex(
                name: "IX_Advertisements_UserNameId",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "UserNameId",
                table: "Advertisements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserNameId",
                table: "Advertisements",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_UserNameId",
                table: "Advertisements",
                column: "UserNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advertisements_AspNetUsers_UserNameId",
                table: "Advertisements",
                column: "UserNameId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
