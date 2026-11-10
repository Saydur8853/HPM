using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientPortal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patient_Portal_Consent",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient_Portal_Consent", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Patient_Portal_Query",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reply = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RepliedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RepliedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient_Portal_Query", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Portal_Consent_PatientID_Version",
                table: "Patient_Portal_Consent",
                columns: new[] { "PatientID", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Portal_Query_PatientID_SubmittedAt",
                table: "Patient_Portal_Query",
                columns: new[] { "PatientID", "SubmittedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Portal_Query_Status_SubmittedAt",
                table: "Patient_Portal_Query",
                columns: new[] { "Status", "SubmittedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Patient_Portal_Consent");

            migrationBuilder.DropTable(
                name: "Patient_Portal_Query");
        }
    }
}
