using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Equipments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "EquipmentHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ApplicationUserId",
                table: "Equipments",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHistories_ApplicationUserId",
                table: "EquipmentHistories",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentHistories_AspNetUsers_ApplicationUserId",
                table: "EquipmentHistories",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_AspNetUsers_ApplicationUserId",
                table: "Equipments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentHistories_AspNetUsers_ApplicationUserId",
                table: "EquipmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_AspNetUsers_ApplicationUserId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_ApplicationUserId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentHistories_ApplicationUserId",
                table: "EquipmentHistories");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "EquipmentHistories");
        }
    }
}
