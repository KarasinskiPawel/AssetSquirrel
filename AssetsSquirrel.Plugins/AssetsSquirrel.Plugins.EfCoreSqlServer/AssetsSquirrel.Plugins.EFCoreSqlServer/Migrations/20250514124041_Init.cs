using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DictionaryEquipments",
                columns: table => new
                {
                    DictionaryEquipmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryEquipments", x => x.DictionaryEquipmentId);
                });

            migrationBuilder.InsertData(
                table: "DictionaryEquipments",
                columns: new[] { "DictionaryEquipmentId", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "", true, "AccessPoint" },
                    { 2, "", true, "Drukarka etykiet" },
                    { 3, "", true, "Drukarka fiskalna" },
                    { 4, "", true, "Dysk zewnętrzny" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DictionaryEquipments");
        }
    }
}
