using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

public static class HrmDemoDataSeeder
{
    private const string DemoEmployeePrefix = "DEMO-";

    public static async Task SeedAsync(HpmDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.HrmEmployees.AnyAsync(x => x.EmployeeNo.StartsWith(DemoEmployeePrefix), cancellationToken))
            return;

        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var departments = await db.HrmDepartments.ToDictionaryAsync(x => x.Code, cancellationToken);
        var designations = await db.HrmDesignations.ToDictionaryAsync(x => x.Code, cancellationToken);
        var shifts = await db.HrmShifts.ToDictionaryAsync(x => x.Code, cancellationToken);
        var leaveTypes = await db.HrmLeaveTypes.ToDictionaryAsync(x => x.Code, cancellationToken);
        var today = DateTime.Today;
        var now = DateTime.UtcNow;

        var employees = new[]
        {
            Employee("1001", "Dr. Nusrat Jahan", "Female", "01711001001", "MED", "MO", new DateTime(2021, 2, 15), 65000, 26000, 5000, 4000, 2500, 650),
            Employee("1002", "Shahriar Kabir", "Male", "01711001002", "ADMIN", "OFFICER", new DateTime(2020, 7, 1), 42000, 16800, 4000, 3000, 1500, 350),
            Employee("1003", "Farzana Akter", "Female", "01711001003", "NURS", "NURSE", new DateTime(2022, 1, 10), 32000, 12800, 3500, 2500, 1200, 280),
            Employee("1004", "Md. Rakib Hasan", "Male", "01711001004", "DIAG", "TECH", new DateTime(2023, 4, 5), 30000, 12000, 3500, 2500, 1000, 260),
            Employee("1005", "Tasnim Sultana", "Female", "01711001005", "PHARM", "OFFICER", new DateTime(2022, 9, 18), 36000, 14400, 4000, 3000, 1200, 300),
            Employee("1006", "Mahmudul Alam", "Male", "01711001006", "ACCT", "OFFICER", new DateTime(2019, 11, 3), 48000, 19200, 4500, 3500, 1800, 400),
            Employee("1007", "Rina Begum", "Female", "01711001007", "SUPPORT", "ASSIST", new DateTime(2024, 3, 12), 22000, 8800, 2500, 1800, 700, 190),
            Employee("1008", "Sohel Rana", "Male", "01711001008", "SUPPORT", "WORKER", new DateTime(2023, 8, 20), 19000, 7600, 2200, 1600, 500, 170)
        };

        db.HrmEmployees.AddRange(employees);
        await db.SaveChangesAsync(cancellationToken);

        db.HrmHolidays.AddRange(
            new HrmHoliday { Name = "Hospital Foundation Day", HolidayDate = today.AddDays(12), Scope = "Hospital", IsPaid = true, Notes = "Demo calendar entry" },
            new HrmHoliday { Name = "Nursing Training Day", HolidayDate = today.AddDays(24), Scope = "Department", DepartmentID = departments["NURS"].ID, IsPaid = true, Notes = "Demo department holiday" },
            new HrmHoliday { Name = "Staff Welfare Day", HolidayDate = today.AddDays(-18), Scope = "Hospital", IsPaid = true, Notes = "Demo calendar entry" });

        db.HrmLeaveRequests.AddRange(
            Leave(employees[2], "CL", today.AddDays(2), today.AddDays(3), 2, "Family programme", "Pending", now.AddHours(-3)),
            Leave(employees[3], "SL", today.AddDays(1), today.AddDays(1), 1, "Medical check-up", "Pending", now.AddHours(-8)),
            Leave(employees[6], "EL", today.AddDays(7), today.AddDays(9), 3, "Visit to home district", "Pending", now.AddDays(-1)),
            Leave(employees[4], "CL", today, today, 1, "Urgent family work", "Approved", now.AddDays(-2), "HR Manager"),
            Leave(employees[0], "EL", today.AddDays(-8), today.AddDays(-6), 3, "Personal leave", "Approved", now.AddDays(-12), "HR Manager"),
            Leave(employees[7], "LWP", today.AddDays(10), today.AddDays(11), 2, "Personal work", "Rejected", now.AddDays(-3), "HR Manager"));

