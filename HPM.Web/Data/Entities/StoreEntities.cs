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

    [ForeignKey(nameof(IssueNo))]
    public StoreIssue? Issue { get; set; }

    [ForeignKey(nameof(ProductID))]
    public StoreProduct? Product { get; set; }
}
