using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialOpdEmergency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cash_Memo_DInvestigation",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Patient_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Patient_Type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Bed = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Age = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount_By = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Paid = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Reg_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Patient_ID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Doctor_ID = table.Column<int>(type: "int", nullable: true),
                    Module_ID = table.Column<int>(type: "int", nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cash_Memo_DInvestigation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Cash_Memo_OT",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Patient_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Patient_Type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Bed = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Age = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Patient_ID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Reg_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Doctor_ID = table.Column<int>(type: "int", nullable: true),
                    OT = table.Column<int>(type: "int", nullable: true),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount_By = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cash_Memo_OT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department_Category_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Doctor_Appointment",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Doctor_ID = table.Column<int>(type: "int", nullable: false),
                    Appt_SLNO = table.Column<int>(type: "int", nullable: false),
                    Appt_Date = table.Column<DateTime>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Age = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Contact_No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Patient_ID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor_Appointment", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Doctors_Profile",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Degree = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Attachment = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, defaultValue: "Temporary"),
                    PO_Serial = table.Column<int>(type: "int", nullable: true),
                    Area_Code = table.Column<int>(type: "int", nullable: true),
                    Balance = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    Opening_Balance = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Room = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Department = table.Column<int>(type: "int", nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: true),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Referral_Category_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors_Profile", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OPD_Ticket",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Reg_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ticket_Rate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Age = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OPD = table.Column<int>(type: "int", nullable: false),
                    Vat = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    Doctor_ID = table.Column<int>(type: "int", nullable: false),
                    Dept_ID = table.Column<int>(type: "int", nullable: false),
                    Room = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Total = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    Posted = table.Column<bool>(type: "bit", nullable: false),
                    Entered_By = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OPD_Ticket", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Registration",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Patient_ID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date_Of_Birth = table.Column<DateTime>(type: "datetime", nullable: false),
                    Sex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Blood_Group = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Father_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mother_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Care_Of = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Permanent_Village = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Permanent_PO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Permanent_PS = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Permanent_District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Present_Village = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Present_PO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Present_PS = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Present_District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Entered_By = table.Column<int>(type: "int", nullable: false),
                    Trans_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registration", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Sale_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Buy_Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Re_Order_Level = table.Column<int>(type: "int", nullable: true),
                    Is_VAT_App = table.Column<bool>(type: "bit", nullable: true),
                    Category_ID = table.Column<int>(type: "int", nullable: true),
                    Type_ID = table.Column<int>(type: "int", nullable: true),
                    Generic_ID = table.Column<int>(type: "int", nullable: true),
                    Supplier_ID = table.Column<int>(type: "int", nullable: true),
                    Investigation_Module_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Service_Category",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sub_ID = table.Column<int>(type: "int", nullable: true),
                    Module_ID = table.Column<int>(type: "int", nullable: true),
                    Service_Charge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Is_Percentage = table.Column<bool>(type: "bit", nullable: true),
                    Group_Sub_ID = table.Column<int>(type: "int", nullable: true),
                    Investigation_SLNO = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service_Category", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Ticket_Type",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket_Type", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Cash_Memo_Sub_DInvestigation",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Memo_ID = table.Column<long>(type: "bigint", nullable: false),
                    SLNo = table.Column<int>(type: "int", nullable: false),
                    Test_ID = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cash_Memo_Sub_DInvestigation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cash_Memo_Sub_DInvestigation_Cash_Memo_DInvestigation_Memo_ID",
                        column: x => x.Memo_ID,
                        principalTable: "Cash_Memo_DInvestigation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cash_Memo_Sub_OT",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Memo_ID = table.Column<long>(type: "bigint", nullable: false),
                    SLNO = table.Column<int>(type: "int", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Doctor_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cash_Memo_Sub_OT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cash_Memo_Sub_OT_Cash_Memo_OT_Memo_ID",
                        column: x => x.Memo_ID,
                        principalTable: "Cash_Memo_OT",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Department",
                columns: new[] { "ID", "Department_Category_ID", "Name" },
                values: new object[,]
                {
                    { 1, null, "Emergency" },
                    { 2, null, "Medicine" }
                });

            migrationBuilder.InsertData(
                table: "Doctors_Profile",
                columns: new[] { "ID", "Address", "Area_Code", "Attachment", "Balance", "Contact", "Degree", "Department", "Email", "Entered_By", "Name", "Opening_Balance", "PO_Serial", "Referral_Category_ID", "Room", "Title", "Trans_Date" },
                values: new object[] { 1, null, null, "Temporary", null, null, "MBBS", 1, null, 1, "Duty Doctor", null, null, null, "ER-01", "Dr.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Service",
                columns: new[] { "ID", "Buy_Rate", "Category_ID", "Generic_ID", "Investigation_Module_ID", "Is_VAT_App", "Name", "Re_Order_Level", "Sale_Rate", "Supplier_ID", "Type_ID" },
                values: new object[,]
                {
                    { 1, null, 6, null, null, null, "Emergency consultation", null, 500m, null, null },
                    { 2, null, 6, null, null, null, "Nebulization", null, 300m, null, null },
                    { 3, null, 1, null, null, null, "Surgeon charge", null, 10000m, null, null },
                    { 4, null, 1, null, null, null, "Anaesthesia charge", null, 2000m, null, null }
                });

            migrationBuilder.InsertData(
                table: "Service_Category",
                columns: new[] { "ID", "Group_Sub_ID", "Investigation_SLNO", "Is_Percentage", "Module_ID", "Name", "Service_Charge", "Sub_ID" },
                values: new object[,]
                {
                    { 1, null, null, null, null, "Team Charge/Doctor Consultation", null, 1 },
                    { 6, null, null, null, null, "OPD Emergency", null, 0 },
                    { 54, null, null, null, null, "OT Charge", null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Ticket_Type",
                columns: new[] { "ID", "Amount", "Name" },
                values: new object[] { 1, 50m, "Consultation Fee" });

            migrationBuilder.CreateIndex(
                name: "IX_Cash_Memo_Sub_DInvestigation_Memo_ID_SLNo",
                table: "Cash_Memo_Sub_DInvestigation",
                columns: new[] { "Memo_ID", "SLNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cash_Memo_Sub_OT_Memo_ID_SLNO",
                table: "Cash_Memo_Sub_OT",
                columns: new[] { "Memo_ID", "SLNO" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_Appointment_Doctor_ID_Appt_Date_Appt_SLNO",
                table: "Doctor_Appointment",
                columns: new[] { "Doctor_ID", "Appt_Date", "Appt_SLNO" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registration_Patient_ID",
                table: "Registration",
                column: "Patient_ID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cash_Memo_Sub_DInvestigation");

            migrationBuilder.DropTable(
                name: "Cash_Memo_Sub_OT");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Doctor_Appointment");

            migrationBuilder.DropTable(
                name: "Doctors_Profile");

            migrationBuilder.DropTable(
                name: "OPD_Ticket");

            migrationBuilder.DropTable(
                name: "Registration");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Service_Category");

            migrationBuilder.DropTable(
                name: "Ticket_Type");

            migrationBuilder.DropTable(
                name: "Cash_Memo_DInvestigation");

            migrationBuilder.DropTable(
                name: "Cash_Memo_OT");
        }
    }
}
