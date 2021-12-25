using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class initialMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatGroups_ChatGroupForeignKey",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatGroupForeignKey",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatGroupForeignKey",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatGroupId",
                table: "Messages",
                column: "ChatGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatGroups_ChatGroupId",
                table: "Messages",
                column: "ChatGroupId",
                principalTable: "ChatGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatGroups_ChatGroupId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatGroupId",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "ChatGroupForeignKey",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatGroupForeignKey",
                table: "Messages",
                column: "ChatGroupForeignKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatGroups_ChatGroupForeignKey",
                table: "Messages",
                column: "ChatGroupForeignKey",
                principalTable: "ChatGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