        foreach (var employee in employees)
        foreach (var leaveType in leaveTypes.Values)
        {
            db.HrmLeaveBalances.Add(new HrmLeaveBalance
            {
                EmployeeID = employee.ID,
                LeaveTypeID = leaveType.ID,
                Year = today.Year,
                AllocatedDays = leaveType.AnnualAllowance,
                UsedDays = leaveType.Code switch { "CL" => employee.ID % 3, "SL" => employee.ID % 2, _ => 0 }
            });
        }

        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        for (var date = monthStart; date <= monthEnd; date = date.AddDays(1))
        {
            if (date.DayOfWeek == DayOfWeek.Friday) continue;
            for (var index = 0; index < employees.Length; index++)
            {
                var shift = ShiftFor(index, shifts);
                db.HrmRosterAssignments.Add(new HrmRosterAssignment
                {
                    EmployeeID = employees[index].ID,
                    ShiftID = shift.ID,
                    DutyDate = date,
                    Notes = index < 2 ? "Regular duty" : null,
                    CreatedAt = now
                });
            }
        }

        var attendanceFrom = today.AddDays(-12);
        for (var date = attendanceFrom; date <= today; date = date.AddDays(1))
        {
            if (date.DayOfWeek == DayOfWeek.Friday) continue;
            for (var index = 0; index < employees.Length; index++)
            {
                var shift = ShiftFor(index, shifts);
                var status = AttendanceStatus(date, today, index);
                var lateMinutes = status == "Late" ? 22 : 0;
                var overtimeMinutes = status == "Present" && (date.Day + index) % 4 == 0 ? 60 : 0;
                var hasTimes = status is "Present" or "Late";
                var checkIn = hasTimes ? date.Date + shift.StartTime + TimeSpan.FromMinutes(lateMinutes) : (DateTime?)null;
                var shiftHours = shift.IsNightShift ? 8 : (shift.EndTime - shift.StartTime).TotalHours;
                if (shiftHours <= 0) shiftHours += 24;
                var workMinutes = hasTimes ? (int)(shiftHours * 60) - shift.BreakMinutes - lateMinutes + overtimeMinutes : 0;
                db.HrmAttendanceRecords.Add(new HrmAttendanceRecord
                {
                    EmployeeID = employees[index].ID,
                    ShiftID = shift.ID,
                    AttendanceDate = date,
                    CheckIn = checkIn,
                    CheckOut = hasTimes ? checkIn!.Value.AddMinutes(workMinutes + shift.BreakMinutes) : null,
                    Status = status,
                    WorkMinutes = workMinutes,
                    LateMinutes = lateMinutes,
                    OvertimeMinutes = overtimeMinutes,
                    Source = index % 3 == 0 ? "CSV Import" : "Device",
                    Remarks = status == "Absent" ? "Unplanned absence" : null,
                    UpdatedAt = now
                });
            }
        }

        var period = await db.HrmPayrollPeriods.SingleOrDefaultAsync(x => x.Year == today.Year && x.Month == today.Month, cancellationToken);
        if (period is null)
        {
            period = new HrmPayrollPeriod
            {
                Name = monthStart.ToString("MMMM yyyy"), Year = today.Year, Month = today.Month,
                FromDate = monthStart, ToDate = monthEnd, Status = "Draft", CreatedAt = now
            };
            db.HrmPayrollPeriods.Add(period);
            await db.SaveChangesAsync(cancellationToken);
        }

        var existingPayrollEmployeeIds = (await db.HrmPayrollEntries
            .Where(x => x.PayrollPeriodID == period.ID)
            .Select(x => x.EmployeeID)
            .ToListAsync(cancellationToken)).ToHashSet();

