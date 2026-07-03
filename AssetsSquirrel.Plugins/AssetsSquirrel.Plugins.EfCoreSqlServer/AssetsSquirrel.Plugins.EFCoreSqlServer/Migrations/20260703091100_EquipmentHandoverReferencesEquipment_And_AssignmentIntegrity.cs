using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class EquipmentHandoverReferencesEquipment_And_AssignmentIntegrity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentHandoverDetails_HardwareTypes_HardwareTypeId",
                table: "EquipmentHandoverDetails");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentHandoverDetails_EquipmentHandoverId",
                table: "EquipmentHandoverDetails");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentAssignments_EquipmentId",
                table: "EquipmentAssignments");

            migrationBuilder.RenameColumn(
                name: "HardwareTypeId",
                table: "EquipmentHandoverDetails",
                newName: "EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentHandoverDetails_HardwareTypeId",
                table: "EquipmentHandoverDetails",
                newName: "IX_EquipmentHandoverDetails_EquipmentId");

            migrationBuilder.AlterColumn<int>(
                name: "ToEmployeeId",
                table: "EquipmentHandovers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FromLocationId",
                table: "EquipmentHandovers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "EquipmentHandovers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreparedByUserId",
                table: "EquipmentHandovers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "EquipmentHandovers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentHandoverId",
                table: "EquipmentAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHandovers_HandoverDocumentNumber",
                table: "EquipmentHandovers",
                column: "HandoverDocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHandovers_PreparedByUserId",
                table: "EquipmentHandovers",
                column: "PreparedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHandoverDetails_EquipmentHandoverId_EquipmentId",
                table: "EquipmentHandoverDetails",
                columns: new[] { "EquipmentHandoverId", "EquipmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignments_EquipmentHandoverId",
                table: "EquipmentAssignments",
                column: "EquipmentHandoverId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignments_EquipmentId",
                table: "EquipmentAssignments",
                column: "EquipmentId",
                unique: true,
                filter: "[DateOfReturn] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAssignments_EquipmentHandovers_EquipmentHandoverId",
                table: "EquipmentAssignments",
                column: "EquipmentHandoverId",
                principalTable: "EquipmentHandovers",
                principalColumn: "EquipmentHandoverId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentHandoverDetails_Equipments_EquipmentId",
                table: "EquipmentHandoverDetails",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentHandovers_AspNetUsers_PreparedByUserId",
                table: "EquipmentHandovers",
                column: "PreparedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAssignments_EquipmentHandovers_EquipmentHandoverId",
                table: "EquipmentAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentHandoverDetails_Equipments_EquipmentId",
                table: "EquipmentHandoverDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentHandovers_AspNetUsers_PreparedByUserId",
                table: "EquipmentHandovers");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentHandovers_HandoverDocumentNumber",
                table: "EquipmentHandovers");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentHandovers_PreparedByUserId",
                table: "EquipmentHandovers");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentHandoverDetails_EquipmentHandoverId_EquipmentId",
                table: "EquipmentHandoverDetails");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentAssignments_EquipmentHandoverId",
                table: "EquipmentAssignments");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentAssignments_EquipmentId",
                table: "EquipmentAssignments");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "EquipmentHandovers");

            migrationBuilder.DropColumn(
                name: "PreparedByUserId",
                table: "EquipmentHandovers");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "EquipmentHandovers");

            migrationBuilder.DropColumn(
                name: "EquipmentHandoverId",
                table: "EquipmentAssignments");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "EquipmentHandoverDetails",
                newName: "HardwareTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentHandoverDetails_EquipmentId",
                table: "EquipmentHandoverDetails",
                newName: "IX_EquipmentHandoverDetails_HardwareTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "ToEmployeeId",
                table: "EquipmentHandovers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FromLocationId",
                table: "EquipmentHandovers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHandoverDetails_EquipmentHandoverId",
                table: "EquipmentHandoverDetails",
                column: "EquipmentHandoverId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignments_EquipmentId",
                table: "EquipmentAssignments",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentHandoverDetails_HardwareTypes_HardwareTypeId",
                table: "EquipmentHandoverDetails",
                column: "HardwareTypeId",
                principalTable: "HardwareTypes",
                principalColumn: "HardwareTypeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
