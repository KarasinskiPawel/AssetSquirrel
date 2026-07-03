using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Equipments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_LocationId",
                table: "Equipments",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Locations_LocationId",
                table: "Equipments",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Locations_LocationId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_LocationId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Equipments");
        }
    }
}
