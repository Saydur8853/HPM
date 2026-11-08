using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImproveDoctorWorkspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Draft_Saved_At",
                table: "Patient_Encounter",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Follow_Up_Completed_At",
                table: "Patient_Encounter",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Follow_Up_Date",
                table: "Patient_Encounter",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Follow_Up_Reason",
                table: "Patient_Encounter",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Follow_Up_Required",
                table: "Patient_Encounter",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Patient_Encounter",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Normal");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Encounter_Doctor_ID_Follow_Up_Required_Follow_Up_Date",
                table: "Patient_Encounter",
                columns: new[] { "Doctor_ID", "Follow_Up_Required", "Follow_Up_Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patient_Encounter_Doctor_ID_Follow_Up_Required_Follow_Up_Date",
                table: "Patient_Encounter");

            migrationBuilder.DropColumn(
                name: "Draft_Saved_At",
                table: "Patient_Encounter");

            migrationBuilder.DropColumn(
                name: "Follow_Up_Completed_At",
                table: "Patient_Encounter");

            migrationBuilder.DropColumn(
                name: "Follow_Up_Date",
                table: "Patient_Encounter");

            migrationBuilder.DropColumn(
                name: "Follow_Up_Reason",
                table: "Patient_Encounter");

            migrationBuilder.DropColumn(
                name: "Follow_Up_Required",
                table: "Patient_Encounter");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Patient_Encounter");
        }
    }
}
