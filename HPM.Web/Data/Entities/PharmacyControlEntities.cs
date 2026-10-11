using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class Prescription
{
    public long ID { get; set; }
    [StringLength(100)] public string? Name { get; set; }
    [StringLength(20)] public string? Age { get; set; }
    [StringLength(10)] public string? Sex { get; set; }
    [StringLength(10)] public string? BloodGroup { get; set; }
    public int? VisitNo { get; set; }
    public DateTime? VisitDate { get; set; }
    [StringLength(10)] public string? Type { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    [StringLength(10)] public string? BMI { get; set; }
    [StringLength(10)] public string? Prem { get; set; }
    [StringLength(20)] public string? Pulse { get; set; }
    [StringLength(20)] public string? Restp { get; set; }
    [StringLength(20)] public string? BP { get; set; }
    [StringLength(10)] public string? Temperature { get; set; }
    [StringLength(10)] public string? Waist { get; set; }
    [StringLength(10)] public string? Hip { get; set; }
    public string? Complain { get; set; }
    public string? MedicalHistory { get; set; }
    public string? Findings { get; set; }
    public string? Diagnosis { get; set; }
    public string? Investigation { get; set; }
    public string? Remark { get; set; }
    public string? Medicine { get; set; }
    public int? ReferredBy { get; set; }
    [StringLength(11)] public string? PatientID { get; set; }
    public long? EncounterID { get; set; }
    public long? TicketID { get; set; }
    [Required, StringLength(20)] public string Status { get; set; } = "Signed";
    public int? CreatedBy { get; set; }
    public DateTime? SignedAt { get; set; }
    public List<PrescriptionMedicine> Medicines { get; set; } = [];
}

public sealed class PrescriptionMedicine
{
    public int ID { get; set; }
    public long PrescriptionID { get; set; }
    public int ServiceID { get; set; }
    public decimal? Interval { get; set; }
    public decimal? Dose { get; set; }
    public int? Days { get; set; }
    public int Quantity { get; set; }
    public int SLNO { get; set; }
    public Prescription? Prescription { get; set; }
}

public sealed class PrescriptionDispense
{
    public long ID { get; set; }
    public long PrescriptionID { get; set; }
    public long CashMemoID { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
    public List<PrescriptionDispenseItem> Items { get; set; } = [];
}

public sealed class PrescriptionDispenseItem
{
    public long ID { get; set; }
    public long DispenseID { get; set; }
    public int PrescriptionMedicineID { get; set; }
    public int ServiceID { get; set; }
    public decimal Quantity { get; set; }
    public PrescriptionDispense? Dispense { get; set; }
}

public sealed class PharmacySalesReturn
{
    public long ID { get; set; }
    public long CashMemoID { get; set; }
    public int StoreID { get; set; }
    public decimal RefundAmount { get; set; }
    [Required, StringLength(500)] public string Reason { get; set; } = string.Empty;
    public bool IsPosted { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
    public List<PharmacySalesReturnItem> Items { get; set; } = [];
}

public sealed class PharmacySalesReturnItem
{
    public long ID { get; set; }
    public long SalesReturnID { get; set; }
    public long CashMemoSubID { get; set; }
    public int ServiceID { get; set; }
    public int? StockBalanceID { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal BuyRate { get; set; }
    public bool IsRestocked { get; set; }
    public PharmacySalesReturn? SalesReturn { get; set; }
}

public sealed class MedicineStockAdjustment
{
    public long ID { get; set; }
    public int StoreID { get; set; }
    [Required, StringLength(30)] public string AdjustmentType { get; set; } = string.Empty;
    [Required, StringLength(500)] public string Reason { get; set; } = string.Empty;
    public bool IsPosted { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
    public List<MedicineStockAdjustmentItem> Items { get; set; } = [];
}

public sealed class MedicineStockAdjustmentItem
{
    public long ID { get; set; }
    public long AdjustmentID { get; set; }
    public int StockBalanceID { get; set; }
    public int ServiceID { get; set; }
    [StringLength(50)] public string? BatchNo { get; set; }
    public DateTime? ExpireDate { get; set; }
    public decimal BeforeQuantity { get; set; }
    public decimal ChangeQuantity { get; set; }
    public decimal AfterQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public MedicineStockAdjustment? Adjustment { get; set; }
}

public sealed class MedicineStockOpening
{
    public long ID { get; set; }
    public int StoreID { get; set; }
    public int ServiceID { get; set; }
    public decimal OpeningBalance { get; set; }
    public DateTime CapturedAt { get; set; }
}
