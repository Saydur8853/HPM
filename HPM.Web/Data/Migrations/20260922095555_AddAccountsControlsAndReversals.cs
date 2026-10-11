using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountsControlsAndReversals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Closed_At",
                table: "Cashier_Handover",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Locked",
                table: "Cashier_Handover",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Reopen_Reason",
                table: "Cashier_Handover",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Reopened_At",
                table: "Cashier_Handover",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Reopened_By",
                table: "Cashier_Handover",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Approved_At",
                table: "Accounts_Voucher",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Approved_By",
                table: "Accounts_Voucher",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cancellation_Reason",
                table: "Accounts_Voucher",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Cancelled_At",
                table: "Accounts_Voucher",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cancelled_By",
                table: "Accounts_Voucher",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Reversal_Of_Voucher_ID",
                table: "Accounts_Voucher",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Accounts_Voucher",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.Sql("""
                UPDATE [Accounts_Voucher]
                SET [Status] = CASE WHEN [Is_Posted] = 1 THEN 'Approved' ELSE 'Pending' END,
                    [Approved_By] = CASE WHEN [Is_Posted] = 1 THEN [Entered_By] ELSE NULL END,
                    [Approved_At] = CASE WHEN [Is_Posted] = 1 THEN [Trans_Date] ELSE NULL END;
                """);

            migrationBuilder.CreateTable(
                name: "Financial_Period_Lock",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    From_Date = table.Column<DateTime>(type: "date", nullable: false),
                    To_Date = table.Column<DateTime>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Locked_By = table.Column<int>(type: "int", nullable: false),
                    Locked_At = table.Column<DateTime>(type: "datetime", nullable: false),
                    Unlocked_By = table.Column<int>(type: "int", nullable: true),
                    Unlocked_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Unlock_Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Financial_Period_Lock", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Financial_Reversal",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entity_Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Entity_ID = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Reversal_Voucher_ID = table.Column<long>(type: "bigint", nullable: true),
                    Reversed_By = table.Column<int>(type: "int", nullable: false),
                    Reversed_At = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Financial_Reversal", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Voucher_Reference",
                table: "Accounts_Voucher",
                column: "Reference");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Voucher_Reversal_Of_Voucher_ID",
                table: "Accounts_Voucher",
                column: "Reversal_Of_Voucher_ID",
                unique: true,
                filter: "[Reversal_Of_Voucher_ID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Voucher_Status",
                table: "Accounts_Voucher",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Financial_Period_Lock_Is_Active_From_Date_To_Date",
                table: "Financial_Period_Lock",
                columns: new[] { "Is_Active", "From_Date", "To_Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Financial_Reversal_Entity_Type_Entity_ID",
                table: "Financial_Reversal",
                columns: new[] { "Entity_Type", "Entity_ID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Financial_Period_Lock");

            migrationBuilder.DropTable(
                name: "Financial_Reversal");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Voucher_Reference",
                table: "Accounts_Voucher");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Voucher_Reversal_Of_Voucher_ID",
                table: "Accounts_Voucher");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Voucher_Status",
                table: "Accounts_Voucher");

            migrationBuilder.DropColumn(
                name: "Closed_At",
                table: "Cashier_Handover");

            migrationBuilder.DropColumn(
                name: "Is_Locked",
                table: "Cashier_Handover");

            migrationBuilder.DropColumn(
                name: "Reopen_Reason",
                table: "Cashier_Handover");

            migrationBuilder.DropColumn(
                name: "Reopened_At",
                table: "Cashier_Handover");

            migrationBuilder.DropColumn(
                name: "Reopened_By",
                table: "Cashier_Handover");

            migrationBuilder.DropColumn(
                name: "Approved_At",
                table: "Accounts_Voucher");

            migrationBuilder.DropColumn(
                name: "Approved_By",
                table: "Accounts_Voucher");

            migrationBuilder.DropColumn(
                name: "Cancellation_Reason",
                table: "Accounts_Voucher");

            migrationBuilder.DropColumn(
                name: "Cancelled_At",
                table: "Accounts_Voucher");

            migrationBuilder.DropColumn(
                name: "Cancelled_By",
                table: "Accounts_Voucher");

            migrationBuilder.DropColumn(
                name: "Reversal_Of_Voucher_ID",
                table: "Accounts_Voucher");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Accounts_Voucher");
        }
    }
}
