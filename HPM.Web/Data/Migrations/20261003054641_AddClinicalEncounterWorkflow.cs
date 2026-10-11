using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicalEncounterWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Created_By",
                table: "Prescription",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Encounter_ID",
                table: "Prescription",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Patient_ID",
                table: "Prescription",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Signed_At",
                table: "Prescription",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Prescription",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Signed");

            migrationBuilder.AddColumn<long>(
                name: "Ticket_ID",
                table: "Prescription",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Doctor_Appointment",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Booked");

            migrationBuilder.CreateTable(
                name: "Patient_Encounter",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Patient_ID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Patient_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Age = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Doctor_ID = table.Column<int>(type: "int", nullable: false),
                    Department_ID = table.Column<int>(type: "int", nullable: true),
                    Appointment_ID = table.Column<int>(type: "int", nullable: true),
                    Ticket_ID = table.Column<long>(type: "bigint", nullable: true),
                    Visit_Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Queue_No = table.Column<int>(type: "int", nullable: false),
                    Visit_Date = table.Column<DateTime>(type: "date", nullable: false),
                    Checked_In_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Started_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Completed_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Blood_Pressure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Pulse = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Temperature = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Oxygen_Saturation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Height_Cm = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Weight_Kg = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Allergies = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Chief_Complaint = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Clinical_Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Advice = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Created_By = table.Column<int>(type: "int", nullable: false),
                    Started_By = table.Column<int>(type: "int", nullable: true),
                    Completed_By = table.Column<int>(type: "int", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient_Encounter", x => x.ID);
                    table.CheckConstraint("CK_Patient_Encounter_Status", "[Status] IN ('Waiting','InConsultation','Completed','Cancelled','NoShow')");
                    table.ForeignKey(
                        name: "FK_Patient_Encounter_Doctor_Appointment_Appointment_ID",
                        column: x => x.Appointment_ID,
                        principalTable: "Doctor_Appointment",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patient_Encounter_Doctors_Profile_Doctor_ID",
                        column: x => x.Doctor_ID,
                        principalTable: "Doctors_Profile",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patient_Encounter_OPD_Ticket_Ticket_ID",
                        column: x => x.Ticket_ID,
                        principalTable: "OPD_Ticket",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_Encounter_ID",
                table: "Prescription",
                column: "Encounter_ID",
                unique: true,
                filter: "[Encounter_ID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_Patient_ID",
                table: "Prescription",
                column: "Patient_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Encounter_Appointment_ID",
                table: "Patient_Encounter",
                column: "Appointment_ID",
                unique: true,
                filter: "[Appointment_ID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Encounter_Doctor_ID_Visit_Date_Status",
                table: "Patient_Encounter",
                columns: new[] { "Doctor_ID", "Visit_Date", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Encounter_Patient_ID_Visit_Date",
                table: "Patient_Encounter",
                columns: new[] { "Patient_ID", "Visit_Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Encounter_Ticket_ID",
                table: "Patient_Encounter",
                column: "Ticket_ID",
                unique: true,
                filter: "[Ticket_ID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Patient_Encounter_Encounter_ID",
                table: "Prescription",
                column: "Encounter_ID",
                principalTable: "Patient_Encounter",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Patient_Encounter_Encounter_ID",
                table: "Prescription");

            migrationBuilder.DropTable(
                name: "Patient_Encounter");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_Encounter_ID",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_Patient_ID",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Created_By",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Encounter_ID",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Patient_ID",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Signed_At",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Ticket_ID",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Doctor_Appointment");
        }
    }
}
