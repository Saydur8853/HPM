using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIpdAdmissionAndBeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admission",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Patient_ID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Department = table.Column<int>(type: "int", nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Age = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Referred_By = table.Column<int>(type: "int", nullable: true),
                    Consultant = table.Column<int>(type: "int", nullable: true),
                    Admitted_By = table.Column<int>(type: "int", nullable: true),
                    Is_Referred = table.Column<bool>(type: "bit", nullable: true),
                    Bed_Cabin_ID = table.Column<int>(type: "int", nullable: true),
                    Admission_Fee = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Admission_Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Release_Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Is_Admitted = table.Column<bool>(type: "bit", nullable: true),
                    Bed_Admission_Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Bill_No = table.Column<long>(type: "bigint", nullable: true),
                    Bill_Generated_By = table.Column<int>(type: "int", nullable: true),
                    MPO_ID = table.Column<int>(type: "int", nullable: true),
                    Is_Package = table.Column<bool>(type: "bit", nullable: true),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: true),
                    Ambulance = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Outcome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Patient_Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Patient_Received_By = table.Column<int>(type: "int", nullable: true),
                    Patient_Received_Date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admission", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Admission_Ext",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Service_Charge = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Food = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admission_Ext", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "IPD_Department",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPD_Department", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Bed_Category",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Specification = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IPD_Dept_ID = table.Column<int>(type: "int", nullable: true),
                    Admission_Fee = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Service_Charge = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Food = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bed_Category", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bed_Category_IPD_Department_IPD_Dept_ID",
                        column: x => x.IPD_Dept_ID,
                        principalTable: "IPD_Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bed_Cabin",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category_ID = table.Column<int>(type: "int", nullable: false),
                    Is_Allocated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bed_Cabin", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bed_Cabin_Bed_Category_Category_ID",
                        column: x => x.Category_ID,
                        principalTable: "Bed_Category",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bed_Cabin_Transection",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bed_Cabin_ID = table.Column<int>(type: "int", nullable: false),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Patient_ID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Patient_Transaction_ID = table.Column<long>(type: "bigint", nullable: true),
                    Start_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    End_Date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bed_Cabin_Transection", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bed_Cabin_Transection_Bed_Cabin_Bed_Cabin_ID",
                        column: x => x.Bed_Cabin_ID,
                        principalTable: "Bed_Cabin",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "IPD_Department",
                columns: new[] { "ID", "Name" },
                values: new object[] { 1, "General IPD" });

            migrationBuilder.InsertData(
                table: "Bed_Category",
                columns: new[] { "ID", "Admission_Fee", "Food", "IPD_Dept_ID", "Name", "Rate", "Service_Charge", "Specification" },
                values: new object[] { 1, 500m, 0m, 1, "General Bed", 1000m, 0m, null });

            migrationBuilder.InsertData(
                table: "Bed_Cabin",
                columns: new[] { "ID", "Category_ID", "Is_Allocated", "Name" },
                values: new object[] { 1, 1, false, "G-01" });

            migrationBuilder.CreateIndex(
                name: "IX_Admission_Is_Admitted_Admission_Date",
                table: "Admission",
                columns: new[] { "Is_Admitted", "Admission_Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Admission_Patient_ID",
                table: "Admission",
                column: "Patient_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Admission_Reg_No",
                table: "Admission",
                column: "Reg_No",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admission_Ext_Reg_No",
                table: "Admission_Ext",
                column: "Reg_No",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bed_Cabin_Category_ID",
                table: "Bed_Cabin",
                column: "Category_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Bed_Cabin_Name",
                table: "Bed_Cabin",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bed_Cabin_Transection_Bed_Cabin_ID_End_Date",
                table: "Bed_Cabin_Transection",
                columns: new[] { "Bed_Cabin_ID", "End_Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Bed_Cabin_Transection_Reg_No",
                table: "Bed_Cabin_Transection",
                column: "Reg_No");

            migrationBuilder.CreateIndex(
                name: "IX_Bed_Category_IPD_Dept_ID",
                table: "Bed_Category",
                column: "IPD_Dept_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admission");

            migrationBuilder.DropTable(
                name: "Admission_Ext");

            migrationBuilder.DropTable(
                name: "Bed_Cabin_Transection");

            migrationBuilder.DropTable(
                name: "Bed_Cabin");

            migrationBuilder.DropTable(
                name: "Bed_Category");

            migrationBuilder.DropTable(
                name: "IPD_Department");
        }
    }
}
