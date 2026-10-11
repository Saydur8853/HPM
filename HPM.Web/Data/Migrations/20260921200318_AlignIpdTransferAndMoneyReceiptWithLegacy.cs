using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlignIpdTransferAndMoneyReceiptWithLegacy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_IPD_Money_Receipt",
                table: "IPD_Money_Receipt");

            migrationBuilder.DropIndex(
                name: "IX_IPD_Money_Receipt_Receipt_No",
                table: "IPD_Money_Receipt");

            migrationBuilder.DropColumn(
                name: "Patient_ID",
                table: "IPD_Money_Receipt");

            migrationBuilder.DropColumn(
                name: "Payment_Mode",
                table: "IPD_Money_Receipt");

            migrationBuilder.DropColumn(
                name: "Receipt_No",
                table: "IPD_Money_Receipt");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "IPD_Money_Receipt");

            migrationBuilder.RenameTable(
                name: "IPD_Money_Receipt",
                newName: "Money_Receipt");

            migrationBuilder.RenameIndex(
                name: "IX_IPD_Money_Receipt_Trans_Date",
                table: "Money_Receipt",
                newName: "IX_Money_Receipt_Trans_Date");

            migrationBuilder.RenameIndex(
                name: "IX_IPD_Money_Receipt_Reg_No",
                table: "Money_Receipt",
                newName: "IX_Money_Receipt_Reg_No");

            migrationBuilder.AlterColumn<int>(
                name: "Entered_By",
                table: "Money_Receipt",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Against",
                table: "Money_Receipt",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Advance");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Money_Receipt",
                table: "Money_Receipt",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "Central_Cash_Collection",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Module_Flag = table.Column<int>(type: "int", nullable: false),
                    Head_ID = table.Column<int>(type: "int", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Trans_Date_CL = table.Column<DateTime>(type: "datetime", nullable: false),
                    Reference_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Central_Cash_Collection", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Patient_Package_Transaction",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Service_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IPD_Dept_ID = table.Column<int>(type: "int", nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient_Package_Transaction", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Patient_Payment",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MR_ID = table.Column<long>(type: "bigint", nullable: false),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    IS_Hospital_Payment = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient_Payment", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Patient_Transaction",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Service_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IPD_Dept_ID = table.Column<int>(type: "int", nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient_Transaction", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Central_Cash_Collection_Reference_ID",
                table: "Central_Cash_Collection",
                column: "Reference_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Package_Transaction_Reg_No",
                table: "Patient_Package_Transaction",
                column: "Reg_No");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Payment_MR_ID",
                table: "Patient_Payment",
                column: "MR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Payment_Reg_No",
                table: "Patient_Payment",
                column: "Reg_No");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Transaction_Reg_No",
                table: "Patient_Transaction",
                column: "Reg_No");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Central_Cash_Collection");

            migrationBuilder.DropTable(
                name: "Patient_Package_Transaction");

            migrationBuilder.DropTable(
                name: "Patient_Payment");

            migrationBuilder.DropTable(
                name: "Patient_Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Money_Receipt",
                table: "Money_Receipt");

            migrationBuilder.DropColumn(
                name: "Against",
                table: "Money_Receipt");

            migrationBuilder.RenameTable(
                name: "Money_Receipt",
                newName: "IPD_Money_Receipt");

            migrationBuilder.RenameIndex(
                name: "IX_Money_Receipt_Trans_Date",
                table: "IPD_Money_Receipt",
                newName: "IX_IPD_Money_Receipt_Trans_Date");

            migrationBuilder.RenameIndex(
                name: "IX_Money_Receipt_Reg_No",
                table: "IPD_Money_Receipt",
                newName: "IX_IPD_Money_Receipt_Reg_No");

            migrationBuilder.AlterColumn<int>(
                name: "Entered_By",
                table: "IPD_Money_Receipt",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Patient_ID",
                table: "IPD_Money_Receipt",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payment_Mode",
                table: "IPD_Money_Receipt",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Receipt_No",
                table: "IPD_Money_Receipt",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "IPD_Money_Receipt",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IPD_Money_Receipt",
                table: "IPD_Money_Receipt",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Money_Receipt_Receipt_No",
                table: "IPD_Money_Receipt",
                column: "Receipt_No",
                unique: true);
        }
    }
}
