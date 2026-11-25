using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class AccountHead
{
    public int ID { get; set; }
    [Required, StringLength(20)] public string Code { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(20)] public string AccountType { get; set; } = "Asset";
    public int? ParentID { get; set; }
    public bool IsActive { get; set; } = true;
    /// <summary>Head used by automatic posting; its type cannot change and it cannot be deactivated.</summary>
    public bool IsSystem { get; set; }
    /// <summary>Every voucher line on this head must name a cost center.</summary>
    public bool RequiresCostCenter { get; set; }
}

/// <summary>A hospital department that earns income (Revenue), serves others (Service) or runs the hospital (Admin).</summary>
public sealed class CostCenter
{
    public int ID { get; set; }
    [Required, StringLength(20)] public string Code { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(20)] public string CenterType { get; set; } = "Revenue";
    [StringLength(100)] public string? Incharge { get; set; }
    public decimal? AreaSqFt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; }
}

public sealed class FixedAsset
{
    public int ID { get; set; }
    [Required, StringLength(30)] public string AssetCode { get; set; } = string.Empty;
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [StringLength(100)] public string? SerialNo { get; set; }
    public int AssetAccountID { get; set; }
    public int CostCenterID { get; set; }
    [StringLength(150)] public string? Location { get; set; }
    [StringLength(150)] public string? Supplier { get; set; }
    [StringLength(50)] public string? InvoiceNo { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime InServiceDate { get; set; }
    public decimal Cost { get; set; }
    public decimal SalvageValue { get; set; }
    public int UsefulLifeYears { get; set; } = 10;
    public decimal OpeningAccumulatedDepreciation { get; set; }
    [Required, StringLength(20)] public string Status { get; set; } = "Active";
    public DateTime? WarrantyEnd { get; set; }
    public DateTime? DisposedDate { get; set; }
    public decimal? DisposalAmount { get; set; }
    public long? PurchaseVoucherID { get; set; }
    public long? DisposalVoucherID { get; set; }
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
}

public sealed class FixedAssetTransfer
{
    public int ID { get; set; }
    public int AssetID { get; set; }
    public int FromCostCenterID { get; set; }
    public int ToCostCenterID { get; set; }
    public DateTime TransferDate { get; set; }
    [StringLength(300)] public string? Reason { get; set; }
    public int EnteredBy { get; set; }
}

public sealed class FixedAssetDepreciation
{
    public long ID { get; set; }
    public int AssetID { get; set; }
    public DateTime PeriodMonth { get; set; }
    public decimal Amount { get; set; }
    public int CostCenterID { get; set; }
    public long VoucherID { get; set; }
}

/// <summary>How the month-end close spreads one cost center's expenses over the revenue cost centers.</summary>
public sealed class AllocationRule
{
    public int ID { get; set; }
    public int SourceCostCenterID { get; set; }
    /// <summary>Area, Revenue, BillCount or Fixed.</summary>
    [Required, StringLength(20)] public string Basis { get; set; } = "Area";
    /// <summary>Only for Basis = Fixed: the receiving cost center and its weight.</summary>
    public int? TargetCostCenterID { get; set; }
    public decimal? Weight { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class AllocationRun
{
    public int ID { get; set; }
    public DateTime PeriodMonth { get; set; }
    public long VoucherID { get; set; }
    public int RunBy { get; set; }
    public DateTime RunAt { get; set; }
}

public sealed class AccountVoucher
{
    public long ID { get; set; }
    [Required, StringLength(20)] public string VoucherNo { get; set; } = string.Empty;
    [Required, StringLength(20)] public string VoucherType { get; set; } = "Journal";
    public DateTime VoucherDate { get; set; }
    [StringLength(500)] public string? Narration { get; set; }
    [StringLength(100)] public string? Reference { get; set; }
    public bool IsPosted { get; set; }
    [Required, StringLength(20)] public string Status { get; set; } = "Pending";
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    [StringLength(500)] public string? CancellationReason { get; set; }
    public long? ReversalOfVoucherID { get; set; }
    public List<AccountVoucherLine> Lines { get; set; } = [];
}

public sealed class AccountVoucherLine
{
    public long ID { get; set; }
    public long VoucherID { get; set; }
    public int AccountID { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public int? CostCenterID { get; set; }
    public AccountVoucher? Voucher { get; set; }
}

public sealed class CashierHandover
{
    public long ID { get; set; }
    public DateTime BusinessDate { get; set; }
    public int CashierID { get; set; }
    public decimal SystemAmount { get; set; }
    public decimal HandedOverAmount { get; set; }
    public decimal Variance { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
    public DateTime TransDate { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? ClosedAt { get; set; }
    public int? ReopenedBy { get; set; }
    public DateTime? ReopenedAt { get; set; }
    [StringLength(500)] public string? ReopenReason { get; set; }
}

public sealed class FinancialPeriodLock
{
    public int ID { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    [Required, StringLength(500)] public string Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int LockedBy { get; set; }
    public DateTime LockedAt { get; set; }
    public int? UnlockedBy { get; set; }
    public DateTime? UnlockedAt { get; set; }
    [StringLength(500)] public string? UnlockReason { get; set; }
}

public sealed class FinancialReversal
{
    public long ID { get; set; }
    [Required, StringLength(50)] public string EntityType { get; set; } = string.Empty;
    public long EntityID { get; set; }
    public decimal Amount { get; set; }
    [Required, StringLength(500)] public string Reason { get; set; } = string.Empty;
    public long? ReversalVoucherID { get; set; }
    public int ReversedBy { get; set; }
    public DateTime ReversedAt { get; set; }
}
