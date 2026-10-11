using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEncounterDiagnosticOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Encounter_ID",
                table: "Cash_Memo_DInvestigation",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Diagnostic_Order",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Encounter_ID = table.Column<long>(type: "bigint", nullable: false),
                    Patient_ID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Patient_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Doctor_ID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Clinical_Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Billed_Memo_ID = table.Column<long>(type: "bigint", nullable: true),
                    Ordered_By = table.Column<int>(type: "int", nullable: false),
                    Ordered_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnostic_Order", x => x.ID);
                    table.CheckConstraint("CK_Diagnostic_Order_Status", "[Status] IN ('Pending','Billed','Cancelled')");
                    table.ForeignKey(
                        name: "FK_Diagnostic_Order_Cash_Memo_DInvestigation_Billed_Memo_ID",
                        column: x => x.Billed_Memo_ID,
                        principalTable: "Cash_Memo_DInvestigation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Diagnostic_Order_Doctors_Profile_Doctor_ID",
                        column: x => x.Doctor_ID,
                        principalTable: "Doctors_Profile",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Diagnostic_Order_Patient_Encounter_Encounter_ID",
                        column: x => x.Encounter_ID,
                        principalTable: "Patient_Encounter",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Diagnostic_Order_Item",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_ID = table.Column<long>(type: "bigint", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnostic_Order_Item", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Diagnostic_Order_Item_Diagnostic_Order_Order_ID",
                        column: x => x.Order_ID,
                        principalTable: "Diagnostic_Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Diagnostic_Order_Item_Service_Service_ID",
                        column: x => x.Service_ID,
                        principalTable: "Service",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cash_Memo_DInvestigation_Encounter_ID",
                table: "Cash_Memo_DInvestigation",
                column: "Encounter_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostic_Order_Billed_Memo_ID",
                table: "Diagnostic_Order",
                column: "Billed_Memo_ID",
                unique: true,
                filter: "[Billed_Memo_ID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostic_Order_Doctor_ID",
                table: "Diagnostic_Order",
                column: "Doctor_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostic_Order_Encounter_ID",
                table: "Diagnostic_Order",
                column: "Encounter_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostic_Order_Status_Ordered_At",
                table: "Diagnostic_Order",
                columns: new[] { "Status", "Ordered_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostic_Order_Item_Order_ID_Service_ID",
                table: "Diagnostic_Order_Item",
                columns: new[] { "Order_ID", "Service_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostic_Order_Item_Service_ID",
                table: "Diagnostic_Order_Item",
                column: "Service_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cash_Memo_DInvestigation_Patient_Encounter_Encounter_ID",
                table: "Cash_Memo_DInvestigation",
                column: "Encounter_ID",
                principalTable: "Patient_Encounter",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cash_Memo_DInvestigation_Patient_Encounter_Encounter_ID",
                table: "Cash_Memo_DInvestigation");

            migrationBuilder.DropTable(
                name: "Diagnostic_Order_Item");

            migrationBuilder.DropTable(
                name: "Diagnostic_Order");

            migrationBuilder.DropIndex(
                name: "IX_Cash_Memo_DInvestigation_Encounter_ID",
                table: "Cash_Memo_DInvestigation");

            migrationBuilder.DropColumn(
                name: "Encounter_ID",
                table: "Cash_Memo_DInvestigation");
        }
    }
}
