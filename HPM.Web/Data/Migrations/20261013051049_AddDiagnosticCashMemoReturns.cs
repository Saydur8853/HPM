using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnosticCashMemoReturns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diagnostic_Cash_Memo_Return",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Memo_ID = table.Column<long>(type: "bigint", nullable: false),
                    Adjustment_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Refund_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Receivable_Adjustment = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnostic_Cash_Memo_Return", x => x.ID);
                    table.CheckConstraint("CK_Diagnostic_Cash_Memo_Return_Amounts", "[Adjustment_Amount] > 0 AND [Refund_Amount] >= 0 AND [Receivable_Adjustment] >= 0 AND [Refund_Amount] + [Receivable_Adjustment] = [Adjustment_Amount]");
                    table.ForeignKey(
                        name: "FK_Diagnostic_Cash_Memo_Return_Cash_Memo_DInvestigation_Memo_ID",
                        column: x => x.Memo_ID,
                        principalTable: "Cash_Memo_DInvestigation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Diagnostic_Cash_Memo_Return_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Return_ID = table.Column<long>(type: "bigint", nullable: false),
                    Cash_Memo_Sub_ID = table.Column<long>(type: "bigint", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnostic_Cash_Memo_Return_Sub", x => x.ID);
                    table.CheckConstraint("CK_Diagnostic_Cash_Memo_Return_Sub_Amounts", "[Quantity] > 0 AND [Rate] >= 0");
                    table.ForeignKey(
                        name: "FK_Diagnostic_Cash_Memo_Return_Sub_Cash_Memo_Sub_DInvestigation_Cash_Memo_Sub_ID",
                        column: x => x.Cash_Memo_Sub_ID,
                        principalTable: "Cash_Memo_Sub_DInvestigation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Diagnostic_Cash_Memo_Return_Sub_Diagnostic_Cash_Memo_Return_Return_ID",
                        column: x => x.Return_ID,
                        principalTable: "Diagnostic_Cash_Memo_Return",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostic_Cash_Memo_Return_Memo_ID_Trans_Date",
                table: "Diagnostic_Cash_Memo_Return",
                columns: new[] { "Memo_ID", "Trans_Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostic_Cash_Memo_Return_Sub_Cash_Memo_Sub_ID",
                table: "Diagnostic_Cash_Memo_Return_Sub",
                column: "Cash_Memo_Sub_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostic_Cash_Memo_Return_Sub_Return_ID_Cash_Memo_Sub_ID",
                table: "Diagnostic_Cash_Memo_Return_Sub",
                columns: new[] { "Return_ID", "Cash_Memo_Sub_ID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnostic_Cash_Memo_Return_Sub");

            migrationBuilder.DropTable(
                name: "Diagnostic_Cash_Memo_Return");
        }
    }
}
