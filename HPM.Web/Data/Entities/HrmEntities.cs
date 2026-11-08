using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class HrmDepartment
{
    public int ID { get; set; }
    [Required, MaxLength(20)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    public bool IsClinical { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class HrmDesignation
{
    public int ID { get; set; }
    [Required, MaxLength(20)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class HrmShift
{
    public int ID { get; set; }
    [Required, MaxLength(20)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int GraceMinutes { get; set; } = 15;
    public int BreakMinutes { get; set; }
    public bool IsNightShift { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class HrmEmployee
{
    public int ID { get; set; }
    [Required, MaxLength(30)] public string EmployeeNo { get; set; } = string.Empty;
    [MaxLength(50)] public string? AttendanceNo { get; set; }
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    [MaxLength(20)] public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    [MaxLength(30)] public string? Phone { get; set; }
    [MaxLength(150)] public string? Email { get; set; }
    [MaxLength(300)] public string? Address { get; set; }
    [MaxLength(150)] public string? EmergencyContact { get; set; }
    public int DepartmentID { get; set; }
    public int DesignationID { get; set; }
    public DateTime JoiningDate { get; set; } = DateTime.Today;
    [Required, MaxLength(30)] public string EmploymentType { get; set; } = "Permanent";
    [Required, MaxLength(30)] public string Status { get; set; } = "Active";
    [MaxLength(80)] public string? BankAccount { get; set; }
    [MaxLength(80)] public string? TaxIdentifier { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal HouseRentAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal OtherAllowance { get; set; }
    public decimal OvertimeHourlyRate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public HrmDepartment? Department { get; set; }
    public HrmDesignation? Designation { get; set; }
}

public sealed class HrmHoliday
{
    public int ID { get; set; }
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    public DateTime HolidayDate { get; set; } = DateTime.Today;
    [Required, MaxLength(20)] public string Scope { get; set; } = "Hospital";
    public int? DepartmentID { get; set; }
    public bool IsPaid { get; set; } = true;
    [MaxLength(300)] public string? Notes { get; set; }
    public HrmDepartment? Department { get; set; }
}

public sealed class HrmLeaveType
{
    public int ID { get; set; }
    [Required, MaxLength(20)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    public decimal AnnualAllowance { get; set; }
    public bool IsPaid { get; set; } = true;
    public bool RequiresAttachment { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class HrmLeaveRequest
{
    public long ID { get; set; }
    public int EmployeeID { get; set; }
    public int LeaveTypeID { get; set; }
    public DateTime FromDate { get; set; } = DateTime.Today;
    public DateTime ToDate { get; set; } = DateTime.Today;
    public decimal TotalDays { get; set; } = 1;
    public bool IsHalfDay { get; set; }
    [Required, MaxLength(500)] public string Reason { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string Status { get; set; } = "Pending";
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    [MaxLength(150)] public string? ReviewedBy { get; set; }
    [MaxLength(500)] public string? ReviewNote { get; set; }

    public HrmEmployee? Employee { get; set; }
    public HrmLeaveType? LeaveType { get; set; }
}

public sealed class HrmLeaveBalance
{
    public long ID { get; set; }
    public int EmployeeID { get; set; }
    public int LeaveTypeID { get; set; }
    public int Year { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal AllocatedDays { get; set; }
    public decimal UsedDays { get; set; }
    public decimal AdjustedDays { get; set; }

    public HrmEmployee? Employee { get; set; }
    public HrmLeaveType? LeaveType { get; set; }
}

public sealed class HrmRosterAssignment
{
    public long ID { get; set; }
    public int EmployeeID { get; set; }
    public int ShiftID { get; set; }
    public DateTime DutyDate { get; set; } = DateTime.Today;
    [MaxLength(300)] public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public HrmEmployee? Employee { get; set; }
    public HrmShift? Shift { get; set; }
}

public sealed class HrmAttendanceRecord
{
    public long ID { get; set; }
    public int EmployeeID { get; set; }
    public int? ShiftID { get; set; }
    public DateTime AttendanceDate { get; set; } = DateTime.Today;
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    [Required, MaxLength(20)] public string Status { get; set; } = "Present";
    public int WorkMinutes { get; set; }
    public int LateMinutes { get; set; }
    public int OvertimeMinutes { get; set; }
    [Required, MaxLength(30)] public string Source { get; set; } = "Manual";
    [MaxLength(300)] public string? Remarks { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public HrmEmployee? Employee { get; set; }
    public HrmShift? Shift { get; set; }
}

public sealed class HrmPayrollPeriod
{
    public long ID { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    [Required, MaxLength(20)] public string Status { get; set; } = "Draft";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinalizedAt { get; set; }
    [MaxLength(150)] public string? FinalizedBy { get; set; }
}

public sealed class HrmPayrollEntry
{
    public long ID { get; set; }
    public long PayrollPeriodID { get; set; }
    public int EmployeeID { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal OvertimeAmount { get; set; }
    public decimal GrossPay { get; set; }
    public decimal AbsentDays { get; set; }
    public decimal UnpaidLeaveDays { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetPay { get; set; }
    [Required, MaxLength(20)] public string PaymentStatus { get; set; } = "Unpaid";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public HrmPayrollPeriod? PayrollPeriod { get; set; }
    public HrmEmployee? Employee { get; set; }
}
