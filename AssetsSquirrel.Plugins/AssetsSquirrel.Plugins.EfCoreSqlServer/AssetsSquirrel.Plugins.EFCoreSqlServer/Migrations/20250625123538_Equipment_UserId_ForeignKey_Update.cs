using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Equipment_UserId_ForeignKey_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_AspNetUsers_ApplicationUserId",
                table: "Equipments");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Equipments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_ApplicationUserId",
                table: "Equipments",
                newName: "IX_Equipments_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_AspNetUsers_UserId",
                table: "Equipments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_AspNetUsers_UserId",
                table: "Equipments");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Equipments",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_UserId",
                table: "Equipments",
                newName: "IX_Equipments_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_AspNetUsers_ApplicationUserId",
                table: "Equipments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
