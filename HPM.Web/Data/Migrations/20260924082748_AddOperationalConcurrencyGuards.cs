using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationalConcurrencyGuards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Visit_Date",
                table: "Prescription",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_Visit_Date_Visit_No",
                table: "Prescription",
                columns: new[] { "Visit_Date", "Visit_No" },
                unique: true,
                filter: "[Visit_Date] IS NOT NULL AND [Visit_No] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Bed_Cabin_Transection_Open_Bed",
                table: "Bed_Cabin_Transection",
                column: "Bed_Cabin_ID",
                unique: true,
                filter: "[End_Date] IS NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Bed_Cabin_Transection_Open_RegNo",
                table: "Bed_Cabin_Transection",
                column: "Reg_No",
                unique: true,
                filter: "[End_Date] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prescription_Visit_Date_Visit_No",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "UX_Bed_Cabin_Transection_Open_Bed",
                table: "Bed_Cabin_Transection");

            migrationBuilder.DropIndex(
                name: "UX_Bed_Cabin_Transection_Open_RegNo",
                table: "Bed_Cabin_Transection");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Visit_Date",
                table: "Prescription",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);
        }
    }
}
