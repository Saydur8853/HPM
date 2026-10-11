using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeparateMedicalAndFinancialDischarge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Medical_Discharge_Order_ID",
                table: "IPD_Final_Bill",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IPD_Medical_Discharge_Order",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Admission_ID = table.Column<long>(type: "bigint", nullable: false),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Doctor_ID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Final_Diagnosis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Condition_At_Discharge = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Advice = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Follow_Up = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Ordered_By = table.Column<int>(type: "int", nullable: false),
                    Ordered_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Completed_By = table.Column<int>(type: "int", nullable: true),
                    Completed_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPD_Medical_Discharge_Order", x => x.ID);
                    table.CheckConstraint("CK_IPD_Medical_Discharge_Order_Status", "[Status] IN ('Ready','Completed','Cancelled')");
                    table.ForeignKey(
                        name: "FK_IPD_Medical_Discharge_Order_Admission_Admission_ID",
                        column: x => x.Admission_ID,
                        principalTable: "Admission",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPD_Medical_Discharge_Order_Doctors_Profile_Doctor_ID",
                        column: x => x.Doctor_ID,
                        principalTable: "Doctors_Profile",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Final_Bill_Medical_Discharge_Order_ID",
                table: "IPD_Final_Bill",
                column: "Medical_Discharge_Order_ID",
                unique: true,
                filter: "[Medical_Discharge_Order_ID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Medical_Discharge_Order_Admission_ID",
                table: "IPD_Medical_Discharge_Order",
                column: "Admission_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Medical_Discharge_Order_Doctor_ID",
                table: "IPD_Medical_Discharge_Order",
                column: "Doctor_ID");

            migrationBuilder.CreateIndex(
                name: "IX_IPD_Medical_Discharge_Order_Status_Ordered_At",
                table: "IPD_Medical_Discharge_Order",
                columns: new[] { "Status", "Ordered_At" });

            migrationBuilder.AddForeignKey(
                name: "FK_IPD_Final_Bill_IPD_Medical_Discharge_Order_Medical_Discharge_Order_ID",
                table: "IPD_Final_Bill",
                column: "Medical_Discharge_Order_ID",
                principalTable: "IPD_Medical_Discharge_Order",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPD_Final_Bill_IPD_Medical_Discharge_Order_Medical_Discharge_Order_ID",
                table: "IPD_Final_Bill");

            migrationBuilder.DropTable(
                name: "IPD_Medical_Discharge_Order");

            migrationBuilder.DropIndex(
                name: "IX_IPD_Final_Bill_Medical_Discharge_Order_ID",
                table: "IPD_Final_Bill");

            migrationBuilder.DropColumn(
                name: "Medical_Discharge_Order_ID",
                table: "IPD_Final_Bill");
        }
    }
}
