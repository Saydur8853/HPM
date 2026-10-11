using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompletePharmacySalesReturnPrescriptionStockControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER DATABASE CURRENT SET AUTO_CLOSE OFF;", suppressTransaction: true);

            migrationBuilder.CreateTable(
                name: "Medicine_Stock_Adjustment",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Store_ID = table.Column<int>(type: "int", nullable: false),
                    Adjustment_Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Is_Posted = table.Column<bool>(type: "bit", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Stock_Adjustment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Medicine_Stock_Adjustment_Medicine_Store_Store_ID",
                        column: x => x.Store_ID,
                        principalTable: "Medicine_Store",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Medicine_Stock_Opening",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Store_ID = table.Column<int>(type: "int", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Opening_Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Captured_At = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Stock_Opening", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Medicine_Stock_Opening_Medicine_Store_Store_ID",
                        column: x => x.Store_ID,
                        principalTable: "Medicine_Store",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Medicine_Stock_Opening_Service_Service_ID",
                        column: x => x.Service_ID,
                        principalTable: "Service",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_Sales_Return",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cash_Memo_ID = table.Column<long>(type: "bigint", nullable: false),
                    Store_ID = table.Column<int>(type: "int", nullable: false),
                    Refund_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Is_Posted = table.Column<bool>(type: "bit", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy_Sales_Return", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Sales_Return_Cash_Memo_Cash_Memo_ID",
                        column: x => x.Cash_Memo_ID,
                        principalTable: "Cash_Memo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Sales_Return_Medicine_Store_Store_ID",
                        column: x => x.Store_ID,
                        principalTable: "Medicine_Store",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prescription",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Age = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Blood_Group = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Visit_No = table.Column<int>(type: "int", nullable: true),
                    Visit_Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BMI = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Prem = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Pulse = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Restp = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Temperature = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Waist = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Hip = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Complain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Medical_History = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Findings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Investigation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Medicine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Referred_By = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescription", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Prescription_Doctors_Profile_Referred_By",
                        column: x => x.Referred_By,
                        principalTable: "Doctors_Profile",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Medicine_Stock_Adjustment_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adjustment_ID = table.Column<long>(type: "bigint", nullable: false),
                    Stock_Balance_ID = table.Column<int>(type: "int", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Batch_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Expire_Date = table.Column<DateTime>(type: "date", nullable: true),
                    Before_Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Change_Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    After_Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Unit_Cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Stock_Adjustment_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Medicine_Stock_Adjustment_Sub_Medicine_Stock_Adjustment_Adjustment_ID",
                        column: x => x.Adjustment_ID,
                        principalTable: "Medicine_Stock_Adjustment",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Medicine_Stock_Adjustment_Sub_Medicine_Store_Balance_Stock_Balance_ID",
                        column: x => x.Stock_Balance_ID,
                        principalTable: "Medicine_Store_Balance",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Medicine_Stock_Adjustment_Sub_Service_Service_ID",
                        column: x => x.Service_ID,
                        principalTable: "Service",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_Sales_Return_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sales_Return_ID = table.Column<long>(type: "bigint", nullable: false),
                    Cash_Memo_Sub_ID = table.Column<long>(type: "bigint", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Stock_Balance_ID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Buy_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Is_Restocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy_Sales_Return_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Sales_Return_Sub_Cash_Memo_Sub_Cash_Memo_Sub_ID",
                        column: x => x.Cash_Memo_Sub_ID,
                        principalTable: "Cash_Memo_Sub",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Sales_Return_Sub_Medicine_Store_Balance_Stock_Balance_ID",
                        column: x => x.Stock_Balance_ID,
                        principalTable: "Medicine_Store_Balance",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Sales_Return_Sub_Pharmacy_Sales_Return_Sales_Return_ID",
                        column: x => x.Sales_Return_ID,
                        principalTable: "Pharmacy_Sales_Return",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_Prescription_Dispense",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prescription_ID = table.Column<long>(type: "bigint", nullable: false),
                    Cash_Memo_ID = table.Column<long>(type: "bigint", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy_Prescription_Dispense", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Prescription_Dispense_Cash_Memo_Cash_Memo_ID",
                        column: x => x.Cash_Memo_ID,
                        principalTable: "Cash_Memo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Prescription_Dispense_Prescription_Prescription_ID",
                        column: x => x.Prescription_ID,
                        principalTable: "Prescription",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prescription_Medicine",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prescription_ID = table.Column<long>(type: "bigint", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Interval = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Dose = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Days = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SLNO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescription_Medicine", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Prescription_Medicine_Prescription_Prescription_ID",
                        column: x => x.Prescription_ID,
                        principalTable: "Prescription",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prescription_Medicine_Service_Service_ID",
                        column: x => x.Service_ID,
                        principalTable: "Service",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_Prescription_Dispense_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dispense_ID = table.Column<long>(type: "bigint", nullable: false),
                    Prescription_Medicine_ID = table.Column<int>(type: "int", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy_Prescription_Dispense_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Prescription_Dispense_Sub_Pharmacy_Prescription_Dispense_Dispense_ID",
                        column: x => x.Dispense_ID,
                        principalTable: "Pharmacy_Prescription_Dispense",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Stock_Adjustment_Store_ID_Trans_Date",
                table: "Medicine_Stock_Adjustment",
                columns: new[] { "Store_ID", "Trans_Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Stock_Adjustment_Sub_Adjustment_ID",
                table: "Medicine_Stock_Adjustment_Sub",
                column: "Adjustment_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Stock_Adjustment_Sub_Service_ID",
                table: "Medicine_Stock_Adjustment_Sub",
                column: "Service_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Stock_Adjustment_Sub_Stock_Balance_ID",
                table: "Medicine_Stock_Adjustment_Sub",
                column: "Stock_Balance_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Stock_Opening_Service_ID",
                table: "Medicine_Stock_Opening",
                column: "Service_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Stock_Opening_Store_ID_Service_ID",
                table: "Medicine_Stock_Opening",
                columns: new[] { "Store_ID", "Service_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Prescription_Dispense_Cash_Memo_ID",
                table: "Pharmacy_Prescription_Dispense",
                column: "Cash_Memo_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Prescription_Dispense_Prescription_ID",
                table: "Pharmacy_Prescription_Dispense",
                column: "Prescription_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Prescription_Dispense_Sub_Dispense_ID",
                table: "Pharmacy_Prescription_Dispense_Sub",
                column: "Dispense_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Prescription_Dispense_Sub_Prescription_Medicine_ID",
                table: "Pharmacy_Prescription_Dispense_Sub",
                column: "Prescription_Medicine_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Sales_Return_Cash_Memo_ID_Trans_Date",
                table: "Pharmacy_Sales_Return",
                columns: new[] { "Cash_Memo_ID", "Trans_Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Sales_Return_Store_ID",
                table: "Pharmacy_Sales_Return",
                column: "Store_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Sales_Return_Sub_Cash_Memo_Sub_ID",
                table: "Pharmacy_Sales_Return_Sub",
                column: "Cash_Memo_Sub_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Sales_Return_Sub_Sales_Return_ID",
                table: "Pharmacy_Sales_Return_Sub",
                column: "Sales_Return_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Sales_Return_Sub_Stock_Balance_ID",
                table: "Pharmacy_Sales_Return_Sub",
                column: "Stock_Balance_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_Referred_By",
                table: "Prescription",
                column: "Referred_By");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_Visit_Date",
                table: "Prescription",
                column: "Visit_Date");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_Medicine_Prescription_ID_SLNO",
                table: "Prescription_Medicine",
                columns: new[] { "Prescription_ID", "SLNO" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_Medicine_Service_ID",
                table: "Prescription_Medicine",
                column: "Service_ID");

            // Establish an auditable starting point without rewriting legacy stock rows.
            // All reconciliation movements are measured after this capture timestamp.
            migrationBuilder.Sql("""
                DECLARE @CapturedAt datetime = GETDATE();
                INSERT INTO dbo.Medicine_Stock_Opening
                    (Store_ID, Service_ID, Opening_Balance, Captured_At)
                SELECT Medicine_Store_ID, Service_ID, SUM(Balance), @CapturedAt
                FROM dbo.Medicine_Store_Balance
                GROUP BY Medicine_Store_ID, Service_ID;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medicine_Stock_Adjustment_Sub");

            migrationBuilder.DropTable(
                name: "Medicine_Stock_Opening");

            migrationBuilder.DropTable(
                name: "Pharmacy_Prescription_Dispense_Sub");

            migrationBuilder.DropTable(
                name: "Pharmacy_Sales_Return_Sub");

            migrationBuilder.DropTable(
                name: "Prescription_Medicine");

            migrationBuilder.DropTable(
                name: "Medicine_Stock_Adjustment");

            migrationBuilder.DropTable(
                name: "Pharmacy_Prescription_Dispense");

            migrationBuilder.DropTable(
                name: "Pharmacy_Sales_Return");

            migrationBuilder.DropTable(
                name: "Prescription");
        }
    }
}
