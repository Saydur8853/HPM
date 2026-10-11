using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyPurchaseStockAndPos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cash_Memo",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Patient_Type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Is_Package = table.Column<bool>(type: "bit", nullable: false),
                    Is_Credit = table.Column<bool>(type: "bit", nullable: true),
                    Buffer_Store = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cash_Memo", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Medicine_Receiving_Challan",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier = table.Column<int>(type: "int", nullable: false),
                    Invoice_No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Invoice_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Is_Posted = table.Column<bool>(type: "bit", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    VAT = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Store_ID = table.Column<int>(type: "int", nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Receiving_Challan", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Supplier_Ledger",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier_ID = table.Column<int>(type: "int", nullable: false),
                    Challan_No = table.Column<long>(type: "bigint", nullable: true),
                    Supplier_Payment_ID = table.Column<long>(type: "bigint", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier_Ledger", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Cash_Memo_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bill_No = table.Column<long>(type: "bigint", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: true),
                    SLNO = table.Column<int>(type: "int", nullable: true),
                    Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Buy_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cash_Memo_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cash_Memo_Sub_Cash_Memo_Bill_No",
                        column: x => x.Bill_No,
                        principalTable: "Cash_Memo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medicine_Receiving_Challan_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Challan_No = table.Column<long>(type: "bigint", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    SLNO = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Buy_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    VAT = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Expire_Date = table.Column<DateTime>(type: "date", nullable: true),
                    Batch_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sale_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Receiving_Challan_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Medicine_Receiving_Challan_Sub_Medicine_Receiving_Challan_Challan_No",
                        column: x => x.Challan_No,
                        principalTable: "Medicine_Receiving_Challan",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cash_Memo_Trans_Date",
                table: "Cash_Memo",
                column: "Trans_Date");

            migrationBuilder.CreateIndex(
                name: "IX_Cash_Memo_Sub_Bill_No_SLNO",
                table: "Cash_Memo_Sub",
                columns: new[] { "Bill_No", "SLNO" },
                unique: true,
                filter: "[SLNO] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Receiving_Challan_Supplier_Invoice_No",
                table: "Medicine_Receiving_Challan",
                columns: new[] { "Supplier", "Invoice_No" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Receiving_Challan_Sub_Challan_No_SLNO",
                table: "Medicine_Receiving_Challan_Sub",
                columns: new[] { "Challan_No", "SLNO" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_Ledger_Supplier_ID_Trans_Date",
                table: "Supplier_Ledger",
                columns: new[] { "Supplier_ID", "Trans_Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cash_Memo_Sub");

            migrationBuilder.DropTable(
                name: "Medicine_Receiving_Challan_Sub");

            migrationBuilder.DropTable(
                name: "Supplier_Ledger");

            migrationBuilder.DropTable(
                name: "Cash_Memo");

            migrationBuilder.DropTable(
                name: "Medicine_Receiving_Challan");
        }
    }
}