        foreach (var employee in employees.Where(x => !existingPayrollEmployeeIds.Contains(x.ID)))
        {
            var allowances = employee.HouseRentAllowance + employee.MedicalAllowance + employee.TransportAllowance + employee.OtherAllowance;
            var overtimeHours = employee.ID % 4 + 2;
            var overtimeAmount = overtimeHours * employee.OvertimeHourlyRate;
            var absentDays = employee.ID % 5 == 0 ? 1 : 0;
            var deduction = Math.Round(employee.BasicSalary / 26 * absentDays, 2);
            var gross = employee.BasicSalary + allowances + overtimeAmount;
            db.HrmPayrollEntries.Add(new HrmPayrollEntry
            {
                PayrollPeriodID = period.ID, EmployeeID = employee.ID, BasicSalary = employee.BasicSalary,
                Allowances = allowances, OvertimeHours = overtimeHours, OvertimeAmount = overtimeAmount,
                GrossPay = gross, AbsentDays = absentDays, DeductionAmount = deduction,
                NetPay = gross - deduction, PaymentStatus = employee.ID % 3 == 0 ? "Paid" : "Unpaid", GeneratedAt = now
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        HrmEmployee Employee(string number, string name, string gender, string phone, string department, string designation,
            DateTime joiningDate, decimal basic, decimal houseRent, decimal medical, decimal transport, decimal other, decimal overtimeRate) => new()
        {
            EmployeeNo = $"{DemoEmployeePrefix}{number}", AttendanceNo = $"D{number}", Name = name, Gender = gender,
            Phone = phone, Email = $"{name.Replace("Dr. ", string.Empty).Replace("Md. ", string.Empty).Replace(" ", ".").ToLowerInvariant()}@primehpm.demo",
            Address = "Dhaka, Bangladesh", EmergencyContact = $"Family · {phone}", DepartmentID = departments[department].ID,
            DesignationID = designations[designation].ID, JoiningDate = joiningDate, EmploymentType = "Permanent", Status = "Active",
            BankAccount = $"DEMO-{number}-BANK", BasicSalary = basic, HouseRentAllowance = houseRent,
            MedicalAllowance = medical, TransportAllowance = transport, OtherAllowance = other,
            OvertimeHourlyRate = overtimeRate, CreatedAt = now, UpdatedAt = now
        };

        HrmLeaveRequest Leave(HrmEmployee employee, string type, DateTime from, DateTime to, decimal days, string reason,
            string status, DateTime appliedAt, string? reviewedBy = null) => new()
        {
            EmployeeID = employee.ID, LeaveTypeID = leaveTypes[type].ID, FromDate = from, ToDate = to,
            TotalDays = days, Reason = reason, Status = status, AppliedAt = appliedAt,
            ReviewedAt = reviewedBy is null ? null : appliedAt.AddHours(5), ReviewedBy = reviewedBy,
            ReviewNote = reviewedBy is null ? null : status == "Approved" ? "Approved as per policy" : "Duty coverage unavailable"
        };
        });
    }

    private static HrmShift ShiftFor(int employeeIndex, IReadOnlyDictionary<string, HrmShift> shifts) =>
        employeeIndex switch { 0 or 1 or 5 => shifts["GENERAL"], 2 or 4 or 6 => shifts["MORNING"], 3 => shifts["EVENING"], _ => shifts["NIGHT"] };

    private static string AttendanceStatus(DateTime date, DateTime today, int employeeIndex)
    {
        if (date.Date == today.Date)
            return employeeIndex switch { 3 => "Late", 4 => "On Leave", 7 => "Absent", _ => "Present" };
        if ((date.Day + employeeIndex) % 17 == 0) return "Absent";
        if ((date.Day + employeeIndex) % 7 == 0) return "Late";
        return "Present";
    }
}
