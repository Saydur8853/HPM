using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityAndPharmacyBatchTrace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE [AspNetUsers] SET [MustChangePassword] = 1 WHERE [NormalizedUserName] = 'ADMIN'");

            migrationBuilder.CreateTable(
                name: "Pharmacy_Sale_Batch_Allocation",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cash_Memo_Sub_ID = table.Column<long>(type: "bigint", nullable: false),
                    Stock_Balance_ID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Buy_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Batch_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Expire_Date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy_Sale_Batch_Allocation", x => x.ID);
                    table.CheckConstraint("CK_Pharmacy_Sale_Batch_Allocation_Amounts", "[Quantity] > 0 AND [Buy_Rate] >= 0");
                    table.ForeignKey(
                        name: "FK_Pharmacy_Sale_Batch_Allocation_Cash_Memo_Sub_Cash_Memo_Sub_ID",
                        column: x => x.Cash_Memo_Sub_ID,
                        principalTable: "Cash_Memo_Sub",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Sale_Batch_Allocation_Medicine_Store_Balance_Stock_Balance_ID",
                        column: x => x.Stock_Balance_ID,
                        principalTable: "Medicine_Store_Balance",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Sale_Batch_Allocation_Cash_Memo_Sub_ID_Stock_Balance_ID",
                table: "Pharmacy_Sale_Batch_Allocation",
                columns: new[] { "Cash_Memo_Sub_ID", "Stock_Balance_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Sale_Batch_Allocation_Stock_Balance_ID_Expire_Date",
                table: "Pharmacy_Sale_Batch_Allocation",
                columns: new[] { "Stock_Balance_ID", "Expire_Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pharmacy_Sale_Batch_Allocation");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "AspNetUsers");
        }
    }
}
