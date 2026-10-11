using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyMastersAndStockFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Generic_Group",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: true),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generic_Group", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Medicine_Store",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Is_Buffer_Store = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Store", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Medicine_Type",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: true),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Type", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Supplier_Pharmacy",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier_Pharmacy", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Medicine_Store_Balance",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Medicine_Store_ID = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Expire_Date = table.Column<DateTime>(type: "date", nullable: true),
                    Medicine_Receiving_Challan_Sub_ID = table.Column<long>(type: "bigint", nullable: true),
                    Batch_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Store_Balance", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Medicine_Store_Balance_Medicine_Store_Medicine_Store_ID",
                        column: x => x.Medicine_Store_ID,
                        principalTable: "Medicine_Store",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Medicine_Store_Balance_Service_Service_ID",
                        column: x => x.Service_ID,
                        principalTable: "Service",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Generic_Group",
                columns: new[] { "ID", "Entered_By", "Name", "Trans_Date" },
                values: new object[] { 1, null, "Others", null });

            migrationBuilder.InsertData(
                table: "Medicine_Store",
                columns: new[] { "ID", "Is_Active", "Is_Buffer_Store", "Name" },
                values: new object[] { 1, true, false, "Main Pharmacy" });

            migrationBuilder.InsertData(
                table: "Medicine_Type",
                columns: new[] { "ID", "Entered_By", "Trans_Date", "Type" },
                values: new object[] { 1, null, null, "CAPSULE" });

            migrationBuilder.InsertData(
                table: "Service_Category",
                columns: new[] { "ID", "Group_Sub_ID", "Investigation_SLNO", "Is_Percentage", "Module_ID", "Name", "Service_Charge", "Sub_ID" },
                values: new object[] { 3, null, null, null, null, "Medicine", null, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Generic_Group_Name",
                table: "Generic_Group",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Store_Name",
                table: "Medicine_Store",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Store_Balance_Medicine_Store_ID_Service_ID_Batch_No_Expire_Date",
                table: "Medicine_Store_Balance",
                columns: new[] { "Medicine_Store_ID", "Service_ID", "Batch_No", "Expire_Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Store_Balance_Service_ID",
                table: "Medicine_Store_Balance",
                column: "Service_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Type_Type",
                table: "Medicine_Type",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_Pharmacy_Name",
                table: "Supplier_Pharmacy",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Generic_Group");

            migrationBuilder.DropTable(
                name: "Medicine_Store_Balance");

            migrationBuilder.DropTable(
                name: "Medicine_Type");

            migrationBuilder.DropTable(
                name: "Supplier_Pharmacy");

            migrationBuilder.DropTable(
                name: "Medicine_Store");

            migrationBuilder.DeleteData(
                table: "Service_Category",
                keyColumn: "ID",
                keyValue: 3);
        }
    }
}
