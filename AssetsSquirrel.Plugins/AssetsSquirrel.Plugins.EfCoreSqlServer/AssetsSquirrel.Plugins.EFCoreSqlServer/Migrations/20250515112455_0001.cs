using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class _0001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DictionaryEquipments");

            migrationBuilder.CreateTable(
                name: "HardwareTypes",
                columns: table => new
                {
                    HardwareTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HardwareTypes", x => x.HardwareTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MPK = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    ManufacturerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.ManufacturerId);
                });

            migrationBuilder.CreateTable(
                name: "Suppilers",
                columns: table => new
                {
                    SuppilerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppilers", x => x.SuppilerId);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuppilerId = table.Column<int>(type: "int", nullable: true),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true),
                    HardwareTypeId = table.Column<int>(type: "int", nullable: true),
                    ModelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.EquipmentId);
                    table.ForeignKey(
                        name: "FK_Equipments_HardwareTypes_HardwareTypeId",
                        column: x => x.HardwareTypeId,
                        principalTable: "HardwareTypes",
                        principalColumn: "HardwareTypeId");
                    table.ForeignKey(
                        name: "FK_Equipments_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerId");
                    table.ForeignKey(
                        name: "FK_Equipments_Suppilers_SuppilerId",
                        column: x => x.SuppilerId,
                        principalTable: "Suppilers",
                        principalColumn: "SuppilerId");
                });

            migrationBuilder.InsertData(
                table: "HardwareTypes",
                columns: new[] { "HardwareTypeId", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "", true, "Komputer" },
                    { 2, "", true, "Laptop" },
                    { 3, "", true, "Monitor" },
                    { 4, "", true, "Mysz" },
                    { 5, "", true, "Mysz optyczna" },
                    { 6, "", true, "Klawiatura" },
                    { 7, "", true, "Drukarka" },
                    { 8, "", true, "Drukarka fiskalna" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "LocationId", "City", "Code", "Email", "IsActive", "MPK", "PhoneNumber", "Street" },
                values: new object[,]
                {
                    { 1, "Stryków", "M100", "", true, "PL1M100Z", "", "Magazyn Centralny" },
                    { 2, "Łódź", "S000", "", true, "PL1C001Z", "", "Biuro - Srebrzyńska 14" },
                    { 3, "Łódź", "N001", "", true, "PL1N001Z", "", "Magazyn IT - Srebrzyńska 14" }
                });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "ManufacturerId", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "", true, "Asus" },
                    { 2, "", true, "Acer" },
                    { 3, "", true, "HP" },
                    { 4, "", true, "Lenovo" },
                    { 5, "", true, "Cisco" }
                });

            migrationBuilder.InsertData(
                table: "Suppilers",
                columns: new[] { "SuppilerId", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "", true, "X-KOM" },
                    { 2, "", true, "MPC" },
                    { 3, "", true, "Lantre" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_HardwareTypeId",
                table: "Equipments",
                column: "HardwareTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ManufacturerId",
                table: "Equipments",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_SuppilerId",
                table: "Equipments",
                column: "SuppilerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "HardwareTypes");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "Suppilers");

            migrationBuilder.CreateTable(
                name: "DictionaryEquipments",
                columns: table => new
                {
                    DictionaryEquipmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
    }
}
