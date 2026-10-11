using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class PatientAdmission
{
    public long ID { get; set; }
    [Required, StringLength(10)] public string RegNo { get; set; } = string.Empty;
    [StringLength(11)] public string? PatientID { get; set; }
    public int? Department { get; set; }
    [StringLength(200)] public string? Diagnosis { get; set; }
    [StringLength(20)] public string? Age { get; set; }
    public int? ReferredBy { get; set; }
    public int? Consultant { get; set; }
    public int? AdmittedBy { get; set; }
    public bool? IsReferred { get; set; }
    public int? BedCabinID { get; set; }
    public decimal? AdmissionFee { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public bool? IsAdmitted { get; set; }
    public DateTime? BedAdmissionDate { get; set; }
    [StringLength(200)] public string? Comments { get; set; }
    public long? BillNo { get; set; }
    public int? BillGeneratedBy { get; set; }
    public int? PromotionOfficerID { get; set; }
    public bool? IsPackage { get; set; }
    public DateTime? TransDate { get; set; }
    public int? EnteredBy { get; set; }
    public decimal? AmbulanceFee { get; set; }
    [StringLength(50)] public string? Outcome { get; set; }
    [StringLength(50)] public string? PatientCategory { get; set; }
    public int? PatientReceivedBy { get; set; }
    public DateTime? PatientReceivedDate { get; set; }
}

public sealed class AdmissionExtension
{
    public long ID { get; set; }
    [Required, StringLength(10)] public string RegNo { get; set; } = string.Empty;
    public decimal? ServiceCharge { get; set; }
    public decimal? Food { get; set; }
}

public sealed class IpdDepartment
{
    public int ID { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
}

public sealed class BedCategory
{
    public int ID { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
    public decimal? Rate { get; set; }
    [StringLength(200)] public string? Specification { get; set; }
    public int? IpdDepartmentID { get; set; }
    public decimal? AdmissionFee { get; set; }
    public decimal? ServiceCharge { get; set; }
    public decimal? Food { get; set; }
}

public sealed class BedCabin
{
    public int ID { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
    public int CategoryID { get; set; }
    public bool IsAllocated { get; set; }
    [Required, StringLength(20)] public string Status { get; set; } = BedStatuses.Available;
}

public static class BedStatuses
{
    public const string Available = "Available";
    public const string Occupied = "Occupied";
    public const string Cleaning = "Cleaning";
    public const string Maintenance = "Maintenance";
}

public sealed class BedTurnover
{
    public long ID { get; set; }
    public int BedCabinID { get; set; }
    public long? AdmissionID { get; set; }
    [StringLength(10)] public string? RegNo { get; set; }
    [Required, StringLength(30)] public string Status { get; set; } = "Cleaning";
    public DateTime RequestedAt { get; set; } = DateTime.Now;
    public int? CleaningCompletedBy { get; set; }
    public DateTime? CleaningCompletedAt { get; set; }
    [StringLength(500)] public string? CleaningNotes { get; set; }
    public int? ReleasedBy { get; set; }
    public DateTime? ReleasedAt { get; set; }
}

public sealed class BedCabinTransaction
{
    public long ID { get; set; }
    public int BedCabinID { get; set; }
    [Required, StringLength(10)] public string RegNo { get; set; } = string.Empty;
    [StringLength(11)] public string? PatientID { get; set; }
    public decimal? Rate { get; set; }
    public long? PatientTransactionID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public sealed class IpdMoneyReceipt
{
    public long ID { get; set; }
    [Required, StringLength(10)] public string RegNo { get; set; } = string.Empty;
    [Required, StringLength(50)] public string Against { get; set; } = "Advance";
    public decimal Amount { get; set; }
    public DateTime TransDate { get; set; }
    public int EnteredBy { get; set; }
}

public sealed class IpdFinalBill
{
    public long BillNo { get; set; }
    public long AdmissionID { get; set; }
    public long? MedicalDischargeOrderID { get; set; }
    [Required, StringLength(10)] public string RegNo { get; set; } = string.Empty;
    public decimal AdmissionFee { get; set; }
    public decimal BedCharges { get; set; }
    public decimal ServiceCharges { get; set; }
    public decimal HospitalCharges { get; set; }
    public decimal AmbulanceCharges { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    [StringLength(100)] public string? DiscountAuthorizedBy { get; set; }
    public decimal NetAmount { get; set; }
    public decimal ReceiptAmount { get; set; }
    public decimal AppliedAdvanceAmount { get; set; }
    public decimal BalanceDue { get; set; }
    public decimal ExcessPayment { get; set; }
    [Required, StringLength(20)] public string SettlementStatus { get; set; } = "Due";
    public DateTime DischargeDate { get; set; }
    [Required, StringLength(50)] public string Outcome { get; set; } = string.Empty;
    [StringLength(1000)] public string? DischargeNotes { get; set; }
    public int GeneratedBy { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<IpdFinalBillReceipt> Receipts { get; set; } = [];
}

public sealed class IpdMedicalDischargeOrder
{
    public long ID { get; set; }
    public long AdmissionID { get; set; }
    [Required, StringLength(10)] public string RegNo { get; set; } = string.Empty;
    public int DoctorID { get; set; }
    [Required, StringLength(20)] public string Status { get; set; } = "Ready";
    [Required, StringLength(50)] public string Outcome { get; set; } = "Improved";
    [Required, StringLength(1000)] public string FinalDiagnosis { get; set; } = string.Empty;
    [StringLength(500)] public string? ConditionAtDischarge { get; set; }
    [Required, StringLength(1000)] public string Advice { get; set; } = string.Empty;
    [StringLength(500)] public string? FollowUp { get; set; }
    public int OrderedBy { get; set; }
    public DateTime OrderedAt { get; set; } = DateTime.Now;
    public int? CompletedBy { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public sealed class IpdFinalBillReceipt
{
    public long ID { get; set; }
    public long BillNo { get; set; }
    public long MoneyReceiptID { get; set; }
    public decimal Amount { get; set; }
    [Required, StringLength(50)] public string Against { get; set; } = string.Empty;
    public IpdFinalBill? Bill { get; set; }
}

public sealed class PatientTransaction
{
    public long ID { get; set; }
    [Required, StringLength(10)] public string RegNo { get; set; } = string.Empty;
    public int ServiceID { get; set; }
    public decimal ServiceRate { get; set; }
    public decimal Quantity { get; set; }
    public int? IpdDepartmentID { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
}

public sealed class PatientPackageTransaction
{
    public long ID { get; set; }
    [Required, StringLength(10)] public string RegNo { get; set; } = string.Empty;
    public int ServiceID { get; set; }
    public decimal ServiceRate { get; set; }
    public decimal Quantity { get; set; }
    public int? IpdDepartmentID { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
}

public sealed class PatientPayment
{
    public long ID { get; set; }
    public long MoneyReceiptID { get; set; }
    [Required, StringLength(10)] public string RegNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
    public bool IsHospitalPayment { get; set; }
}

public sealed class CentralCashCollection
{
    public long ID { get; set; }
    public decimal Amount { get; set; }
    public int ModuleFlag { get; set; }
    public int HeadID { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
    public DateTime TransDateCL { get; set; }
    public long? ReferenceID { get; set; }
}

public sealed class IpdPatientServiceItem
{
    public long ID { get; set; }
    [Required, StringLength(10)] public string RegNo { get; set; } = string.Empty;
    [StringLength(11)] public string? PatientID { get; set; }
    public int ServiceID { get; set; }
    [StringLength(200)] public string ServiceName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal Total { get; set; }
    public int? DoctorID { get; set; }
    public DateTime ServiceDate { get; set; }
    public int? EnteredBy { get; set; }
    [StringLength(200)] public string? Remarks { get; set; }
}
