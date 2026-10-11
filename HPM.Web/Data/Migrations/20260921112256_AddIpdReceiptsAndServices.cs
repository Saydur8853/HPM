using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIpdReceiptsAndServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IPD_Money_Receipt",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Receipt_No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Patient_ID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Payment_Mode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPD_Money_Receipt", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "IPD_Patient_Service_Item",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Patient_ID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Service_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Doctor_ID = table.Column<int>(type: "int", nullable: true),
                    Service_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPD_Patient_Service_Item", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Money_Receipt_Receipt_No",
                table: "IPD_Money_Receipt",
                column: "Receipt_No",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Money_Receipt_Reg_No",
                table: "IPD_Money_Receipt",
                column: "Reg_No");

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Money_Receipt_Trans_Date",
                table: "IPD_Money_Receipt",
                column: "Trans_Date");

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Patient_Service_Item_Reg_No",
                table: "IPD_Patient_Service_Item",
                column: "Reg_No");

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Patient_Service_Item_Service_Date",
                table: "IPD_Patient_Service_Item",
                column: "Service_Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IPD_Money_Receipt");

            migrationBuilder.DropTable(
                name: "IPD_Patient_Service_Item");
        }
    }
}
