using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPM.Web.Data.Entities;

public sealed class CanteenCategory
{
    public int ID { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
}

public sealed class CanteenProduct
{
    public int ID { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    public int CategoryID { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal? SaleRate { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal? BuyRate { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal? Balance { get; set; }
    public int? ReorderLevel { get; set; }
    [StringLength(50)] public string? Unit { get; set; }
    public bool? IsKitchenItem { get; set; }

    public CanteenCategory? Category { get; set; }
}

public sealed class CanteenSale
{
    public long ID { get; set; }
    [Required, StringLength(50)] public string CustomerName { get; set; } = "Walk-in Customer";
    [StringLength(20)] public string? Contact { get; set; }
    [Required, StringLength(10)] public string CustomerType { get; set; } = "Customer";
    [Column(TypeName = "numeric(18,2)")] public decimal? Total { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal? ServiceCharge { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal? Discount { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal? Paid { get; set; }
    [StringLength(5)] public string? TokenNo { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; } = DateTime.Now;
    [Column(TypeName = "numeric(18,2)")] public decimal? VAT { get; set; }

    public List<CanteenSaleSub> Items { get; set; } = [];
}

public sealed class CanteenSaleSub
{
    public long ID { get; set; }
    public long BillNo { get; set; }
    public int? ServiceID { get; set; }
    public int? SLNO { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal? Rate { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal? BuyRate { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal? Quantity { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal? Total { get; set; }

    public CanteenSale? Sale { get; set; }
    public CanteenProduct? Product { get; set; }
}

public sealed class CanteenChallan
{
    public long ID { get; set; }
    public int Supplier { get; set; }
    [Required, StringLength(20)] public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.Today;
    public bool IsPosted { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal Discount { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal VAT { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; } = DateTime.Now;

    public List<CanteenChallanSub> Items { get; set; } = [];
}

public sealed class CanteenChallanSub
{
    public long ID { get; set; }
    public long ChallanNo { get; set; }
    public int ServiceID { get; set; }
    public int SLNO { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal BuyRate { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal Quantity { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal Discount { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal VAT { get; set; }

    public CanteenChallan? Challan { get; set; }
    public CanteenProduct? Product { get; set; }
}

public sealed class CanteenDebtor
{
    public int ID { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(50)] public string Designation { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18,2)")] public decimal Balance { get; set; }
}

public sealed class CanteenDebtorLedger
{
    public long ID { get; set; }
    public int DebtorID { get; set; }
    public long BillNo { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Debit { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Credit { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Balance { get; set; }
    public DateTime TransDate { get; set; } = DateTime.Now;

    public CanteenDebtor? Debtor { get; set; }
}
