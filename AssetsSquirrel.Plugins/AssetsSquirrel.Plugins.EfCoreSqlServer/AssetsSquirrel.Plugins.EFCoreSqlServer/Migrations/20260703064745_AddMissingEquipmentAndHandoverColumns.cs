using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingEquipmentAndHandoverColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAddedToWarehouse",
                table: "Equipments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPosted",
                table: "EquipmentHandovers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAddedToWarehouse",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "IsPosted",
                table: "EquipmentHandovers");
        }
    }
}
