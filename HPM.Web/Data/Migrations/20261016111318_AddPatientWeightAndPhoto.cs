using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientWeightAndPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Registration]') AND name = 'Photo_Data')
                BEGIN
                    ALTER TABLE [Registration] ADD [Photo_Data] nvarchar(max) NULL;
                END
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Registration]') AND name = 'Weight')
                BEGIN
                    ALTER TABLE [Registration] ADD [Weight] nvarchar(20) NULL;
                END
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctors_Profile]') AND name = 'Is_Active')
                BEGIN
                    ALTER TABLE [Doctors_Profile] ADD [Is_Active] bit NOT NULL DEFAULT 1;
                END
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctors_Profile]') AND name = 'Priority')
                BEGIN
                    ALTER TABLE [Doctors_Profile] ADD [Priority] int NULL;
                END
            ");

            migrationBuilder.CreateTable(
                name: "Canteen_Category",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canteen_Category", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Canteen_Challan",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier = table.Column<int>(type: "int", nullable: false),
                    Invoice_No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Invoice_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Is_Posted = table.Column<bool>(type: "bit", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    VAT = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canteen_Challan", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Canteen_Debtor",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canteen_Debtor", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Canteen_Sales",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Customer_Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Service_Charge = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Paid = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Token_No = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    VAT = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canteen_Sales", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Lab_Consultent",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Others = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Institution = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Module = table.Column<int>(type: "int", nullable: true),
                    Consultent2 = table.Column<int>(type: "int", nullable: true),
                    Signature = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Title1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Title2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lab_Consultent", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Pathology_Group_Head",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pathology_Group_Head", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Canteen_Products",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category_ID = table.Column<int>(type: "int", nullable: false),
                    Sale_Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Buy_Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Reorder_Level = table.Column<int>(type: "int", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsKitchen_Item = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canteen_Products", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Canteen_Products_Canteen_Category_Category_ID",
                        column: x => x.Category_ID,
                        principalTable: "Canteen_Category",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Canteen_Debtor_Ledger",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Debtor_ID = table.Column<int>(type: "int", nullable: false),
                    Bill_No = table.Column<long>(type: "bigint", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canteen_Debtor_Ledger", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Canteen_Debtor_Ledger_Canteen_Debtor_Debtor_ID",
                        column: x => x.Debtor_ID,
                        principalTable: "Canteen_Debtor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Canteen_Challan_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Challan_No = table.Column<long>(type: "bigint", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    SLNO = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Buy_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    VAT = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canteen_Challan_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Canteen_Challan_Sub_Canteen_Challan_Challan_No",
                        column: x => x.Challan_No,
                        principalTable: "Canteen_Challan",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Canteen_Challan_Sub_Canteen_Products_Service_ID",
                        column: x => x.Service_ID,
                        principalTable: "Canteen_Products",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Canteen_Sales_Sub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bill_No = table.Column<long>(type: "bigint", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: true),
                    SLNO = table.Column<int>(type: "int", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Buy_Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canteen_Sales_Sub", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Canteen_Sales_Sub_Canteen_Products_Service_ID",
                        column: x => x.Service_ID,
                        principalTable: "Canteen_Products",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Canteen_Sales_Sub_Canteen_Sales_Bill_No",
                        column: x => x.Bill_No,
                        principalTable: "Canteen_Sales",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Doctors_Profile",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Is_Active", "Priority" },
                values: new object[] { true, null });

            migrationBuilder.CreateIndex(
                name: "IX_Canteen_Challan_Sub_Challan_No",
                table: "Canteen_Challan_Sub",
                column: "Challan_No");

            migrationBuilder.CreateIndex(
                name: "IX_Canteen_Challan_Sub_Service_ID",
                table: "Canteen_Challan_Sub",
                column: "Service_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Canteen_Debtor_Ledger_Debtor_ID",
                table: "Canteen_Debtor_Ledger",
                column: "Debtor_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Canteen_Products_Category_ID",
                table: "Canteen_Products",
                column: "Category_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Canteen_Sales_Sub_Bill_No",
                table: "Canteen_Sales_Sub",
                column: "Bill_No");

            migrationBuilder.CreateIndex(
                name: "IX_Canteen_Sales_Sub_Service_ID",
                table: "Canteen_Sales_Sub",
                column: "Service_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Canteen_Challan_Sub");

            migrationBuilder.DropTable(
                name: "Canteen_Debtor_Ledger");

            migrationBuilder.DropTable(
                name: "Canteen_Sales_Sub");

            migrationBuilder.DropTable(
                name: "Lab_Consultent");

            migrationBuilder.DropTable(
                name: "Pathology_Group_Head");

            migrationBuilder.DropTable(
                name: "Canteen_Challan");

            migrationBuilder.DropTable(
                name: "Canteen_Debtor");

            migrationBuilder.DropTable(
                name: "Canteen_Products");

            migrationBuilder.DropTable(
                name: "Canteen_Sales");

            migrationBuilder.DropTable(
                name: "Canteen_Category");

            migrationBuilder.DropColumn(
                name: "Photo_Data",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "Is_Active",
                table: "Doctors_Profile");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Doctors_Profile");
        }
    }
}
