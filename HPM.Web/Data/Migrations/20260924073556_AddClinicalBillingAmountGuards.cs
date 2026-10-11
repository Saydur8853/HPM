using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicalBillingAmountGuards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Cash_Memo_Sub_OT_Amounts",
                table: "Cash_Memo_Sub_OT",
                sql: "[Rate] >= 0 AND [Quantity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Cash_Memo_Sub_DInvestigation_Amounts",
                table: "Cash_Memo_Sub_DInvestigation",
                sql: "[Rate] >= 0 AND [Quantity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Cash_Memo_OT_Amounts",
                table: "Cash_Memo_OT",
                sql: "[Total] >= 0 AND [Discount] >= 0 AND [Discount] <= [Total]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Cash_Memo_DInvestigation_Amounts",
                table: "Cash_Memo_DInvestigation",
                sql: "[Total] >= 0 AND [Discount] >= 0 AND [Discount] <= [Total] AND [Paid] >= 0 AND [Paid] <= [Total] - [Discount]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Cash_Memo_Sub_OT_Amounts",
                table: "Cash_Memo_Sub_OT");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Cash_Memo_Sub_DInvestigation_Amounts",
                table: "Cash_Memo_Sub_DInvestigation");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Cash_Memo_OT_Amounts",
                table: "Cash_Memo_OT");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Cash_Memo_DInvestigation_Amounts",
                table: "Cash_Memo_DInvestigation");
        }
    }
}
