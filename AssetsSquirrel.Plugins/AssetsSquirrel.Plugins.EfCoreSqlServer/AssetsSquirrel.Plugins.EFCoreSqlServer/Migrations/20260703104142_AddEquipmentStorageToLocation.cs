using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentStorageToLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EquipmentStorage",
                table: "Locations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "LocationId",
                keyValue: 1,
                column: "EquipmentStorage",
                value: false);

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "LocationId",
                keyValue: 2,
                column: "EquipmentStorage",
                value: false);

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "LocationId",
                keyValue: 3,
                column: "EquipmentStorage",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipmentStorage",
                table: "Locations");
        }
    }
}
