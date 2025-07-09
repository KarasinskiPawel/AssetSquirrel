using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class EquipmentAssignement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentAssignment",
                columns: table => new
                {
                    EquipmentAssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    DateOfHandover = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfReturn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentAssignment", x => x.EquipmentAssignmentId);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignment_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignment_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignment_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignment_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentAssignmentHistory",
                columns: table => new
                {
                    EquipmentAssignmentHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentAssignmentId = table.Column<int>(type: "int", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    DateOfHandover = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfReturn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentAssignmentHistory", x => x.EquipmentAssignmentHistoryId);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignmentHistory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignmentHistory_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignmentHistory_EquipmentAssignment_EquipmentAssignmentId",
                        column: x => x.EquipmentAssignmentId,
                        principalTable: "EquipmentAssignment",
                        principalColumn: "EquipmentAssignmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignmentHistory_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignmentHistory_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignment_EmployeeId",
                table: "EquipmentAssignment",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignment_EquipmentId",
                table: "EquipmentAssignment",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignment_LocationId",
                table: "EquipmentAssignment",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignment_UserId",
                table: "EquipmentAssignment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignmentHistory_EmployeeId",
                table: "EquipmentAssignmentHistory",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignmentHistory_EquipmentAssignmentId",
                table: "EquipmentAssignmentHistory",
                column: "EquipmentAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignmentHistory_EquipmentId",
                table: "EquipmentAssignmentHistory",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignmentHistory_LocationId",
                table: "EquipmentAssignmentHistory",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignmentHistory_UserId",
                table: "EquipmentAssignmentHistory",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentAssignmentHistory");

            migrationBuilder.DropTable(
                name: "EquipmentAssignment");
        }
    }
}
