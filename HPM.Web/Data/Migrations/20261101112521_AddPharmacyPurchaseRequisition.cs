using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyPurchaseRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Purchase_Requisition",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Store_ID = table.Column<int>(type: "int", nullable: false),
                    Requisition_No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    Requested_By = table.Column<int>(type: "int", nullable: false),
                    Requested_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reviewed_By = table.Column<int>(type: "int", nullable: true),
                    Reviewed_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Review_Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchase_Requisition", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Purchase_Requisition_Medicine_Store_Store_ID",
                        column: x => x.Store_ID,
                        principalTable: "Medicine_Store",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Purchase_Requisition_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Requisition_ID = table.Column<long>(type: "bigint", nullable: false),
                    SLNO = table.Column<int>(type: "int", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Requested_Qty = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Estimated_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchase_Requisition_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Purchase_Requisition_Sub_Purchase_Requisition_Requisition_ID",
                        column: x => x.Requisition_ID,
                        principalTable: "Purchase_Requisition",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchase_Requisition_Sub_Service_Service_ID",
                        column: x => x.Service_ID,
                        principalTable: "Service",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_Requisition_Requisition_No",
                table: "Purchase_Requisition",
                column: "Requisition_No",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_Requisition_Store_ID_Trans_Date",
                table: "Purchase_Requisition",
                columns: new[] { "Store_ID", "Trans_Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_Requisition_Sub_Requisition_ID_SLNO",
                table: "Purchase_Requisition_Sub",
                columns: new[] { "Requisition_ID", "SLNO" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_Requisition_Sub_Service_ID",
                table: "Purchase_Requisition_Sub",
                column: "Service_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchase_Requisition_Sub");

            migrationBuilder.DropTable(
                name: "Purchase_Requisition");
        }
    }
}
