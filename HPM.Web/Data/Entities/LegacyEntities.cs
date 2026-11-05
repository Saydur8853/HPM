using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class Registration
{
    public long ID { get; set; }
    [Required, StringLength(11)] public string PatientID { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    [Required, StringLength(10)] public string Sex { get; set; } = string.Empty;
    [StringLength(10)] public string? BloodGroup { get; set; }
    [StringLength(20)] public string? Weight { get; set; }
    public string? PhotoData { get; set; }
    [StringLength(100)] public string? FatherName { get; set; }
    [StringLength(100)] public string? MotherName { get; set; }
    [StringLength(50)] public string? CareOf { get; set; }
    [StringLength(200)] public string? PermanentVillage { get; set; }
    [StringLength(100)] public string? PermanentPO { get; set; }
    [StringLength(100)] public string? PermanentPS { get; set; }
    [StringLength(50)] public string? PermanentDistrict { get; set; }
    [StringLength(200)] public string? PresentVillage { get; set; }
    [StringLength(100)] public string? PresentPO { get; set; }
    [StringLength(100)] public string? PresentPS { get; set; }
    [StringLength(50)] public string? PresentDistrict { get; set; }
    [StringLength(50)] public string? Contact { get; set; }
    [EmailAddress, StringLength(50)] public string? Email { get; set; }
    [StringLength(50)] public string? Occupation { get; set; }
    public int EnteredBy { get; set; } = 1;
    public DateTime TransDate { get; set; } = DateTime.Now;
}

public sealed class DoctorAppointment
{
    public int ID { get; set; }
    [Range(1, int.MaxValue)] public int DoctorID { get; set; }
    public int ApptSLNO { get; set; }
    public DateTime ApptDate { get; set; } = DateTime.Today;
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [StringLength(50)] public string? Age { get; set; }
    [StringLength(50)] public string? Sex { get; set; }
    [StringLength(20)] public string? Weight { get; set; }
    [StringLength(20)] public string? ContactNo { get; set; }
    [StringLength(11)] public string? PatientID { get; set; }
    [Required, StringLength(30)] public string Status { get; set; } = "Booked";
    public int EnteredBy { get; set; } = 1;
    public DateTime TransDate { get; set; } = DateTime.Now;
}

public sealed class OPDTicket
{
    public long ID { get; set; }
    public DateTime TransDate { get; set; } = DateTime.Now;
    [StringLength(50)] public string? RegNo { get; set; }
    [StringLength(50)] public string? TicketRate { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
    [StringLength(50)] public string? Age { get; set; }
    [StringLength(50)] public string? Sex { get; set; }
    public int OPD { get; set; }
    public decimal Vat { get; set; }
    [Range(1, int.MaxValue)] public int DoctorID { get; set; }
    [Range(1, int.MaxValue)] public int DeptID { get; set; }
    [StringLength(50)] public string? Room { get; set; }
    [StringLength(200)] public string? Comments { get; set; }
    [Range(0, double.MaxValue)] public decimal Total { get; set; }
    public bool Posted { get; set; } = true;
    public int EnteredBy { get; set; } = 1;
}

public sealed class CashMemoDInvestigation
{
    public long ID { get; set; }
    [Required, StringLength(100)] public string PatientName { get; set; } = string.Empty;
    [StringLength(1)] public string PatientType { get; set; } = "1";
    [StringLength(10)] public string? Bed { get; set; }
    [StringLength(50)] public string? Age { get; set; }
    [StringLength(10)] public string? Sex { get; set; }
    [StringLength(200)] public string? Contact { get; set; }
    [Range(0, 9999999999999999d)] public decimal Total { get; set; }
    [Range(0, 9999999999999999d)] public decimal Discount { get; set; }
    [StringLength(50)] public string? DiscountBy { get; set; }
    [Range(0, 9999999999999999d)] public decimal Paid { get; set; }
    [StringLength(50)] public string? RegNo { get; set; }
    public DateTime TransDate { get; set; } = DateTime.Now;
    [StringLength(11)] public string? PatientID { get; set; }
    public int? DoctorID { get; set; }
    public int? ModuleID { get; set; }
    public int EnteredBy { get; set; } = 1;
    public long? EncounterID { get; set; }
    [StringLength(200)] public string? Remarks { get; set; }
    public List<CashMemoSubDInvestigation> Items { get; set; } = [];
}

public sealed class CashMemoSubDInvestigation
{
    public long ID { get; set; }
    public long MemoID { get; set; }
    public int SLNo { get; set; }
    public int TestID { get; set; }
    [Range(0, 9999999999999999d)] public decimal Rate { get; set; }
    [Range(1, int.MaxValue)] public int Quantity { get; set; } = 1;
    public CashMemoDInvestigation? Memo { get; set; }
}

public sealed class CashMemoOT
{
    public long ID { get; set; }
    [Required, StringLength(100)] public string PatientName { get; set; } = string.Empty;
    [StringLength(1)] public string PatientType { get; set; } = "1";
    [StringLength(10)] public string? Bed { get; set; }
    [StringLength(50)] public string? Age { get; set; }
    [StringLength(10)] public string? Sex { get; set; }
    [StringLength(200)] public string? Contact { get; set; }
    [StringLength(11)] public string? PatientID { get; set; }
    [StringLength(50)] public string? RegNo { get; set; }
    public int? DoctorID { get; set; }
    public int? OT { get; set; }
    [Range(0, 9999999999999999d)] public decimal Total { get; set; }
    [Range(0, 9999999999999999d)] public decimal Discount { get; set; }
    [StringLength(50)] public string? DiscountBy { get; set; }
    public int EnteredBy { get; set; } = 1;
    public DateTime TransDate { get; set; } = DateTime.Now;
    public List<CashMemoSubOT> Items { get; set; } = [];
}

public sealed class CashMemoSubOT
{
    public long ID { get; set; }
    public long MemoID { get; set; }
    public int SLNO { get; set; }
    public int ServiceID { get; set; }
    [Range(0, 9999999999999999d)] public decimal Rate { get; set; }
    [Range(1, int.MaxValue)] public int Quantity { get; set; } = 1;
    public int? DoctorID { get; set; }
    public CashMemoOT? Memo { get; set; }
}

public sealed class DoctorsProfile
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Degree { get; set; }
    public string? Attachment { get; set; } = "Temporary";
    public int? POSerial { get; set; }
    public int? AreaCode { get; set; }
    public decimal? Balance { get; set; }
    public decimal? OpeningBalance { get; set; }
    public string? Contact { get; set; }
    public string? Email { get; set; }
    public string? Room { get; set; }
    public int? Department { get; set; }
    public int? EnteredBy { get; set; }
    public DateTime? TransDate { get; set; }
    public string? Address { get; set; }
    public int? ReferralCategoryID { get; set; }
    public bool IsActive { get; set; } = true;
    public int? Priority { get; set; }
    [StringLength(100)] public string? ConsultationTime { get; set; }
    [StringLength(100)] public string? OffDay { get; set; } = "Friday";
    [Range(0, 1000000)] public decimal? ConsultationFee { get; set; }
    [Range(0, 1000000)] public decimal? OpdConsultationFee { get; set; }
}

public sealed class Department
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? DepartmentCategoryID { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class TicketType
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class ServiceCategory
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? SubID { get; set; }
    public int? ModuleID { get; set; }
    public decimal? ServiceCharge { get; set; }
    public bool? IsPercentage { get; set; }
    public int? GroupSubID { get; set; }
    public int? InvestigationSLNO { get; set; }
}

public sealed class Service
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public decimal? SaleRate { get; set; }
    public decimal? BuyRate { get; set; }
    public int? ReOrderLevel { get; set; }
    public bool? IsVATApp { get; set; }
    public int? CategoryID { get; set; }
    public int? TypeID { get; set; }
    public int? GenericID { get; set; }
    public int? SupplierID { get; set; }
    public int? InvestigationModuleID { get; set; }
    public string? DiagnosticType { get; set; }
    public int? DeliveryHours { get; set; }
}
