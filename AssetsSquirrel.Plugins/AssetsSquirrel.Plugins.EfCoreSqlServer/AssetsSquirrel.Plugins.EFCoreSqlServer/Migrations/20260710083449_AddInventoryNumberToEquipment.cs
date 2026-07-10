using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryNumberToEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InventoryNumber",
                table: "Equipments",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            // Backfill existing rows with sequential numbers ("491" + 8 digits, same
            // format AddEquipmentUseCase.InventoryNumberGenerator uses going forward)
            // before the column becomes NOT NULL + unique -- can't leave them all
            // blank/equal to each other.
            migrationBuilder.Sql(@"
                WITH Numbered AS (
                    SELECT EquipmentId, ROW_NUMBER() OVER (ORDER BY EquipmentId) AS RowNum
                    FROM Equipments
                )
                UPDATE e
                SET e.InventoryNumber = '491' + RIGHT('00000000' + CAST(n.RowNum AS varchar(8)), 8)
                FROM Equipments e
                JOIN Numbered n ON e.EquipmentId = n.EquipmentId;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "InventoryNumber",
                table: "Equipments",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_InventoryNumber",
                table: "Equipments",
                column: "InventoryNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Equipments_InventoryNumber",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "InventoryNumber",
                table: "Equipments");
        }
    }
}
