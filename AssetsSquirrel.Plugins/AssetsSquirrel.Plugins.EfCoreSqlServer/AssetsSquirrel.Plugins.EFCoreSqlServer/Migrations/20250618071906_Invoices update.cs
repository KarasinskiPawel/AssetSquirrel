using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Invoicesupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentHistories_Invoice_InvoiceId",
                table: "EquipmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Invoice_InvoiceId",
                table: "Equipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice");

            migrationBuilder.RenameTable(
                name: "Invoice",
                newName: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "Invoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentHistories_Invoices_InvoiceId",
                table: "EquipmentHistories",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Invoices_InvoiceId",
                table: "Equipments",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentHistories_Invoices_InvoiceId",
                table: "EquipmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Invoices_InvoiceId",
                table: "Equipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Invoices");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoice");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentHistories_Invoice_InvoiceId",
                table: "EquipmentHistories",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Invoice_InvoiceId",
                table: "Equipments",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
