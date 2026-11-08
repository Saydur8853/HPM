using System.Globalization;
using System.Text;
using HPM.Web.Data;
using HPM.Web.Data.Entities;
using HPM.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace HPM.Web.Components.Pages;

public partial class Hrm : IDisposable
{
    [Inject] private IDbContextFactory<HpmDbContext> DbFactory { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "view")] public string? View { get; set; }

    private static readonly HashSet<string> ValidViews = ["overview", "employees", "holidays", "leave", "shifts", "roster", "attendance", "payroll", "reports"];
    private static readonly string[] EmploymentTypes = ["Permanent", "Contract", "Probation", "Part-time", "Intern"];
    private static readonly string[] AttendanceStatuses = ["Present", "Absent", "Late", "On Leave", "Off Day", "Holiday"];
    private static readonly NavigationItem[] NavigationItems =
    [
        new("overview", "Overview", "⌂"), new("employees", "Employees", "♙"), new("holidays", "Holidays", "◇"),
        new("leave", "Leave", "↗"), new("shifts", "Shifts", "◷"), new("roster", "Roster", "▦"),
        new("attendance", "Attendance", "✓"), new("payroll", "Payroll", "৳"), new("reports", "Reports", "▥")
    ];

    private string ActiveView => View is not null && ValidViews.Contains(View.ToLowerInvariant()) ? View.ToLowerInvariant() : "overview";
    private bool loading = true;
    private bool saving;
    private string message = string.Empty;
    private string messageClass = "success";

    private List<HrmDepartment> departments = [];
    private List<HrmDesignation> designations = [];
    private List<HrmShift> shifts = [];
    private List<HrmEmployee> employees = [];
    private List<HrmHoliday> holidays = [];
    private List<HrmLeaveType> leaveTypes = [];
    private List<HrmLeaveRequest> leaveRequests = [];
    private List<HrmRosterAssignment> rosterAssignments = [];
    private List<HrmAttendanceRecord> monthAttendance = [];
    private List<HrmAttendanceRecord> todayAttendance = [];
    private List<HrmPayrollPeriod> payrollPeriods = [];
    private List<HrmPayrollEntry> payrollEntries = [];
    private List<PayrollHistoryRow> payrollHistory = [];

    private HrmEmployee employeeEditor = NewEmployee();
    private HrmHoliday holidayEditor = NewHoliday();
    private HrmLeaveRequest leaveEditor = NewLeave();
    private HrmShift shiftEditor = NewShift();
    private RosterEditor rosterEditor = new();
    private AttendanceEditor attendanceEditor = new();
    private List<AttendanceImportRow> importRows = [];

    private string employeeSearch = string.Empty;
    private int employeeDepartmentFilter;
    private string leaveStatusFilter = "All";
    private DateTime rosterMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private DateTime attendanceMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private int payrollMonth = DateTime.Today.Month;
    private int payrollYear = DateTime.Today.Year;
    private HrmPayrollPeriod? selectedPayrollPeriod;

    protected override async Task OnInitializedAsync()
    {
        I18n.OnLanguageChanged += HandleLanguageChanged;
        await LoadAllAsync();
    }

    private void HandleLanguageChanged() => InvokeAsync(StateHasChanged);

    private async Task LoadAllAsync()
    {
        loading = true;
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            departments = await db.HrmDepartments.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
            designations = await db.HrmDesignations.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
            shifts = await db.HrmShifts.AsNoTracking().OrderBy(x => x.StartTime).ToListAsync();
            employees = await db.HrmEmployees.AsNoTracking().Include(x => x.Department).Include(x => x.Designation).OrderBy(x => x.Name).ToListAsync();
            holidays = await db.HrmHolidays.AsNoTracking().Include(x => x.Department).Where(x => x.HolidayDate >= DateTime.Today.AddYears(-1) && x.HolidayDate < DateTime.Today.AddYears(2)).ToListAsync();
            leaveTypes = await db.HrmLeaveTypes.AsNoTracking().OrderBy(x => x.ID).ToListAsync();
            leaveRequests = await db.HrmLeaveRequests.AsNoTracking().Include(x => x.Employee).ThenInclude(x => x!.Department).Include(x => x.LeaveType).OrderByDescending(x => x.AppliedAt).Take(500).ToListAsync();

            var rosterFrom = DateTime.Today.AddMonths(-2).Date;
            var rosterTo = DateTime.Today.AddMonths(3).Date;
            rosterAssignments = await RosterQuery(db).Where(x => x.DutyDate >= rosterFrom && x.DutyDate < rosterTo).ToListAsync();

            todayAttendance = await AttendanceQuery(db).Where(x => x.AttendanceDate == DateTime.Today).ToListAsync();
            await LoadAttendanceMonthAsync(db);
            await LoadPayrollAsync(db);
            ResetEditorsFromMasters();
        }
        catch (Exception ex)
        {
            Fail(ex.GetBaseException().Message);
        }
        finally
        {
            loading = false;
        }
    }

    private void ResetEditorsFromMasters()
    {
        if (employeeEditor.ID == 0)
        {
            employeeEditor.DepartmentID = departments.FirstOrDefault(x => x.IsActive)?.ID ?? 0;
            employeeEditor.DesignationID = designations.FirstOrDefault(x => x.IsActive)?.ID ?? 0;
        }
        if (leaveEditor.EmployeeID == 0) leaveEditor.EmployeeID = ActiveEmployees.FirstOrDefault()?.ID ?? 0;
        if (leaveEditor.LeaveTypeID == 0) leaveEditor.LeaveTypeID = leaveTypes.FirstOrDefault(x => x.IsActive)?.ID ?? 0;
        if (rosterEditor.EmployeeID == 0) rosterEditor.EmployeeID = ActiveEmployees.FirstOrDefault()?.ID ?? 0;
        if (rosterEditor.ShiftID == 0) rosterEditor.ShiftID = shifts.FirstOrDefault(x => x.IsActive)?.ID ?? 0;
        if (attendanceEditor.EmployeeID == 0) attendanceEditor.EmployeeID = ActiveEmployees.FirstOrDefault()?.ID ?? 0;
    }

    private static IQueryable<HrmRosterAssignment> RosterQuery(HpmDbContext db) => db.HrmRosterAssignments.AsNoTracking()
        .Include(x => x.Employee).ThenInclude(x => x!.Department).Include(x => x.Shift).OrderBy(x => x.DutyDate).ThenBy(x => x.Employee!.Name);

    private static IQueryable<HrmAttendanceRecord> AttendanceQuery(HpmDbContext db) => db.HrmAttendanceRecords.AsNoTracking()
        .Include(x => x.Employee).ThenInclude(x => x!.Department).Include(x => x.Shift).OrderByDescending(x => x.AttendanceDate).ThenBy(x => x.Employee!.Name);

    private IEnumerable<HrmEmployee> ActiveEmployees => employees.Where(x => x.Status == "Active").OrderBy(x => x.Name);
    private int ActiveEmployeeCount => employees.Count(x => x.Status == "Active");
    private int PendingLeaveCount => leaveRequests.Count(x => x.Status == "Pending");
    private int TodayPresentCount => todayAttendance.Count(x => x.Status is "Present" or "Late");
    private int TodayLeaveCount => leaveRequests.Count(x => x.Status == "Approved" && x.FromDate.Date <= DateTime.Today && x.ToDate.Date >= DateTime.Today);
    private decimal TodayAttendanceRate => ActiveEmployeeCount == 0 ? 0 : todayAttendance.Count * 100m / ActiveEmployeeCount;
    private decimal LatestPayrollTotal => payrollHistory.FirstOrDefault()?.Net ?? 0;
    private string LatestPayrollLabel => payrollHistory.FirstOrDefault()?.Period.Name ?? "No payroll generated";
    private IEnumerable<DateTime> NextSevenDays => Enumerable.Range(0, 7).Select(x => DateTime.Today.AddDays(x));

    private IEnumerable<HrmEmployee> FilteredEmployees => employees.Where(x =>
        (employeeDepartmentFilter == 0 || x.DepartmentID == employeeDepartmentFilter) &&
        (string.IsNullOrWhiteSpace(employeeSearch) || x.Name.Contains(employeeSearch, StringComparison.OrdinalIgnoreCase) || x.EmployeeNo.Contains(employeeSearch, StringComparison.OrdinalIgnoreCase) || (x.Phone?.Contains(employeeSearch, StringComparison.OrdinalIgnoreCase) ?? false)));

    private IEnumerable<HrmLeaveRequest> FilteredLeaveRequests => leaveRequests.Where(x => leaveStatusFilter == "All" || x.Status == leaveStatusFilter).Take(200);
    private List<HrmRosterAssignment> MonthRoster => rosterAssignments.Where(x => x.DutyDate.Year == rosterMonth.Year && x.DutyDate.Month == rosterMonth.Month).OrderBy(x => x.DutyDate).ThenBy(x => x.Employee?.Name).ToList();
    private List<HrmAttendanceRecord> MonthAttendance => monthAttendance;

    private IEnumerable<StatusSummaryRow> TodayStatusSummary
    {
        get
        {
            var denominator = Math.Max(ActiveEmployeeCount, 1);
            foreach (var status in new[] { "Present", "Late", "Absent", "On Leave" })
            {
                var count = todayAttendance.Count(x => x.Status == status);
                yield return new(status, count, count * 100m / denominator);
            }
        }
    }

    private IEnumerable<DepartmentSummaryRow> DepartmentSummary => departments.Where(x => x.IsActive).Select(x =>
    {
        var count = employees.Count(e => e.Status == "Active" && e.DepartmentID == x.ID);
        return new DepartmentSummaryRow(x.Name, count, ActiveEmployeeCount == 0 ? 0 : count * 100m / ActiveEmployeeCount);
    }).OrderByDescending(x => x.Count);

    private decimal LeaveApprovedPercent => leaveRequests.Count == 0 ? 0 : leaveRequests.Count(x => x.Status == "Approved") * 100m / leaveRequests.Count;

    private static HrmEmployee NewEmployee() => new() { JoiningDate = DateTime.Today, EmploymentType = "Permanent", Status = "Active" };
    private static HrmHoliday NewHoliday() => new() { HolidayDate = DateTime.Today, Scope = "Hospital", IsPaid = true };
    private static HrmLeaveRequest NewLeave() => new() { FromDate = DateTime.Today, ToDate = DateTime.Today, Status = "Pending" };
    private static HrmShift NewShift() => new() { StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), GraceMinutes = 15, BreakMinutes = 60, IsActive = true };

    private void ResetEmployee() { employeeEditor = NewEmployee(); ResetEditorsFromMasters(); ClearMessage(); }
    private void ResetHoliday() { holidayEditor = NewHoliday(); ClearMessage(); }
    private void ResetLeave() { leaveEditor = NewLeave(); ResetEditorsFromMasters(); ClearMessage(); }
    private void ResetShift() { shiftEditor = NewShift(); ClearMessage(); }
    private void ResetAttendance() { attendanceEditor = new AttendanceEditor { EmployeeID = ActiveEmployees.FirstOrDefault()?.ID ?? 0 }; ClearMessage(); }

    private void EditEmployee(HrmEmployee x) => employeeEditor = new HrmEmployee
    {
        ID = x.ID, EmployeeNo = x.EmployeeNo, AttendanceNo = x.AttendanceNo, Name = x.Name, Gender = x.Gender, DateOfBirth = x.DateOfBirth,
        Phone = x.Phone, Email = x.Email, Address = x.Address, EmergencyContact = x.EmergencyContact, DepartmentID = x.DepartmentID,
        DesignationID = x.DesignationID, JoiningDate = x.JoiningDate, EmploymentType = x.EmploymentType, Status = x.Status, BankAccount = x.BankAccount,
        TaxIdentifier = x.TaxIdentifier, BasicSalary = x.BasicSalary, HouseRentAllowance = x.HouseRentAllowance, MedicalAllowance = x.MedicalAllowance,
        TransportAllowance = x.TransportAllowance, OtherAllowance = x.OtherAllowance, OvertimeHourlyRate = x.OvertimeHourlyRate, CreatedAt = x.CreatedAt, UpdatedAt = x.UpdatedAt
    };

    private async Task SaveEmployeeAsync()
    {
        if (employeeEditor.DepartmentID == 0 || employeeEditor.DesignationID == 0) { Fail("Department and designation are required."); return; }
        saving = true;
        try
        {
            employeeEditor.EmployeeNo = employeeEditor.EmployeeNo.Trim();
            employeeEditor.Name = employeeEditor.Name.Trim();
            employeeEditor.AttendanceNo = NullIfEmpty(employeeEditor.AttendanceNo);
            employeeEditor.UpdatedAt = DateTime.UtcNow;
            await using var db = await DbFactory.CreateDbContextAsync();
            if (employeeEditor.ID == 0)
            {
                employeeEditor.CreatedAt = DateTime.UtcNow;
                db.HrmEmployees.Add(employeeEditor);
            }
            else
            {
                var existing = await db.HrmEmployees.FindAsync(employeeEditor.ID) ?? throw new InvalidOperationException("Employee record was not found.");
                db.Entry(existing).CurrentValues.SetValues(employeeEditor);
            }
            await db.SaveChangesAsync();
            Success($"Employee {employeeEditor.Name} saved successfully.");
            ResetEmployee();
            employees = await db.HrmEmployees.AsNoTracking().Include(x => x.Department).Include(x => x.Designation).OrderBy(x => x.Name).ToListAsync();
        }
        catch (Exception ex) { Fail(FriendlyDatabaseError(ex)); }
        finally { saving = false; }
    }

    private void EditHoliday(HrmHoliday x) => holidayEditor = new HrmHoliday { ID = x.ID, Name = x.Name, HolidayDate = x.HolidayDate, Scope = x.Scope, DepartmentID = x.DepartmentID, IsPaid = x.IsPaid, Notes = x.Notes };

    private async Task SaveHolidayAsync()
    {
        if (holidayEditor.Scope == "Department" && holidayEditor.DepartmentID is null) { Fail("Select a department for a department holiday."); return; }
        if (holidayEditor.Scope == "Hospital") holidayEditor.DepartmentID = null;
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            if (holidayEditor.ID == 0) db.HrmHolidays.Add(holidayEditor);
            else
            {
                var existing = await db.HrmHolidays.FindAsync(holidayEditor.ID) ?? throw new InvalidOperationException("Holiday was not found.");
                db.Entry(existing).CurrentValues.SetValues(holidayEditor);
            }
            await db.SaveChangesAsync();
            Success("Holiday calendar updated."); ResetHoliday();
            holidays = await db.HrmHolidays.AsNoTracking().Include(x => x.Department).OrderBy(x => x.HolidayDate).ToListAsync();
        }
        catch (Exception ex) { Fail(FriendlyDatabaseError(ex)); }
    }

    private async Task SaveLeaveAsync()
    {
        if (leaveEditor.EmployeeID == 0 || leaveEditor.LeaveTypeID == 0) { Fail("Employee and leave type are required."); return; }
        if (leaveEditor.ToDate.Date < leaveEditor.FromDate.Date) { Fail("Leave end date cannot be before start date."); return; }
        var days = CalculateLeaveDays(leaveEditor.FromDate, leaveEditor.ToDate, leaveEditor.IsHalfDay);
        if (days <= 0) { Fail("The selected range has no working leave day."); return; }
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            var overlap = await db.HrmLeaveRequests.AnyAsync(x => x.EmployeeID == leaveEditor.EmployeeID && x.Status != "Rejected" && x.FromDate <= leaveEditor.ToDate.Date && x.ToDate >= leaveEditor.FromDate.Date);
            if (overlap) { Fail("This employee already has an overlapping leave request."); return; }
            leaveEditor.FromDate = leaveEditor.FromDate.Date;
            leaveEditor.ToDate = leaveEditor.ToDate.Date;
            leaveEditor.TotalDays = days;
            leaveEditor.Status = "Pending";
            leaveEditor.AppliedAt = DateTime.UtcNow;
            db.HrmLeaveRequests.Add(leaveEditor);
            await db.SaveChangesAsync();
            Success("Leave request submitted for approval."); ResetLeave();
            await ReloadLeaveAsync(db);
        }
        catch (Exception ex) { Fail(FriendlyDatabaseError(ex)); }
    }

    private async Task ReviewLeaveAsync(long id, bool approved)
    {
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();
            var request = await db.HrmLeaveRequests.FindAsync(id) ?? throw new InvalidOperationException("Leave request was not found.");
            if (request.Status != "Pending") { Fail("This request has already been reviewed."); return; }
            request.Status = approved ? "Approved" : "Rejected";
            request.ReviewedAt = DateTime.UtcNow;
            request.ReviewedBy = await CurrentUserNameAsync();
            if (approved)
            {
                var type = await db.HrmLeaveTypes.FindAsync(request.LeaveTypeID) ?? throw new InvalidOperationException("Leave type was not found.");
                var balance = await db.HrmLeaveBalances.SingleOrDefaultAsync(x => x.EmployeeID == request.EmployeeID && x.LeaveTypeID == request.LeaveTypeID && x.Year == request.FromDate.Year);
                if (balance is null)
                {
                    balance = new HrmLeaveBalance { EmployeeID = request.EmployeeID, LeaveTypeID = request.LeaveTypeID, Year = request.FromDate.Year, AllocatedDays = type.AnnualAllowance };
                    db.HrmLeaveBalances.Add(balance);
                }
                var available = balance.OpeningBalance + balance.AllocatedDays + balance.AdjustedDays - balance.UsedDays;
                if (type.AnnualAllowance > 0 && available < request.TotalDays)
                {
                    Fail($"Insufficient {type.Code} balance. Available: {available:0.#} day(s).");
                    return;
                }
                balance.UsedDays += request.TotalDays;
            }
            await db.SaveChangesAsync();
            await tx.CommitAsync();
            Success($"Leave request {request.Status.ToLowerInvariant()}.");
            await ReloadLeaveAsync(db);
        }
        catch (Exception ex) { Fail(FriendlyDatabaseError(ex)); }
    }

    private async Task ReloadLeaveAsync(HpmDbContext db) => leaveRequests = await db.HrmLeaveRequests.AsNoTracking().Include(x => x.Employee).ThenInclude(x => x!.Department).Include(x => x.LeaveType).OrderByDescending(x => x.AppliedAt).Take(500).ToListAsync();

    private decimal CalculateLeaveDays(DateTime from, DateTime to, bool halfDay)
    {
        if (to.Date < from.Date) return 0;
        if (halfDay) return 0.5m;
        var hospitalHolidays = holidays.Where(x => x.Scope == "Hospital").Select(x => x.HolidayDate.Date).ToHashSet();
        decimal days = 0;
        for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            if (date.DayOfWeek != DayOfWeek.Friday && !hospitalHolidays.Contains(date)) days++;
        return days;
    }

    private void EditShift(HrmShift x) => shiftEditor = new HrmShift { ID = x.ID, Code = x.Code, Name = x.Name, StartTime = x.StartTime, EndTime = x.EndTime, GraceMinutes = x.GraceMinutes, BreakMinutes = x.BreakMinutes, IsNightShift = x.IsNightShift, IsActive = x.IsActive };
    private void SetShiftStart(ChangeEventArgs e) { if (TimeSpan.TryParse(e.Value?.ToString(), out var value)) shiftEditor.StartTime = value; }
    private void SetShiftEnd(ChangeEventArgs e) { if (TimeSpan.TryParse(e.Value?.ToString(), out var value)) shiftEditor.EndTime = value; }

    private async Task SaveShiftAsync()
    {
        if (string.IsNullOrWhiteSpace(shiftEditor.Code) || string.IsNullOrWhiteSpace(shiftEditor.Name)) { Fail("Shift code and name are required."); return; }
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            shiftEditor.Code = shiftEditor.Code.Trim().ToUpperInvariant();
            shiftEditor.Name = shiftEditor.Name.Trim();
            if (shiftEditor.ID == 0) db.HrmShifts.Add(shiftEditor);
            else
            {
                var existing = await db.HrmShifts.FindAsync(shiftEditor.ID) ?? throw new InvalidOperationException("Shift was not found.");
                db.Entry(existing).CurrentValues.SetValues(shiftEditor);
            }
            await db.SaveChangesAsync();
            shifts = await db.HrmShifts.AsNoTracking().OrderBy(x => x.StartTime).ToListAsync();
            Success("Shift template saved."); ResetShift();
        }
        catch (Exception ex) { Fail(FriendlyDatabaseError(ex)); }
    }

    private async Task ChangeRosterMonthAsync(ChangeEventArgs e)
    {
        if (DateTime.TryParseExact(e.Value?.ToString(), "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var month))
        {
            rosterMonth = month;
            rosterEditor.FromDate = month;
            rosterEditor.ToDate = month.AddMonths(1).AddDays(-1);
            await using var db = await DbFactory.CreateDbContextAsync();
            var rows = await RosterQuery(db).Where(x => x.DutyDate >= month && x.DutyDate < month.AddMonths(1)).ToListAsync();
            rosterAssignments.RemoveAll(x => x.DutyDate.Year == month.Year && x.DutyDate.Month == month.Month);
            rosterAssignments.AddRange(rows);
        }
    }

    private async Task AssignRosterAsync()
    {
        if (rosterEditor.EmployeeID == 0 || rosterEditor.ShiftID == 0) { Fail("Employee and shift are required."); return; }
        if (rosterEditor.ToDate.Date < rosterEditor.FromDate.Date || (rosterEditor.ToDate - rosterEditor.FromDate).TotalDays > 62) { Fail("Choose a valid roster range of no more than 62 days."); return; }
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            var dates = Enumerable.Range(0, (rosterEditor.ToDate.Date - rosterEditor.FromDate.Date).Days + 1).Select(x => rosterEditor.FromDate.Date.AddDays(x)).Where(x => !rosterEditor.SkipFridays || x.DayOfWeek != DayOfWeek.Friday).ToList();
            var existing = await db.HrmRosterAssignments.Where(x => x.EmployeeID == rosterEditor.EmployeeID && x.DutyDate >= rosterEditor.FromDate.Date && x.DutyDate <= rosterEditor.ToDate.Date).ToDictionaryAsync(x => x.DutyDate.Date);
            foreach (var date in dates)
            {
                if (existing.TryGetValue(date, out var row)) row.ShiftID = rosterEditor.ShiftID;
                else db.HrmRosterAssignments.Add(new HrmRosterAssignment { EmployeeID = rosterEditor.EmployeeID, ShiftID = rosterEditor.ShiftID, DutyDate = date, CreatedAt = DateTime.UtcNow });
            }
            await db.SaveChangesAsync();
            Success($"Roster assigned for {dates.Count} day(s).");
            await RefreshRosterRangeAsync(db, rosterEditor.FromDate, rosterEditor.ToDate);
        }
        catch (Exception ex) { Fail(FriendlyDatabaseError(ex)); }
    }

    private async Task DeleteRosterAsync(long id)
    {
        await using var db = await DbFactory.CreateDbContextAsync();
        var row = await db.HrmRosterAssignments.FindAsync(id);
        if (row is null) return;
        db.Remove(row); await db.SaveChangesAsync();
        rosterAssignments.RemoveAll(x => x.ID == id);
        Success("Roster assignment removed.");
    }

    private async Task RefreshRosterRangeAsync(HpmDbContext db, DateTime from, DateTime to)
    {
        var rows = await RosterQuery(db).Where(x => x.DutyDate >= from.Date && x.DutyDate <= to.Date).ToListAsync();
        rosterAssignments.RemoveAll(x => x.DutyDate.Date >= from.Date && x.DutyDate.Date <= to.Date);
        rosterAssignments.AddRange(rows);
    }

    private async Task ChangeAttendanceMonthAsync(ChangeEventArgs e)
    {
        if (DateTime.TryParseExact(e.Value?.ToString(), "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var month))
        {
            attendanceMonth = month;
            await using var db = await DbFactory.CreateDbContextAsync();
            await LoadAttendanceMonthAsync(db);
        }
    }

    private async Task LoadAttendanceMonthAsync(HpmDbContext db)
    {
        var to = attendanceMonth.AddMonths(1);
        monthAttendance = await AttendanceQuery(db).Where(x => x.AttendanceDate >= attendanceMonth && x.AttendanceDate < to).ToListAsync();
    }

    private async Task SaveAttendanceAsync()
    {
        if (attendanceEditor.EmployeeID == 0) { Fail("Select an employee."); return; }
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            var record = await db.HrmAttendanceRecords.SingleOrDefaultAsync(x => x.EmployeeID == attendanceEditor.EmployeeID && x.AttendanceDate == attendanceEditor.Date.Date)
                         ?? new HrmAttendanceRecord { EmployeeID = attendanceEditor.EmployeeID, AttendanceDate = attendanceEditor.Date.Date };
            if (record.ID == 0) db.HrmAttendanceRecords.Add(record);
            record.Status = attendanceEditor.Status;
            record.Source = "Manual";
            record.Remarks = NullIfEmpty(attendanceEditor.Remarks);
            record.UpdatedAt = DateTime.UtcNow;
            var roster = await db.HrmRosterAssignments.AsNoTracking().Include(x => x.Shift).SingleOrDefaultAsync(x => x.EmployeeID == record.EmployeeID && x.DutyDate == record.AttendanceDate);
            record.ShiftID = roster?.ShiftID;
            ApplyAttendanceTimes(record, attendanceEditor.CheckIn?.ToString("HH:mm", CultureInfo.InvariantCulture), attendanceEditor.CheckOut?.ToString("HH:mm", CultureInfo.InvariantCulture), roster?.Shift);
            await db.SaveChangesAsync();
            Success("Attendance saved."); ResetAttendance();
            await LoadAttendanceMonthAsync(db);
            if (record.AttendanceDate == DateTime.Today) todayAttendance = await AttendanceQuery(db).Where(x => x.AttendanceDate == DateTime.Today).ToListAsync();
        }
        catch (Exception ex) { Fail(FriendlyDatabaseError(ex)); }
    }

    private static void ApplyAttendanceTimes(HrmAttendanceRecord record, string? checkInText, string? checkOutText, HrmShift? shift)
    {
        record.CheckIn = TryCombine(record.AttendanceDate, checkInText);
        record.CheckOut = TryCombine(record.AttendanceDate, checkOutText);
        if (record.CheckIn.HasValue && record.CheckOut.HasValue && record.CheckOut < record.CheckIn) record.CheckOut = record.CheckOut.Value.AddDays(1);
        record.WorkMinutes = record.CheckIn.HasValue && record.CheckOut.HasValue ? Math.Max(0, (int)(record.CheckOut.Value - record.CheckIn.Value).TotalMinutes) : 0;
        record.LateMinutes = 0;
        record.OvertimeMinutes = 0;
        if (shift is null) return;
        var scheduledStart = record.AttendanceDate.Date + shift.StartTime;
        var scheduledEnd = record.AttendanceDate.Date + shift.EndTime + (shift.IsNightShift || shift.EndTime <= shift.StartTime ? TimeSpan.FromDays(1) : TimeSpan.Zero);
        if (record.CheckIn.HasValue) record.LateMinutes = Math.Max(0, (int)(record.CheckIn.Value - scheduledStart).TotalMinutes - shift.GraceMinutes);
        var scheduledMinutes = Math.Max(0, (int)(scheduledEnd - scheduledStart).TotalMinutes - shift.BreakMinutes);
        record.OvertimeMinutes = Math.Max(0, record.WorkMinutes - scheduledMinutes);
    }

    private async Task PrepareAttendanceImportAsync(InputFileChangeEventArgs e)
    {
        importRows.Clear();
        try
        {
            await using var stream = e.File.OpenReadStream(5 * 1024 * 1024);
            using var reader = new StreamReader(stream, Encoding.UTF8, true);
            var lineNo = 0;
            while (await reader.ReadLineAsync() is { } line)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line)) continue;
                var columns = SplitCsvLine(line);
                if (lineNo == 1 && columns.FirstOrDefault()?.Contains("Employee", StringComparison.OrdinalIgnoreCase) == true) continue;
                var row = ParseImportRow(lineNo, columns);
                importRows.Add(row);
                if (importRows.Count >= 5000) break;
            }
            Success($"Validated {importRows.Count} CSV row(s). Review the staging preview before import.");
        }
        catch (Exception ex) { Fail($"Could not read CSV: {ex.GetBaseException().Message}"); }
    }

    private AttendanceImportRow ParseImportRow(int lineNo, IReadOnlyList<string> columns)
    {
        var row = new AttendanceImportRow { RowNo = lineNo };
        if (columns.Count < 2) { row.Error = "Expected at least EmployeeNo and Date"; return row; }
        row.EmployeeNo = columns[0].Trim();
        var employee = employees.FirstOrDefault(x => x.EmployeeNo.Equals(row.EmployeeNo, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrWhiteSpace(x.AttendanceNo) && x.AttendanceNo.Equals(row.EmployeeNo, StringComparison.OrdinalIgnoreCase)));
        if (employee is null) { row.Error = "Employee not found"; return row; }
        row.EmployeeID = employee.ID; row.EmployeeName = employee.Name;
        if (!DateTime.TryParse(columns[1], CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date) && !DateTime.TryParse(columns[1], out date)) { row.Error = "Invalid date"; return row; }
        row.Date = date.Date;
        row.CheckIn = columns.Count > 2 ? columns[2].Trim() : string.Empty;
        row.CheckOut = columns.Count > 3 ? columns[3].Trim() : string.Empty;
        row.Status = columns.Count > 4 && AttendanceStatuses.Contains(columns[4].Trim(), StringComparer.OrdinalIgnoreCase) ? AttendanceStatuses.First(x => x.Equals(columns[4].Trim(), StringComparison.OrdinalIgnoreCase)) : "Present";
        if (!string.IsNullOrWhiteSpace(row.CheckIn) && TryCombine(row.Date.Value, row.CheckIn) is null) { row.Error = "Invalid check-in"; return row; }
        if (!string.IsNullOrWhiteSpace(row.CheckOut) && TryCombine(row.Date.Value, row.CheckOut) is null) { row.Error = "Invalid check-out"; return row; }
        row.IsValid = true;
        return row;
    }

    private async Task CommitAttendanceImportAsync()
    {
        var validRows = importRows.Where(x => x.IsValid && x.Date.HasValue).ToList();
        if (validRows.Count == 0) return;
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();
            var from = validRows.Min(x => x.Date!.Value);
            var to = validRows.Max(x => x.Date!.Value);
            var employeeIds = validRows.Select(x => x.EmployeeID).Distinct().ToList();
            var existing = await db.HrmAttendanceRecords.Where(x => employeeIds.Contains(x.EmployeeID) && x.AttendanceDate >= from && x.AttendanceDate <= to).ToDictionaryAsync(x => (x.EmployeeID, x.AttendanceDate));
            var rosters = await db.HrmRosterAssignments.AsNoTracking().Include(x => x.Shift).Where(x => employeeIds.Contains(x.EmployeeID) && x.DutyDate >= from && x.DutyDate <= to).ToDictionaryAsync(x => (x.EmployeeID, x.DutyDate));
            foreach (var staged in validRows)
            {
                var key = (staged.EmployeeID, staged.Date!.Value.Date);
                if (!existing.TryGetValue(key, out var record))
                {
                    record = new HrmAttendanceRecord { EmployeeID = staged.EmployeeID, AttendanceDate = key.Item2 };
                    db.HrmAttendanceRecords.Add(record);
                    existing[key] = record;
                }
                rosters.TryGetValue(key, out var roster);
                record.ShiftID = roster?.ShiftID;
                record.Status = staged.Status;
                record.Source = "CSV Import";
                record.UpdatedAt = DateTime.UtcNow;
                ApplyAttendanceTimes(record, staged.CheckIn, staged.CheckOut, roster?.Shift);
            }
            await db.SaveChangesAsync();
            await tx.CommitAsync();
            importRows.Clear();
            Success($"Imported {validRows.Count} attendance row(s). Existing employee/date rows were updated without duplicates.");
            await LoadAttendanceMonthAsync(db);
            todayAttendance = await AttendanceQuery(db).Where(x => x.AttendanceDate == DateTime.Today).ToListAsync();
        }
        catch (Exception ex) { Fail(FriendlyDatabaseError(ex)); }
    }

    private async Task GeneratePayrollAsync()
    {
        try
        {
            var from = new DateTime(payrollYear, payrollMonth, 1);
            var to = from.AddMonths(1).AddDays(-1);
            await using var db = await DbFactory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();
            var period = await db.HrmPayrollPeriods.SingleOrDefaultAsync(x => x.Year == payrollYear && x.Month == payrollMonth);
            if (period?.Status == "Finalized") { Fail("This payroll period is finalized and cannot be regenerated."); return; }
            if (period is null)
            {
                period = new HrmPayrollPeriod { Name = from.ToString("MMMM yyyy"), Year = payrollYear, Month = payrollMonth, FromDate = from, ToDate = to, Status = "Draft", CreatedAt = DateTime.UtcNow };
                db.HrmPayrollPeriods.Add(period);
                await db.SaveChangesAsync();
            }
            else
            {
                var oldEntries = await db.HrmPayrollEntries.Where(x => x.PayrollPeriodID == period.ID).ToListAsync();
                db.HrmPayrollEntries.RemoveRange(oldEntries);
            }

            var staff = await db.HrmEmployees.AsNoTracking().Where(x => x.Status == "Active" && x.JoiningDate <= to).ToListAsync();
            var attendance = await db.HrmAttendanceRecords.AsNoTracking().Where(x => x.AttendanceDate >= from && x.AttendanceDate <= to).ToListAsync();
            var unpaidLeaveTypeIds = await db.HrmLeaveTypes.AsNoTracking().Where(x => !x.IsPaid).Select(x => x.ID).ToListAsync();
            var unpaidLeaves = await db.HrmLeaveRequests.AsNoTracking().Where(x => x.Status == "Approved" && unpaidLeaveTypeIds.Contains(x.LeaveTypeID) && x.FromDate <= to && x.ToDate >= from).ToListAsync();
            var holidayDates = await db.HrmHolidays.AsNoTracking().Where(x => x.Scope == "Hospital" && x.HolidayDate >= from && x.HolidayDate <= to).Select(x => x.HolidayDate).ToListAsync();
            var workingDays = Enumerable.Range(0, to.Day).Select(x => from.AddDays(x)).Count(x => x.DayOfWeek != DayOfWeek.Friday && !holidayDates.Any(h => h.Date == x.Date));
            workingDays = Math.Max(workingDays, 1);

            foreach (var employee in staff)
            {
                var employeeAttendance = attendance.Where(x => x.EmployeeID == employee.ID).ToList();
                var employeeUnpaidLeaves = unpaidLeaves.Where(x => x.EmployeeID == employee.ID).ToList();
                var unpaidDates = new HashSet<DateTime>();
                decimal unpaidDays = 0;
                foreach (var leave in employeeUnpaidLeaves)
                {
                    if (leave.IsHalfDay)
                    {
                        unpaidDays += .5m;
                        unpaidDates.Add(Max(leave.FromDate, from).Date);
                    }
                    else
                    {
                        var leaveFrom = Max(leave.FromDate, from).Date;
                        var leaveTo = Min(leave.ToDate, to).Date;
                        for (var date = leaveFrom; date <= leaveTo; date = date.AddDays(1))
                            if (date.DayOfWeek != DayOfWeek.Friday && !holidayDates.Any(x => x.Date == date)) unpaidDates.Add(date);
                    }
                }
                unpaidDays += unpaidDates.Count(x => !employeeUnpaidLeaves.Any(l => l.IsHalfDay && Max(l.FromDate, from).Date == x));
                var absentDays = employeeAttendance.Where(x => x.Status == "Absent" && !unpaidDates.Contains(x.AttendanceDate.Date)).Select(x => x.AttendanceDate.Date).Distinct().Count();
                var overtimeHours = Math.Round(employeeAttendance.Sum(x => x.OvertimeMinutes) / 60m, 2);
                var allowances = employee.HouseRentAllowance + employee.MedicalAllowance + employee.TransportAllowance + employee.OtherAllowance;
                var overtime = Math.Round(overtimeHours * employee.OvertimeHourlyRate, 2);
                var deductionDays = absentDays + unpaidDays;
                var deductions = Math.Round(employee.BasicSalary / workingDays * deductionDays, 2);
                var gross = employee.BasicSalary + allowances + overtime;
                db.HrmPayrollEntries.Add(new HrmPayrollEntry
                {
                    PayrollPeriodID = period.ID, EmployeeID = employee.ID, BasicSalary = employee.BasicSalary, Allowances = allowances,
                    OvertimeHours = overtimeHours, OvertimeAmount = overtime, GrossPay = gross, AbsentDays = absentDays,
                    UnpaidLeaveDays = unpaidDays, DeductionAmount = deductions, NetPay = Math.Max(0, gross - deductions), PaymentStatus = "Unpaid", GeneratedAt = DateTime.UtcNow
                });
            }
            await db.SaveChangesAsync();
            await tx.CommitAsync();
            Success($"Payroll generated for {staff.Count} active employee(s).");
            await LoadPayrollAsync(db);
        }
        catch (Exception ex) { Fail(FriendlyDatabaseError(ex)); }
    }

    private async Task FinalizePayrollAsync()
    {
        if (selectedPayrollPeriod is null || selectedPayrollPeriod.Status != "Draft") return;
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            var period = await db.HrmPayrollPeriods.FindAsync(selectedPayrollPeriod.ID) ?? throw new InvalidOperationException("Payroll period was not found.");
            period.Status = "Finalized";
            period.FinalizedAt = DateTime.UtcNow;
            period.FinalizedBy = await CurrentUserNameAsync();
            await db.SaveChangesAsync();
            Success("Payroll period finalized. Regeneration is now locked.");
            await LoadPayrollAsync(db);
        }
        catch (Exception ex) { Fail(FriendlyDatabaseError(ex)); }
    }

    private async Task ChangePayrollPeriodAsync()
    {
        await using var db = await DbFactory.CreateDbContextAsync();
        await LoadPayrollAsync(db);
    }

    private async Task LoadPayrollAsync(HpmDbContext db)
    {
        payrollPeriods = await db.HrmPayrollPeriods.AsNoTracking().OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToListAsync();
        selectedPayrollPeriod = payrollPeriods.FirstOrDefault(x => x.Year == payrollYear && x.Month == payrollMonth);
        payrollEntries = selectedPayrollPeriod is null ? [] : await db.HrmPayrollEntries.AsNoTracking().Include(x => x.Employee).ThenInclude(x => x!.Department).Where(x => x.PayrollPeriodID == selectedPayrollPeriod.ID).OrderBy(x => x.Employee!.Name).ToListAsync();
        var allEntries = await db.HrmPayrollEntries.AsNoTracking().ToListAsync();
        payrollHistory = payrollPeriods.Take(12).Select(period =>
        {
            var rows = allEntries.Where(x => x.PayrollPeriodID == period.ID).ToList();
            return new PayrollHistoryRow(period, rows.Count, rows.Sum(x => x.GrossPay), rows.Sum(x => x.DeductionAmount), rows.Sum(x => x.NetPay));
        }).ToList();
    }

    private static decimal CountWorkingDays(DateTime from, DateTime to, IReadOnlyCollection<DateTime> holidays)
    {
        decimal days = 0;
        for (var date = from.Date; date <= to.Date; date = date.AddDays(1)) if (date.DayOfWeek != DayOfWeek.Friday && !holidays.Any(x => x.Date == date)) days++;
        return days;
    }

    private async Task PrintAsync() => await JS.InvokeVoidAsync("print");
    private async Task<string> CurrentUserNameAsync() => (await AuthStateProvider.GetAuthenticationStateAsync()).User.Identity?.Name ?? "System";

    private static DateTime? TryCombine(DateTime date, string? timeText)
    {
        if (string.IsNullOrWhiteSpace(timeText)) return null;
        if (TimeSpan.TryParse(timeText, CultureInfo.InvariantCulture, out var time) || TimeSpan.TryParse(timeText, out time)) return date.Date + time;
        if (DateTime.TryParse(timeText, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out var parsed)) return date.Date + parsed.TimeOfDay;
        return null;
    }

    private static List<string> SplitCsvLine(string line)
    {
        var result = new List<string>(); var value = new StringBuilder(); var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"' && quoted && i + 1 < line.Length && line[i + 1] == '"') { value.Append('"'); i++; }
            else if (c == '"') quoted = !quoted;
            else if (c == ',' && !quoted) { result.Add(value.ToString()); value.Clear(); }
            else value.Append(c);
        }
        result.Add(value.ToString()); return result;
    }

    private static string Initials(string? name) => string.IsNullOrWhiteSpace(name) ? "?" : string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(x => char.ToUpperInvariant(x[0])));
    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static decimal EmployeeGross(HrmEmployee x) => x.BasicSalary + x.HouseRentAllowance + x.MedicalAllowance + x.TransportAllowance + x.OtherAllowance;
    private static string Money(decimal value) => $"৳{value:N2}";
    private static string FormatCompactMoney(decimal value) => value >= 1_000_000 ? $"৳{value / 1_000_000:0.##}M" : value >= 1_000 ? $"৳{value / 1_000:0.#}K" : $"৳{value:0}";
    private static string Minutes(int value) => value <= 0 ? "—" : $"{value / 60}h {value % 60}m";
    private static string FormatTime(TimeSpan value) => DateTime.Today.Add(value).ToString("h:mm tt");
    private static string FormatTime(TimeSpan? value) => value.HasValue ? FormatTime(value.Value) : "—";
    private static string FormatTimeInput(TimeSpan value) => value.ToString(@"hh\:mm");
    private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;
    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;

    private void Success(string value) { messageClass = "success"; message = value; }
    private void Fail(string value) { messageClass = "danger"; message = value; }
    private void ClearMessage() => message = string.Empty;
    private static string FriendlyDatabaseError(Exception ex)
    {
        var text = ex.GetBaseException().Message;
        if (text.Contains("duplicate", StringComparison.OrdinalIgnoreCase) || text.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)) return "A record with the same unique value already exists.";
        return text;
    }

    public void Dispose() => I18n.OnLanguageChanged -= HandleLanguageChanged;

    private sealed record NavigationItem(string Key, string Label, string Icon);
    private sealed record StatusSummaryRow(string Label, int Count, decimal Percent);
    private sealed record DepartmentSummaryRow(string Name, int Count, decimal Percent);
    private sealed record PayrollHistoryRow(HrmPayrollPeriod Period, int EmployeeCount, decimal Gross, decimal Deductions, decimal Net);

    private sealed class RosterEditor
    {
        public int EmployeeID { get; set; }
        public int ShiftID { get; set; }
        public DateTime FromDate { get; set; } = DateTime.Today;
        public DateTime ToDate { get; set; } = DateTime.Today;
        public bool SkipFridays { get; set; } = true;
    }

    private sealed class AttendanceEditor
    {
        public int EmployeeID { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public string Status { get; set; } = "Present";
        public TimeOnly? CheckIn { get; set; }
        public TimeOnly? CheckOut { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    private sealed class AttendanceImportRow
    {
        public int RowNo { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeNo { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public string CheckIn { get; set; } = string.Empty;
        public string CheckOut { get; set; } = string.Empty;
        public string Status { get; set; } = "Present";
        public bool IsValid { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
