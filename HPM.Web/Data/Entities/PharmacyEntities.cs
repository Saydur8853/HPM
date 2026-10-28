using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class GenericGroup
{
    public int ID { get; set; }
    [StringLength(200)] public string? Name { get; set; }
    public int? EnteredBy { get; set; }
    public DateTime? TransDate { get; set; }
}

public sealed class MedicineType
{
    public int ID { get; set; }
    [Required, StringLength(100)] public string Type { get; set; } = string.Empty;
    public int? EnteredBy { get; set; }
    public DateTime? TransDate { get; set; }
}

public sealed class SupplierPharmacy
{
    public int ID { get; set; }
    [StringLength(50)] public string? Name { get; set; }
    [StringLength(100)] public string? Address { get; set; }
    [StringLength(50)] public string? Contact { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? TransDate { get; set; }
}

public sealed class MedicineStore
{
    public int ID { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsBufferStore { get; set; }
}

public sealed class MedicineStoreBalance
{
    public int ID { get; set; }
    public int ServiceID { get; set; }
    public int MedicineStoreID { get; set; }
    public decimal Balance { get; set; }
    public DateTime? ExpireDate { get; set; }
    public long? MedicineReceivingChallanSubID { get; set; }
    [StringLength(50)] public string? BatchNo { get; set; }
}

public sealed class MedicineReceivingChallan
{
    public long ID { get; set; }
    public int Supplier { get; set; }
    [Required, StringLength(100)] public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public bool IsPosted { get; set; }
    public decimal Total { get; set; }
    public decimal Discount { get; set; }
    public decimal VAT { get; set; }
    public int? StoreID { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
    public List<MedicineReceivingChallanItem> Items { get; set; } = [];
}

public sealed class MedicineReceivingChallanItem
{
    public long ID { get; set; }
    public long ChallanNo { get; set; }
    public int ServiceID { get; set; }
    public int SLNO { get; set; }
    public decimal Total { get; set; }
    public decimal BuyRate { get; set; }
    public decimal Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal VAT { get; set; }
    public DateTime? ExpireDate { get; set; }
    [StringLength(50)] public string? BatchNo { get; set; }
    public decimal? SaleRate { get; set; }
    public MedicineReceivingChallan? Challan { get; set; }
}

public sealed class PharmacyCashMemo
{
    public long ID { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
    [StringLength(20)] public string? Contact { get; set; }
    [StringLength(10)] public string? RegNo { get; set; }
    [Required, StringLength(1)] public string PatientType { get; set; } = "1";
    public decimal? Total { get; set; }
    public decimal? Discount { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
    public bool IsPackage { get; set; }
    public bool? IsCredit { get; set; }
    public int? BufferStore { get; set; }
    public List<PharmacyCashMemoItem> Items { get; set; } = [];
}

public sealed class PharmacyCashMemoItem
{
    public long ID { get; set; }
    public long BillNo { get; set; }
    public int? ServiceID { get; set; }
    public int? SLNO { get; set; }
    public decimal? Rate { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? BuyRate { get; set; }
    public PharmacyCashMemo? Memo { get; set; }
    public List<PharmacySaleBatchAllocation> BatchAllocations { get; set; } = [];
}

public sealed class PharmacySaleBatchAllocation
{
    public long ID { get; set; }
    public long CashMemoItemID { get; set; }
    public int StockBalanceID { get; set; }
    public decimal Quantity { get; set; }
    public decimal BuyRate { get; set; }
    [StringLength(50)] public string? BatchNo { get; set; }
    public DateTime? ExpireDate { get; set; }
    public PharmacyCashMemoItem? CashMemoItem { get; set; }
    public MedicineStoreBalance? StockBalance { get; set; }
}

public sealed class SupplierLedger
{
    public long ID { get; set; }
    public int SupplierID { get; set; }
    public long? ChallanNo { get; set; }
    public long? SupplierPaymentID { get; set; }
    public decimal? Debit { get; set; }
    public decimal? Credit { get; set; }
    public decimal Balance { get; set; }
    public DateTime TransDate { get; set; }
}

public sealed class MedicineReturn
{
    public int ID { get; set; }
    public int SupplierID { get; set; }
    public int StoreID { get; set; }
    public bool IsPosted { get; set; }
    public DateTime TransDate { get; set; }
    public int EnteredBy { get; set; }
    public List<MedicineReturnItem> Items { get; set; } = [];
}

public sealed class MedicineReturnItem
{
    public int ID { get; set; }
    public int MedicineReturnID { get; set; }
    public int ServiceID { get; set; }
    public int SLNO { get; set; }
    public decimal Quantity { get; set; }
    public decimal BuyRate { get; set; }
    public MedicineReturn? MedicineReturn { get; set; }
}

public sealed class SupplierPayment
{
    public long ID { get; set; }
    public int SupplierID { get; set; }
    public decimal Amount { get; set; }
    [StringLength(50)] public string? PaymentMode { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
}

public sealed class PurchaseRequisition
{
    public long ID { get; set; }
    public int StoreID { get; set; }
    [Required, StringLength(20)] public string RequisitionNo { get; set; } = string.Empty;
    [StringLength(20)] public string Status { get; set; } = "Pending";
    public int RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; }
    public int? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    [StringLength(500)] public string? Remarks { get; set; }
    [StringLength(500)] public string? ReviewNote { get; set; }
    public DateTime TransDate { get; set; }
    public List<PurchaseRequisitionItem> Items { get; set; } = [];
}

public sealed class PurchaseRequisitionItem
{
    public long ID { get; set; }
    public long RequisitionID { get; set; }
    public int SLNO { get; set; }
    public int ServiceID { get; set; }
    public decimal RequestedQty { get; set; }
    public decimal? EstimatedRate { get; set; }
    [StringLength(200)] public string? Remarks { get; set; }
    public PurchaseRequisition? Requisition { get; set; }
}
