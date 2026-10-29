using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseRequisition : Migration
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
                name: "Store_Issue",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Issue_Department = table.Column<int>(type: "int", nullable: false),
                    IndentNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store_Issue", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Store_Product_Category",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store_Product_Category", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Supplier_Store",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier_Store", x => x.ID);
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

            migrationBuilder.CreateTable(
                name: "Store_Challan",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier = table.Column<int>(type: "int", nullable: false),
                    Invoice_No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Invoice_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Is_Posted = table.Column<bool>(type: "bit", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store_Challan", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Store_Challan_Supplier_Store_Supplier",
                        column: x => x.Supplier,
                        principalTable: "Supplier_Store",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Store_Product",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Supplier = table.Column<int>(type: "int", nullable: false),
                    Re_Order_Level = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Buy_Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Sale_Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store_Product", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Store_Product_Store_Product_Category_Category",
                        column: x => x.Category,
                        principalTable: "Store_Product_Category",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Store_Product_Supplier_Store_Supplier",
                        column: x => x.Supplier,
                        principalTable: "Supplier_Store",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Store_Challan_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Challan_No = table.Column<long>(type: "bigint", nullable: false),
                    Product_ID = table.Column<int>(type: "int", nullable: false),
                    SLNO = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Buy_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store_Challan_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Store_Challan_Sub_Store_Challan_Challan_No",
                        column: x => x.Challan_No,
                        principalTable: "Store_Challan",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Store_Challan_Sub_Store_Product_Product_ID",
                        column: x => x.Product_ID,
                        principalTable: "Store_Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Store_Issue_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Issue_No = table.Column<long>(type: "bigint", nullable: false),
                    Product_ID = table.Column<int>(type: "int", nullable: true),
                    SLNO = table.Column<int>(type: "int", nullable: true),
                    Buy_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store_Issue_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Store_Issue_Sub_Store_Issue_Issue_No",
                        column: x => x.Issue_No,
                        principalTable: "Store_Issue",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Store_Issue_Sub_Store_Product_Product_ID",
                        column: x => x.Product_ID,
                        principalTable: "Store_Product",
                        principalColumn: "ID");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Store_Challan_Supplier",
                table: "Store_Challan",
                column: "Supplier");

            migrationBuilder.CreateIndex(
                name: "IX_Store_Challan_Sub_Challan_No",
                table: "Store_Challan_Sub",
                column: "Challan_No");

            migrationBuilder.CreateIndex(
                name: "IX_Store_Challan_Sub_Product_ID",
                table: "Store_Challan_Sub",
                column: "Product_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Store_Issue_Sub_Issue_No",
                table: "Store_Issue_Sub",
                column: "Issue_No");

            migrationBuilder.CreateIndex(
                name: "IX_Store_Issue_Sub_Product_ID",
                table: "Store_Issue_Sub",
                column: "Product_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Store_Product_Category",
                table: "Store_Product",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Store_Product_Supplier",
                table: "Store_Product",
                column: "Supplier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchase_Requisition_Sub");

            migrationBuilder.DropTable(
                name: "Store_Challan_Sub");

            migrationBuilder.DropTable(
                name: "Store_Issue_Sub");

            migrationBuilder.DropTable(
                name: "Purchase_Requisition");

            migrationBuilder.DropTable(
                name: "Store_Challan");

            migrationBuilder.DropTable(
                name: "Store_Issue");

            migrationBuilder.DropTable(
                name: "Store_Product");

            migrationBuilder.DropTable(
                name: "Store_Product_Category");

            migrationBuilder.DropTable(
                name: "Supplier_Store");
        }
    }
}
