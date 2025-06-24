using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Equipment_Add_Required_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_HardwareTypes_HardwareTypeId",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Manufacturers_ManufacturerId",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Suppilers_SuppilerId",
                table: "Equipments");

            migrationBuilder.AlterColumn<int>(
                name: "SuppilerId",
                table: "Equipments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManufacturerId",
                table: "Equipments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HardwareTypeId",
                table: "Equipments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdd",
                table: "Equipments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_HardwareTypes_HardwareTypeId",
                table: "Equipments",
                column: "HardwareTypeId",
                principalTable: "HardwareTypes",
                principalColumn: "HardwareTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Manufacturers_ManufacturerId",
                table: "Equipments",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "ManufacturerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Suppilers_SuppilerId",
                table: "Equipments",
                column: "SuppilerId",
                principalTable: "Suppilers",
                principalColumn: "SuppilerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_HardwareTypes_HardwareTypeId",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Manufacturers_ManufacturerId",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Suppilers_SuppilerId",
                table: "Equipments");

            migrationBuilder.AlterColumn<int>(
                name: "SuppilerId",
                table: "Equipments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ManufacturerId",
                table: "Equipments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HardwareTypeId",
                table: "Equipments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdd",
                table: "Equipments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_HardwareTypes_HardwareTypeId",
                table: "Equipments",
                column: "HardwareTypeId",
                principalTable: "HardwareTypes",
                principalColumn: "HardwareTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Manufacturers_ManufacturerId",
                table: "Equipments",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "ManufacturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Suppilers_SuppilerId",
                table: "Equipments",
                column: "SuppilerId",
                principalTable: "Suppilers",
                principalColumn: "SuppilerId");
        }
    }
}
