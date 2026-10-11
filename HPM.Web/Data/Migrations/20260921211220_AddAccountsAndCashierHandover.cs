using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountsAndCashierHandover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts_Chart",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Account_Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Parent_ID = table.Column<int>(type: "int", nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts_Chart", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Accounts_Voucher",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Voucher_No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Voucher_Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Voucher_Date = table.Column<DateTime>(type: "date", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Is_Posted = table.Column<bool>(type: "bit", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts_Voucher", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Cashier_Handover",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Business_Date = table.Column<DateTime>(type: "date", nullable: false),
                    Cashier_ID = table.Column<int>(type: "int", nullable: false),
                    System_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Handed_Over_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Variance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cashier_Handover", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Accounts_Voucher_Line",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Voucher_ID = table.Column<long>(type: "bigint", nullable: false),
                    Account_ID = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts_Voucher_Line", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Accounts_Voucher_Line_Accounts_Chart_Account_ID",
                        column: x => x.Account_ID,
                        principalTable: "Accounts_Chart",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Voucher_Line_Accounts_Voucher_Voucher_ID",
                        column: x => x.Voucher_ID,
                        principalTable: "Accounts_Voucher",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts_Chart",
                columns: new[] { "ID", "Account_Type", "Code", "Is_Active", "Name", "Parent_ID" },
                values: new object[,]
                {
                    { 1, "Asset", "1000", true, "Cash in Hand", null },
                    { 2, "Income", "4000", true, "Hospital Service Revenue", null },
                    { 3, "Income", "4010", true, "Pharmacy Sales", null },
                    { 4, "Expense", "5000", true, "Medicine Purchase", null },
                    { 5, "Expense", "5100", true, "General Expense", null },
                    { 6, "Liability", "2000", true, "Supplier Payable", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Chart_Code",
                table: "Accounts_Chart",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Voucher_Voucher_Date",
                table: "Accounts_Voucher",
                column: "Voucher_Date");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Voucher_Voucher_No",
                table: "Accounts_Voucher",
                column: "Voucher_No",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Voucher_Line_Account_ID_Voucher_ID",
                table: "Accounts_Voucher_Line",
                columns: new[] { "Account_ID", "Voucher_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Voucher_Line_Voucher_ID",
                table: "Accounts_Voucher_Line",
                column: "Voucher_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Cashier_Handover_Business_Date_Cashier_ID",
                table: "Cashier_Handover",
                columns: new[] { "Business_Date", "Cashier_ID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts_Voucher_Line");

            migrationBuilder.DropTable(
                name: "Cashier_Handover");

            migrationBuilder.DropTable(
                name: "Accounts_Chart");

            migrationBuilder.DropTable(
                name: "Accounts_Voucher");
        }
    }
}
