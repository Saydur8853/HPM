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
