using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class HospitalLicense
{
    public int ID { get; set; } = 1;
    public bool IsActivated { get; set; }
    [StringLength(20)] public string BillingCycle { get; set; } = "Monthly";
    public DateTime? PurchasedAt { get; set; }
    public DateTime? SubscriptionExpiresAt { get; set; }
    public decimal HostingCost { get; set; }
    public DateTime? HostingExpiresAt { get; set; }
    public decimal DomainCost { get; set; }
    public DateTime? DomainExpiresAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class HospitalModuleLicense
{
    public int ID { get; set; }
    [Required, StringLength(50)] public string ModuleCode { get; set; } = "";
    public bool IsEnabled { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
}

public sealed class HospitalSubscriptionInvoice
{
    public long ID { get; set; }
    [Required, StringLength(25)] public string Status { get; set; } = "Issued";
    [Required, StringLength(20)] public string BillingCycle { get; set; } = "Monthly";
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal ModuleTotal { get; set; }
    public decimal HostingCost { get; set; }
    public decimal DomainCost { get; set; }
    public decimal Total { get; set; }
    public DateTime? PaidAt { get; set; }
    public List<HospitalSubscriptionInvoiceItem> Items { get; set; } = [];
}

public sealed class HospitalSubscriptionInvoiceItem
{
    public long ID { get; set; }
    public long InvoiceID { get; set; }
    [Required, StringLength(50)] public string ModuleCode { get; set; } = "";
    [Required, StringLength(100)] public string ModuleName { get; set; } = "";
    public decimal Price { get; set; }
    public HospitalSubscriptionInvoice? Invoice { get; set; }
}
