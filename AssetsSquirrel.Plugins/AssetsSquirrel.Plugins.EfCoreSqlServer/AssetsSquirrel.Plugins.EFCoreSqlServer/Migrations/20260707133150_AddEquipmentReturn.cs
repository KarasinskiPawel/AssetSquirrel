using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentReturn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EquipmentReturnId",
                table: "EquipmentAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EquipmentReturns",
                columns: table => new
                {
                    EquipmentReturnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnDocumentNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StorageLocationId = table.Column<int>(type: "int", nullable: false),
                    PreparedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentReturns", x => x.EquipmentReturnId);
                    table.ForeignKey(
                        name: "FK_EquipmentReturns_AspNetUsers_PreparedByUserId",
                        column: x => x.PreparedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentReturns_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentReturns_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentReturns_Locations_StorageLocationId",
                        column: x => x.StorageLocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignments_EquipmentReturnId",
                table: "EquipmentAssignments",
                column: "EquipmentReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReturns_EmployeeId",
                table: "EquipmentReturns",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReturns_LocationId",
                table: "EquipmentReturns",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReturns_PreparedByUserId",
                table: "EquipmentReturns",
                column: "PreparedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReturns_ReturnDocumentNumber",
                table: "EquipmentReturns",
                column: "ReturnDocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReturns_StorageLocationId",
                table: "EquipmentReturns",
                column: "StorageLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignments_EquipmentReturns_EquipmentReturnId",
                table: "EquipmentAssignments",
                column: "EquipmentReturnId",
                principalTable: "EquipmentReturns",
                principalColumn: "EquipmentReturnId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignments_EquipmentReturns_EquipmentReturnId",
                table: "EquipmentAssignments");

            migrationBuilder.DropTable(
                name: "EquipmentReturns");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentAssignments_EquipmentReturnId",
                table: "EquipmentAssignments");

            migrationBuilder.DropColumn(
                name: "EquipmentReturnId",
                table: "EquipmentAssignments");
        }
    }
}
