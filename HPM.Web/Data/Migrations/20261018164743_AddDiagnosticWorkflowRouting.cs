using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnosticWorkflowRouting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Diagnostic_Type",
                table: "Service",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Result_Value",
                table: "Lab_Test_Result",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "Completed_At",
                table: "Lab_Sample_Collection",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Completed_By",
                table: "Lab_Sample_Collection",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Processing_Started_At",
                table: "Lab_Sample_Collection",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Processing_Started_By",
                table: "Lab_Sample_Collection",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Received_At",
                table: "Lab_Sample_Collection",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Received_By",
                table: "Lab_Sample_Collection",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Scheduled_At",
                table: "Lab_Sample_Collection",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Workstation",
                table: "Lab_Sample_Collection",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql(
                "IF COL_LENGTH('Doctor_Appointment', 'Sex') IS NULL " +
                "ALTER TABLE [Doctor_Appointment] ADD [Sex] nvarchar(50) NULL;");

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "ID",
                keyValue: 1,
                column: "Diagnostic_Type",
                value: null);

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "ID",
                keyValue: 2,
                column: "Diagnostic_Type",
                value: null);

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "ID",
                keyValue: 3,
                column: "Diagnostic_Type",
                value: null);

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "ID",
                keyValue: 4,
                column: "Diagnostic_Type",
                value: null);

            migrationBuilder.Sql(
                """
                UPDATE s
                SET [Diagnostic_Type] = CASE
                    WHEN c.[ID] IN (8, 41, 56, 66, 91, 101)
                      OR c.[Name] LIKE '%Pathology%'
                      OR c.[Name] LIKE '%Laboratory%'
                    THEN 'Pathology'
                    ELSE 'Imaging'
                END
                FROM [Service] s
                INNER JOIN [Service_Category] c ON c.[ID] = s.[Category_ID]
                WHERE s.[Diagnostic_Type] IS NULL
                  AND (c.[Sub_ID] = 8 OR c.[ID] IN (66, 91)
                       OR c.[Name] LIKE '%Investigation%'
                       OR c.[Name] LIKE '%Pathology%'
                       OR c.[Name] LIKE '%Laboratory%'
                       OR c.[Name] LIKE '%Imaging%'
                       OR c.[Name] LIKE '%Radiology%'
                       OR c.[Name] LIKE '%X-Ray%'
                       OR c.[Name] LIKE '%Ultrasound%');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diagnostic_Type",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Completed_At",
                table: "Lab_Sample_Collection");

            migrationBuilder.DropColumn(
                name: "Completed_By",
                table: "Lab_Sample_Collection");

            migrationBuilder.DropColumn(
                name: "Processing_Started_At",
                table: "Lab_Sample_Collection");

            migrationBuilder.DropColumn(
                name: "Processing_Started_By",
                table: "Lab_Sample_Collection");

            migrationBuilder.DropColumn(
                name: "Received_At",
                table: "Lab_Sample_Collection");

            migrationBuilder.DropColumn(
                name: "Received_By",
                table: "Lab_Sample_Collection");

            migrationBuilder.DropColumn(
                name: "Scheduled_At",
                table: "Lab_Sample_Collection");

            migrationBuilder.DropColumn(
                name: "Workstation",
                table: "Lab_Sample_Collection");

            migrationBuilder.AlterColumn<string>(
                name: "Result_Value",
                table: "Lab_Test_Result",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);
        }
    }
}
