using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIpdFinalBillPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IPD_Final_Bill",
                columns: table => new
                {
                    Bill_No = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Admission_ID = table.Column<long>(type: "bigint", nullable: false),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Admission_Fee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Bed_Charges = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Service_Charges = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Hospital_Charges = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Ambulance_Charges = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Gross_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount_Authorized_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Net_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Receipt_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Applied_Advance_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Balance_Due = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Excess_Payment = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Settlement_Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Discharge_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Discharge_Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Generated_By = table.Column<int>(type: "int", nullable: false),
                    Generated_At = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPD_Final_Bill", x => x.Bill_No);
                    table.CheckConstraint("CK_IPD_Final_Bill_Amounts", "[Gross_Amount] >= 0 AND [Discount_Amount] >= 0 AND [Discount_Amount] <= [Gross_Amount] AND [Net_Amount] = [Gross_Amount] - [Discount_Amount] AND [Receipt_Amount] >= 0 AND [Applied_Advance_Amount] >= 0 AND [Balance_Due] >= 0 AND [Excess_Payment] >= 0");
                    table.ForeignKey(
                        name: "FK_IPD_Final_Bill_Admission_Admission_ID",
                        column: x => x.Admission_ID,
                        principalTable: "Admission",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IPD_Final_Bill_Receipt",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bill_No = table.Column<long>(type: "bigint", nullable: false),
                    Money_Receipt_ID = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Against = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPD_Final_Bill_Receipt", x => x.ID);
                    table.CheckConstraint("CK_IPD_Final_Bill_Receipt_Amount", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_IPD_Final_Bill_Receipt_IPD_Final_Bill_Bill_No",
                        column: x => x.Bill_No,
                        principalTable: "IPD_Final_Bill",
                        principalColumn: "Bill_No",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IPD_Final_Bill_Receipt_Money_Receipt_Money_Receipt_ID",
                        column: x => x.Money_Receipt_ID,
                        principalTable: "Money_Receipt",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Final_Bill_Admission_ID",
                table: "IPD_Final_Bill",
                column: "Admission_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Final_Bill_Reg_No",
                table: "IPD_Final_Bill",
                column: "Reg_No",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Final_Bill_Receipt_Bill_No",
                table: "IPD_Final_Bill_Receipt",
                column: "Bill_No");

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Final_Bill_Receipt_Money_Receipt_ID",
                table: "IPD_Final_Bill_Receipt",
                column: "Money_Receipt_ID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IPD_Final_Bill_Receipt");

            migrationBuilder.DropTable(
                name: "IPD_Final_Bill");
        }
    }
}
