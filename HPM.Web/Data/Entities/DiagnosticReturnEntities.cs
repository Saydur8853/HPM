using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class DiagnosticCashMemoReturn
{
    public long ID { get; set; }
    public long MemoID { get; set; }
    public decimal AdjustmentAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public decimal ReceivableAdjustment { get; set; }
    [Required, StringLength(500)] public string Reason { get; set; } = string.Empty;
    public int EnteredBy { get; set; }
    public DateTime TransDate { get; set; } = DateTime.Now;
    public CashMemoDInvestigation? Memo { get; set; }
    public List<DiagnosticCashMemoReturnItem> Items { get; set; } = [];
}

public sealed class DiagnosticCashMemoReturnItem
{
    public long ID { get; set; }
    public long ReturnID { get; set; }
    public long CashMemoItemID { get; set; }
    public int ServiceID { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public DiagnosticCashMemoReturn? Return { get; set; }
    public CashMemoSubDInvestigation? CashMemoItem { get; set; }
}
