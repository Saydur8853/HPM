using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class HardenDiagnosticReturnWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cancellation_Reason",
                table: "Lab_Test_Result",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Cancelled_At",
                table: "Lab_Test_Result",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cancelled_By",
                table: "Lab_Test_Result",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Lab_Test_Result",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.AddColumn<string>(
                name: "Cancellation_Reason",
                table: "Diagnostic_Cash_Memo_Return",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Cancelled_At",
                table: "Diagnostic_Cash_Memo_Return",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cancelled_By",
                table: "Diagnostic_Cash_Memo_Return",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Refund_Method",
                table: "Diagnostic_Cash_Memo_Return",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Refund_Received_By",
                table: "Diagnostic_Cash_Memo_Return",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Refund_Reference",
                table: "Diagnostic_Cash_Memo_Return",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Diagnostic_Cash_Memo_Return",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Posted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cancellation_Reason",
                table: "Lab_Test_Result");

            migrationBuilder.DropColumn(
                name: "Cancelled_At",
                table: "Lab_Test_Result");

            migrationBuilder.DropColumn(
                name: "Cancelled_By",
                table: "Lab_Test_Result");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Lab_Test_Result");

            migrationBuilder.DropColumn(
                name: "Cancellation_Reason",
                table: "Diagnostic_Cash_Memo_Return");

            migrationBuilder.DropColumn(
                name: "Cancelled_At",
                table: "Diagnostic_Cash_Memo_Return");

            migrationBuilder.DropColumn(
                name: "Cancelled_By",
                table: "Diagnostic_Cash_Memo_Return");

            migrationBuilder.DropColumn(
                name: "Refund_Method",
                table: "Diagnostic_Cash_Memo_Return");

            migrationBuilder.DropColumn(
                name: "Refund_Received_By",
                table: "Diagnostic_Cash_Memo_Return");

            migrationBuilder.DropColumn(
                name: "Refund_Reference",
                table: "Diagnostic_Cash_Memo_Return");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Diagnostic_Cash_Memo_Return");
        }
    }
}
