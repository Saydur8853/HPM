using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPM.Web.Data.Entities;

[Table("Store_Product_Category")]
public sealed class StoreProductCategory
{
    public int ID { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
}

[Table("Supplier_Store")]
public sealed class SupplierStore
{
    public int ID { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
    [StringLength(100)] public string? Address { get; set; }
    [StringLength(50)] public string? Contact { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal Amount { get; set; }
    [Column("Trans_Date")] public DateTime? TransDate { get; set; }
}

[Table("Store_Product")]
public sealed class StoreProduct
{
    public int ID { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(50)] public string Unit { get; set; } = "Pcs";
    [Column("Category")] public int CategoryID { get; set; }
    public int Supplier { get; set; }
    [Column("Re_Order_Level")] public int ReOrderLevel { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Balance { get; set; }
    [Column("Buy_Rate", TypeName = "decimal(18,2)")] public decimal BuyRate { get; set; }
    [Column("Sale_Rate", TypeName = "decimal(18,2)")] public decimal SaleRate { get; set; }
    [Column("Entered_By")] public int EnteredBy { get; set; }
    [Column("Trans_Date")] public DateTime TransDate { get; set; } = DateTime.Now;
    [Column("Is_Active")] public bool IsActive { get; set; } = true;
    /// <summary>Items such as reagents/consumables whose batches carry an expiry date.</summary>
    [Column("Track_Expiry")] public bool TrackExpiry { get; set; }

    [ForeignKey(nameof(CategoryID))]
    public StoreProductCategory? Category { get; set; }

    [ForeignKey(nameof(Supplier))]
    public SupplierStore? SupplierStore { get; set; }
}

[Table("Store_Challan")]
public sealed class StoreChallan
{
    public long ID { get; set; }
    public int Supplier { get; set; }
    [Required, StringLength(20), Column("Invoice_No")] public string InvoiceNo { get; set; } = string.Empty;
    [Column("Invoice_Date")] public DateTime InvoiceDate { get; set; } = DateTime.Today;
    [Column("Is_Posted")] public bool IsPosted { get; set; } = true;
    [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
    [Column("Entered_By")] public int EnteredBy { get; set; } = 1;
    [Column("Trans_Date")] public DateTime TransDate { get; set; } = DateTime.Now;
    [Column("Location_ID")] public int? LocationID { get; set; }
    [StringLength(300)] public string? Note { get; set; }

    [ForeignKey(nameof(Supplier))]
    public SupplierStore? SupplierStore { get; set; }

    public List<StoreChallanSub> Items { get; set; } = [];
}

[Table("Store_Challan_Sub")]
public sealed class StoreChallanSub
{
    public long ID { get; set; }
    [Column("Challan_No")] public long ChallanNo { get; set; }
    [Column("Product_ID")] public int ProductID { get; set; }
    public int SLNO { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
    [Column("Buy_Rate", TypeName = "numeric(18,2)")] public decimal BuyRate { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal Quantity { get; set; }
    [StringLength(50), Column("Batch_No")] public string? BatchNo { get; set; }
    [Column("Expire_Date", TypeName = "date")] public DateTime? ExpireDate { get; set; }
    [Column("Stock_ID")] public int? StockID { get; set; }

    [ForeignKey(nameof(ChallanNo))]
    public StoreChallan? Challan { get; set; }

    [ForeignKey(nameof(ProductID))]
    public StoreProduct? Product { get; set; }
}

[Table("Store_Issue")]
public sealed class StoreIssue
{
    public long ID { get; set; }
    [Column("Issue_Department")] public int IssueDepartment { get; set; }
    [Required, StringLength(10)] public string IndentNo { get; set; } = string.Empty;
    [Column(TypeName = "numeric(18,2)")] public decimal? Total { get; set; }
    [Column("Entered_By")] public int EnteredBy { get; set; } = 1;
    [Column("Trans_Date")] public DateTime TransDate { get; set; } = DateTime.Now;
    [Column("Location_ID")] public int? LocationID { get; set; }
    [Column("Requisition_ID")] public long? RequisitionID { get; set; }
    [StringLength(100), Column("Received_By")] public string? ReceivedBy { get; set; }
    [StringLength(300)] public string? Note { get; set; }

    public List<StoreIssueSub> Items { get; set; } = [];
}

[Table("Store_Issue_Sub")]
public sealed class StoreIssueSub
{
    public long ID { get; set; }
    [Column("Issue_No")] public long IssueNo { get; set; }
    [Column("Product_ID")] public int? ProductID { get; set; }
    public int? SLNO { get; set; }
    [Column("Buy_Rate", TypeName = "numeric(18,2)")] public decimal? BuyRate { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal? Quantity { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal? Total { get; set; }
    [Column("Stock_ID")] public int? StockID { get; set; }
    [StringLength(50), Column("Batch_No")] public string? BatchNo { get; set; }
    [Column("Expire_Date", TypeName = "date")] public DateTime? ExpireDate { get; set; }

    [ForeignKey(nameof(IssueNo))]
    public StoreIssue? Issue { get; set; }

    [ForeignKey(nameof(ProductID))]
    public StoreProduct? Product { get; set; }
}


/// <summary>A physical store / sub-store (Central Store, Laboratory Store, OT Store...).</summary>
[Table("Store_Location")]
public sealed class StoreLocation
{
    public int ID { get; set; }
    [Required, StringLength(60)] public string Name { get; set; } = string.Empty;
    [Column("Is_Central")] public bool IsCentral { get; set; }
    [Column("Is_Active")] public bool IsActive { get; set; } = true;
    [Column("Sort_Order")] public int SortOrder { get; set; }
}

/// <summary>Hospital department / cost centre that requests and consumes store items.</summary>
[Table("Store_Department")]
public sealed class StoreDepartment
{
    public int ID { get; set; }
    [Required, StringLength(80)] public string Name { get; set; } = string.Empty;
    [Column("Is_Active")] public bool IsActive { get; set; } = true;
}

/// <summary>Batch-wise balance of one item in one store location.</summary>
[Table("Store_Stock")]
public sealed class StoreStock
{
    public int ID { get; set; }
    [Column("Location_ID")] public int LocationID { get; set; }
    [Column("Product_ID")] public int ProductID { get; set; }
    [StringLength(50), Column("Batch_No")] public string? BatchNo { get; set; }
    [Column("Expire_Date", TypeName = "date")] public DateTime? ExpireDate { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Balance { get; set; }
    [Column("Buy_Rate", TypeName = "decimal(18,2)")] public decimal BuyRate { get; set; }
    [Column("Received_Date")] public DateTime ReceivedDate { get; set; } = DateTime.Now;
}

/// <summary>Every stock movement (in or out) - the single source for item history and consumption reports.</summary>
[Table("Store_Stock_Ledger")]
public sealed class StoreStockLedger
{
    public long ID { get; set; }
    [Column("Trans_Date")] public DateTime TransDate { get; set; } = DateTime.Now;
    [Column("Location_ID")] public int LocationID { get; set; }
    [Column("Product_ID")] public int ProductID { get; set; }
    [Column("Stock_ID")] public int? StockID { get; set; }
    [Column("Qty_In", TypeName = "decimal(18,2)")] public decimal QtyIn { get; set; }
    [Column("Qty_Out", TypeName = "decimal(18,2)")] public decimal QtyOut { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Rate { get; set; }
    /// <summary>OPENING, GRN, ISSUE, RETURN, TRANSFER_IN, TRANSFER_OUT, ADJUST.</summary>
    [Required, StringLength(20), Column("Ref_Type")] public string RefType { get; set; } = string.Empty;
    [Column("Ref_ID")] public long RefID { get; set; }
    [Column("Department_ID")] public int? DepartmentID { get; set; }
    [StringLength(200)] public string? Note { get; set; }
    [Column("Entered_By")] public int EnteredBy { get; set; }
}

[Table("Store_Requisition")]
public sealed class StoreRequisition
{
    public long ID { get; set; }
    [Column("Department_ID")] public int DepartmentID { get; set; }
    [Column("Location_ID")] public int LocationID { get; set; }
    /// <summary>Pending, Approved, Partially Issued, Issued, Rejected, Cancelled.</summary>
    [Required, StringLength(20)] public string Status { get; set; } = StoreStatus.Pending;
    [Required, StringLength(10)] public string Priority { get; set; } = "Normal";
    [StringLength(300)] public string? Note { get; set; }
    [Column("Requested_By")] public int RequestedBy { get; set; }
    [Column("Requested_At")] public DateTime RequestedAt { get; set; } = DateTime.Now;
    [Column("Reviewed_By")] public int? ReviewedBy { get; set; }
    [Column("Reviewed_At")] public DateTime? ReviewedAt { get; set; }
    [StringLength(300), Column("Review_Note")] public string? ReviewNote { get; set; }
    public List<StoreRequisitionSub> Items { get; set; } = [];
}

[Table("Store_Requisition_Sub")]
public sealed class StoreRequisitionSub
{
    public long ID { get; set; }
    [Column("Requisition_ID")] public long RequisitionID { get; set; }
    [Column("Product_ID")] public int ProductID { get; set; }
    [Column("Qty_Requested", TypeName = "decimal(18,2)")] public decimal QtyRequested { get; set; }
    [Column("Qty_Approved", TypeName = "decimal(18,2)")] public decimal? QtyApproved { get; set; }
    [Column("Qty_Issued", TypeName = "decimal(18,2)")] public decimal QtyIssued { get; set; }

    [ForeignKey(nameof(RequisitionID))]
    public StoreRequisition? Requisition { get; set; }
}

/// <summary>Transfer between sub-stores, stock adjustment / count, or a department return.</summary>
[Table("Store_Movement")]
public sealed class StoreMovement
{
    public long ID { get; set; }
    /// <summary>TRANSFER, ADJUST or RETURN.</summary>
    [Required, StringLength(10)] public string Type { get; set; } = string.Empty;
    [Column("Location_ID")] public int LocationID { get; set; }
    [Column("To_Location_ID")] public int? ToLocationID { get; set; }
    [Column("Department_ID")] public int? DepartmentID { get; set; }
    [StringLength(40)] public string? Reason { get; set; }
    [StringLength(300)] public string? Note { get; set; }
    [Column("Entered_By")] public int EnteredBy { get; set; }
    [Column("Trans_Date")] public DateTime TransDate { get; set; } = DateTime.Now;
    public List<StoreMovementSub> Items { get; set; } = [];
}

[Table("Store_Movement_Sub")]
public sealed class StoreMovementSub
{
    public long ID { get; set; }
    [Column("Movement_ID")] public long MovementID { get; set; }
    [Column("Product_ID")] public int ProductID { get; set; }
    [Column("Stock_ID")] public int? StockID { get; set; }
    [StringLength(50), Column("Batch_No")] public string? BatchNo { get; set; }
    [Column("Expire_Date", TypeName = "date")] public DateTime? ExpireDate { get; set; }
    /// <summary>Quantity moved; for ADJUST the signed difference (new - old).</summary>
    [Column(TypeName = "decimal(18,2)")] public decimal Quantity { get; set; }
    [Column("Qty_Before", TypeName = "decimal(18,2)")] public decimal? QtyBefore { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Rate { get; set; }
    /// <summary>RETURN only: true when the item went back into usable stock.</summary>
    [Column("Is_Restocked")] public bool IsRestocked { get; set; }

    [ForeignKey(nameof(MovementID))]
    public StoreMovement? Movement { get; set; }
}

public static class StoreStatus
{
    public const string Pending = "Pending";
    public const string Approved = "Approved";
    public const string PartiallyIssued = "Partially Issued";
    public const string Issued = "Issued";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";
}
