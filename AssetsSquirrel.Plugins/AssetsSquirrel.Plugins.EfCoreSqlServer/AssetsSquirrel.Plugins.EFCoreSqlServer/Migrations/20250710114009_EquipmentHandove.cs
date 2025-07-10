using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class EquipmentHandove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignment_AspNetUsers_UserId",
                table: "EquipmentAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignment_Employees_EmployeeId",
                table: "EquipmentAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignment_Equipments_EquipmentId",
                table: "EquipmentAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignment_Locations_LocationId",
                table: "EquipmentAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignmentHistory_AspNetUsers_UserId",
                table: "EquipmentAssignmentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignmentHistory_Employees_EmployeeId",
                table: "EquipmentAssignmentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignmentHistory_EquipmentAssignment_EquipmentAssignmentId",
                table: "EquipmentAssignmentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignmentHistory_Equipments_EquipmentId",
                table: "EquipmentAssignmentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignmentHistory_Locations_LocationId",
                table: "EquipmentAssignmentHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentAssignmentHistory",
                table: "EquipmentAssignmentHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentAssignment",
                table: "EquipmentAssignment");

            migrationBuilder.RenameTable(
                name: "EquipmentAssignmentHistory",
                newName: "EquipmentAssignmentHistories");

            migrationBuilder.RenameTable(
                name: "EquipmentAssignment",
                newName: "EquipmentAssignments");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignmentHistory_UserId",
                table: "EquipmentAssignmentHistories",
                newName: "IX_EquipmentAssignmentHistories_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignmentHistory_LocationId",
                table: "EquipmentAssignmentHistories",
                newName: "IX_EquipmentAssignmentHistories_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignmentHistory_EquipmentId",
                table: "EquipmentAssignmentHistories",
                newName: "IX_EquipmentAssignmentHistories_EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignmentHistory_EquipmentAssignmentId",
                table: "EquipmentAssignmentHistories",
                newName: "IX_EquipmentAssignmentHistories_EquipmentAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignmentHistory_EmployeeId",
                table: "EquipmentAssignmentHistories",
                newName: "IX_EquipmentAssignmentHistories_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignment_UserId",
                table: "EquipmentAssignments",
                newName: "IX_EquipmentAssignments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignment_LocationId",
                table: "EquipmentAssignments",
                newName: "IX_EquipmentAssignments_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignment_EquipmentId",
                table: "EquipmentAssignments",
                newName: "IX_EquipmentAssignments_EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignment_EmployeeId",
                table: "EquipmentAssignments",
                newName: "IX_EquipmentAssignments_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentAssignmentHistories",
                table: "EquipmentAssignmentHistories",
                column: "EquipmentAssignmentHistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentAssignments",
                table: "EquipmentAssignments",
                column: "EquipmentAssignmentId");

            migrationBuilder.CreateTable(
                name: "EquipmentHandovers",
                columns: table => new
                {
                    EquipmentHandoverId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HandoverDocumentNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    FromLocationId = table.Column<int>(type: "int", nullable: false),
                    ToLocationId = table.Column<int>(type: "int", nullable: true),
                    FromEmployeeId = table.Column<int>(type: "int", nullable: true),
                    ToEmployeeId = table.Column<int>(type: "int", nullable: false),
                    HandoverDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentHandovers", x => x.EquipmentHandoverId);
                    table.ForeignKey(
                        name: "FK_EquipmentHandovers_Employees_FromEmployeeId",
                        column: x => x.FromEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentHandovers_Employees_ToEmployeeId",
                        column: x => x.ToEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentHandovers_Locations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentHandovers_Locations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentHandoverDetails",
                columns: table => new
                {
                    EquipmentHandoverDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentHandoverId = table.Column<int>(type: "int", nullable: false),
                    HardwareTypeId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentHandoverDetails", x => x.EquipmentHandoverDetailId);
                    table.ForeignKey(
                        name: "FK_EquipmentHandoverDetails_EquipmentHandovers_EquipmentHandoverId",
                        column: x => x.EquipmentHandoverId,
                        principalTable: "EquipmentHandovers",
                        principalColumn: "EquipmentHandoverId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentHandoverDetails_HardwareTypes_HardwareTypeId",
                        column: x => x.HardwareTypeId,
                        principalTable: "HardwareTypes",
                        principalColumn: "HardwareTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHandoverDetails_EquipmentHandoverId",
                table: "EquipmentHandoverDetails",
                column: "EquipmentHandoverId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHandoverDetails_HardwareTypeId",
                table: "EquipmentHandoverDetails",
                column: "HardwareTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHandovers_FromEmployeeId",
                table: "EquipmentHandovers",
                column: "FromEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHandovers_FromLocationId",
                table: "EquipmentHandovers",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHandovers_ToEmployeeId",
                table: "EquipmentHandovers",
                column: "ToEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHandovers_ToLocationId",
                table: "EquipmentHandovers",
                column: "ToLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignmentHistories_AspNetUsers_UserId",
                table: "EquipmentAssignmentHistories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignmentHistories_Employees_EmployeeId",
                table: "EquipmentAssignmentHistories",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignmentHistories_EquipmentAssignments_EquipmentAssignmentId",
                table: "EquipmentAssignmentHistories",
                column: "EquipmentAssignmentId",
                principalTable: "EquipmentAssignments",
                principalColumn: "EquipmentAssignmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignmentHistories_Equipments_EquipmentId",
                table: "EquipmentAssignmentHistories",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignmentHistories_Locations_LocationId",
                table: "EquipmentAssignmentHistories",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignments_AspNetUsers_UserId",
                table: "EquipmentAssignments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignments_Employees_EmployeeId",
                table: "EquipmentAssignments",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignments_Equipments_EquipmentId",
                table: "EquipmentAssignments",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignments_Locations_LocationId",
                table: "EquipmentAssignments",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignmentHistories_AspNetUsers_UserId",
                table: "EquipmentAssignmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignmentHistories_Employees_EmployeeId",
                table: "EquipmentAssignmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignmentHistories_EquipmentAssignments_EquipmentAssignmentId",
                table: "EquipmentAssignmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignmentHistories_Equipments_EquipmentId",
                table: "EquipmentAssignmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignmentHistories_Locations_LocationId",
                table: "EquipmentAssignmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignments_AspNetUsers_UserId",
                table: "EquipmentAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignments_Employees_EmployeeId",
                table: "EquipmentAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignments_Equipments_EquipmentId",
                table: "EquipmentAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignments_Locations_LocationId",
                table: "EquipmentAssignments");

            migrationBuilder.DropTable(
                name: "EquipmentHandoverDetails");

            migrationBuilder.DropTable(
                name: "EquipmentHandovers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentAssignments",
                table: "EquipmentAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentAssignmentHistories",
                table: "EquipmentAssignmentHistories");

            migrationBuilder.RenameTable(
                name: "EquipmentAssignments",
                newName: "EquipmentAssignment");

            migrationBuilder.RenameTable(
                name: "EquipmentAssignmentHistories",
                newName: "EquipmentAssignmentHistory");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignments_UserId",
                table: "EquipmentAssignment",
                newName: "IX_EquipmentAssignment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignments_LocationId",
                table: "EquipmentAssignment",
                newName: "IX_EquipmentAssignment_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignments_EquipmentId",
                table: "EquipmentAssignment",
                newName: "IX_EquipmentAssignment_EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignments_EmployeeId",
                table: "EquipmentAssignment",
                newName: "IX_EquipmentAssignment_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignmentHistories_UserId",
                table: "EquipmentAssignmentHistory",
                newName: "IX_EquipmentAssignmentHistory_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignmentHistories_LocationId",
                table: "EquipmentAssignmentHistory",
                newName: "IX_EquipmentAssignmentHistory_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignmentHistories_EquipmentId",
                table: "EquipmentAssignmentHistory",
                newName: "IX_EquipmentAssignmentHistory_EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignmentHistories_EquipmentAssignmentId",
                table: "EquipmentAssignmentHistory",
                newName: "IX_EquipmentAssignmentHistory_EquipmentAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAssignmentHistories_EmployeeId",
                table: "EquipmentAssignmentHistory",
                newName: "IX_EquipmentAssignmentHistory_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentAssignment",
                table: "EquipmentAssignment",
                column: "EquipmentAssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentAssignmentHistory",
                table: "EquipmentAssignmentHistory",
                column: "EquipmentAssignmentHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignment_AspNetUsers_UserId",
                table: "EquipmentAssignment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignment_Employees_EmployeeId",
                table: "EquipmentAssignment",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignment_Equipments_EquipmentId",
                table: "EquipmentAssignment",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignment_Locations_LocationId",
                table: "EquipmentAssignment",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignmentHistory_AspNetUsers_UserId",
                table: "EquipmentAssignmentHistory",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignmentHistory_Employees_EmployeeId",
                table: "EquipmentAssignmentHistory",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignmentHistory_EquipmentAssignment_EquipmentAssignmentId",
                table: "EquipmentAssignmentHistory",
                column: "EquipmentAssignmentId",
                principalTable: "EquipmentAssignment",
                principalColumn: "EquipmentAssignmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignmentHistory_Equipments_EquipmentId",
                table: "EquipmentAssignmentHistory",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignmentHistory_Locations_LocationId",
                table: "EquipmentAssignmentHistory",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
