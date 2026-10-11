using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyReturnsSupplierPaymentsAndAutoAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicine_Return",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier_ID = table.Column<int>(type: "int", nullable: false),
                    Store_ID = table.Column<int>(type: "int", nullable: false),
                    Is_Posted = table.Column<bool>(type: "bit", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Return", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Supplier_Payment",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier_ID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Payment_Mode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier_Payment", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Medicine_Return_Sub",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Medicine_Return_ID = table.Column<int>(type: "int", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    SLNO = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Buy_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Return_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Medicine_Return_Sub_Medicine_Return_Medicine_Return_ID",
                        column: x => x.Medicine_Return_ID,
                        principalTable: "Medicine_Return",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts_Chart",
                columns: new[] { "ID", "Account_Type", "Code", "Is_Active", "Name", "Parent_ID" },
                values: new object[] { 7, "Asset", "1100", true, "Patient Receivable", null });

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Return_Supplier_ID_Trans_Date",
                table: "Medicine_Return",
                columns: new[] { "Supplier_ID", "Trans_Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Return_Sub_Medicine_Return_ID_SLNO",
                table: "Medicine_Return_Sub",
                columns: new[] { "Medicine_Return_ID", "SLNO" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_Payment_Supplier_ID_Trans_Date",
                table: "Supplier_Payment",
                columns: new[] { "Supplier_ID", "Trans_Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medicine_Return_Sub");

            migrationBuilder.DropTable(
                name: "Supplier_Payment");

            migrationBuilder.DropTable(
                name: "Medicine_Return");

            migrationBuilder.DeleteData(
                table: "Accounts_Chart",
                keyColumn: "ID",
                keyValue: 7);
        }
    }
}
