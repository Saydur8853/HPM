using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalHrm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HRM_Department",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsClinical = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Department", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Designation",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Designation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Leave_Type",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnnualAllowance = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    RequiresAttachment = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Leave_Type", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Payroll_Period",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinalizedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalizedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Payroll_Period", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Shift",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    GraceMinutes = table.Column<int>(type: "int", nullable: false),
                    BreakMinutes = table.Column<int>(type: "int", nullable: false),
                    IsNightShift = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Shift", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Holiday",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HolidayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Holiday", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HRM_Holiday_HRM_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "HRM_Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Employee",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AttendanceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    DesignationID = table.Column<int>(type: "int", nullable: false),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmploymentType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BankAccount = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    TaxIdentifier = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HouseRentAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MedicalAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransportAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvertimeHourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Employee", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HRM_Employee_HRM_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "HRM_Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HRM_Employee_HRM_Designation_DesignationID",
                        column: x => x.DesignationID,
                        principalTable: "HRM_Designation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Attendance",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    ShiftID = table.Column<int>(type: "int", nullable: true),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckIn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    WorkMinutes = table.Column<int>(type: "int", nullable: false),
                    LateMinutes = table.Column<int>(type: "int", nullable: false),
                    OvertimeMinutes = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Attendance", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HRM_Attendance_HRM_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "HRM_Employee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HRM_Attendance_HRM_Shift_ShiftID",
                        column: x => x.ShiftID,
                        principalTable: "HRM_Shift",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Leave_Balance",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeID = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    AllocatedDays = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    UsedDays = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    AdjustedDays = table.Column<decimal>(type: "decimal(8,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Leave_Balance", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HRM_Leave_Balance_HRM_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "HRM_Employee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HRM_Leave_Balance_HRM_Leave_Type_LeaveTypeID",
                        column: x => x.LeaveTypeID,
                        principalTable: "HRM_Leave_Type",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Leave_Request",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeID = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDays = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    IsHalfDay = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Leave_Request", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HRM_Leave_Request_HRM_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "HRM_Employee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HRM_Leave_Request_HRM_Leave_Type_LeaveTypeID",
                        column: x => x.LeaveTypeID,
                        principalTable: "HRM_Leave_Type",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Payroll_Entry",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollPeriodID = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Allowances = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvertimeHours = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    OvertimeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AbsentDays = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    UnpaidLeaveDays = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Payroll_Entry", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HRM_Payroll_Entry_HRM_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "HRM_Employee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HRM_Payroll_Entry_HRM_Payroll_Period_PayrollPeriodID",
                        column: x => x.PayrollPeriodID,
                        principalTable: "HRM_Payroll_Period",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HRM_Roster_Assignment",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    ShiftID = table.Column<int>(type: "int", nullable: false),
                    DutyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRM_Roster_Assignment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HRM_Roster_Assignment_HRM_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "HRM_Employee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HRM_Roster_Assignment_HRM_Shift_ShiftID",
                        column: x => x.ShiftID,
                        principalTable: "HRM_Shift",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "HRM_Department",
                columns: new[] { "ID", "Code", "IsActive", "IsClinical", "Name" },
                values: new object[,]
                {
                    { 1, "ADMIN", true, false, "Administration" },
                    { 2, "MED", true, true, "Medical Services" },
                    { 3, "NURS", true, true, "Nursing" },
                    { 4, "DIAG", true, true, "Diagnostics & Laboratory" },
                    { 5, "PHARM", true, true, "Pharmacy" },
                    { 6, "ACCT", true, false, "Accounts & Finance" },
                    { 7, "SUPPORT", true, false, "Support Services" }
                });

            migrationBuilder.InsertData(
                table: "HRM_Designation",
                columns: new[] { "ID", "Code", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "MO", true, "Medical Officer" },
                    { 2, "NURSE", true, "Staff Nurse" },
                    { 3, "TECH", true, "Technician" },
                    { 4, "OFFICER", true, "Officer" },
                    { 5, "ASSIST", true, "Assistant" },
                    { 6, "WORKER", true, "Support Worker" }
                });

            migrationBuilder.InsertData(
                table: "HRM_Leave_Type",
                columns: new[] { "ID", "AnnualAllowance", "Code", "IsActive", "IsPaid", "Name", "RequiresAttachment" },
                values: new object[,]
                {
                    { 1, 10m, "CL", true, true, "Casual Leave", false },
                    { 2, 14m, "SL", true, true, "Sick Leave", true },
                    { 3, 15m, "EL", true, true, "Earned Leave", false },
                    { 4, 0m, "MAT", true, true, "Maternity Leave", true },
                    { 5, 0m, "LWP", true, false, "Leave Without Pay", false }
                });

            migrationBuilder.InsertData(
                table: "HRM_Shift",
                columns: new[] { "ID", "BreakMinutes", "Code", "EndTime", "GraceMinutes", "IsActive", "IsNightShift", "Name", "StartTime" },
                values: new object[,]
                {
                    { 1, 60, "GENERAL", new TimeSpan(0, 17, 0, 0, 0), 15, true, false, "General Duty", new TimeSpan(0, 9, 0, 0, 0) },
                    { 2, 30, "MORNING", new TimeSpan(0, 15, 0, 0, 0), 10, true, false, "Morning Shift", new TimeSpan(0, 7, 0, 0, 0) },
                    { 3, 30, "EVENING", new TimeSpan(0, 23, 0, 0, 0), 10, true, false, "Evening Shift", new TimeSpan(0, 15, 0, 0, 0) },
                    { 4, 30, "NIGHT", new TimeSpan(0, 7, 0, 0, 0), 10, true, true, "Night Shift", new TimeSpan(0, 23, 0, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Attendance_AttendanceDate_Status",
                table: "HRM_Attendance",
                columns: new[] { "AttendanceDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Attendance_EmployeeID_AttendanceDate",
                table: "HRM_Attendance",
                columns: new[] { "EmployeeID", "AttendanceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Attendance_ShiftID",
                table: "HRM_Attendance",
                column: "ShiftID");

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Department_Code",
                table: "HRM_Department",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Department_Name",
                table: "HRM_Department",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Designation_Code",
                table: "HRM_Designation",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Designation_Name",
                table: "HRM_Designation",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Employee_AttendanceNo",
                table: "HRM_Employee",
                column: "AttendanceNo",
                unique: true,
                filter: "[AttendanceNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Employee_DepartmentID_Status",
                table: "HRM_Employee",
                columns: new[] { "DepartmentID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Employee_DesignationID",
                table: "HRM_Employee",
                column: "DesignationID");

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Employee_EmployeeNo",
                table: "HRM_Employee",
                column: "EmployeeNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Holiday_DepartmentID",
                table: "HRM_Holiday",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Holiday_HolidayDate_DepartmentID",
                table: "HRM_Holiday",
                columns: new[] { "HolidayDate", "DepartmentID" });

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Leave_Balance_EmployeeID_LeaveTypeID_Year",
                table: "HRM_Leave_Balance",
                columns: new[] { "EmployeeID", "LeaveTypeID", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Leave_Balance_LeaveTypeID",
                table: "HRM_Leave_Balance",
                column: "LeaveTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Leave_Request_EmployeeID_FromDate_ToDate",
                table: "HRM_Leave_Request",
                columns: new[] { "EmployeeID", "FromDate", "ToDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Leave_Request_LeaveTypeID",
                table: "HRM_Leave_Request",
                column: "LeaveTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Leave_Request_Status_AppliedAt",
                table: "HRM_Leave_Request",
                columns: new[] { "Status", "AppliedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Leave_Type_Code",
                table: "HRM_Leave_Type",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Payroll_Entry_EmployeeID",
                table: "HRM_Payroll_Entry",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Payroll_Entry_PayrollPeriodID_EmployeeID",
                table: "HRM_Payroll_Entry",
                columns: new[] { "PayrollPeriodID", "EmployeeID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Payroll_Period_Year_Month",
                table: "HRM_Payroll_Period",
                columns: new[] { "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Roster_Assignment_DutyDate_ShiftID",
                table: "HRM_Roster_Assignment",
                columns: new[] { "DutyDate", "ShiftID" });

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Roster_Assignment_EmployeeID_DutyDate",
                table: "HRM_Roster_Assignment",
                columns: new[] { "EmployeeID", "DutyDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Roster_Assignment_ShiftID",
                table: "HRM_Roster_Assignment",
                column: "ShiftID");

            migrationBuilder.CreateIndex(
                name: "IX_HRM_Shift_Code",
                table: "HRM_Shift",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HRM_Attendance");

            migrationBuilder.DropTable(
                name: "HRM_Holiday");

            migrationBuilder.DropTable(
                name: "HRM_Leave_Balance");

            migrationBuilder.DropTable(
                name: "HRM_Leave_Request");

            migrationBuilder.DropTable(
                name: "HRM_Payroll_Entry");

            migrationBuilder.DropTable(
                name: "HRM_Roster_Assignment");

            migrationBuilder.DropTable(
                name: "HRM_Leave_Type");

            migrationBuilder.DropTable(
                name: "HRM_Payroll_Period");

            migrationBuilder.DropTable(
                name: "HRM_Employee");

            migrationBuilder.DropTable(
                name: "HRM_Shift");

            migrationBuilder.DropTable(
                name: "HRM_Department");

            migrationBuilder.DropTable(
                name: "HRM_Designation");

        }
    }
}
