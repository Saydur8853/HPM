using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

public static class AccountingPosting
{
    public const int Cash = 1;
    public const int HospitalRevenue = 2;
    public const int PharmacySales = 3;
    public const int MedicinePurchase = 4;
    public const int SupplierPayable = 6;
    public const int PatientReceivable = 7;
    public const int PatientAdvance = 8;

    public static class ModuleFlags
    {
        public const int IpdReceipt = 1;
        public const int Pharmacy = 4;
        public const int OpdTicket = 10;
        public const int Emergency = 11;
        public const int Diagnostic = 12;
        public const int Ot = 13;
    }

    public static void AddPostedVoucher(
        HpmDbContext db,
        string voucherType,
        DateTime voucherDate,
        string narration,
        string reference,
        int enteredBy,
        params (int AccountID, decimal Debit, decimal Credit)[] lines)
    {
        var debit = lines.Sum(x => x.Debit);
        var credit = lines.Sum(x => x.Credit);
        if (lines.Length < 2 || debit <= 0 || debit != credit)
            throw new InvalidOperationException("Automatic accounting voucher is not balanced.");

        var voucher = new AccountVoucher
        {
            VoucherNo = $"A{DateTime.UtcNow:yyMMddHHmmssfff}{Guid.NewGuid():N}"[..20],
            VoucherType = voucherType,
            VoucherDate = voucherDate.Date,
            Narration = narration,
            Reference = reference,
            IsPosted = true,
            Status = "Approved",
            EnteredBy = enteredBy,
            TransDate = DateTime.Now,
            ApprovedBy = enteredBy,
            ApprovedAt = DateTime.Now
        };

        foreach (var line in lines)
            voucher.Lines.Add(new AccountVoucherLine
            {
                AccountID = line.AccountID,
                Debit = line.Debit,
                Credit = line.Credit
            });

        db.AccountVouchers.Add(voucher);
    }

    public static async Task<AccountVoucher> ReversePostedVoucherAsync(
        HpmDbContext db,
        AccountVoucher original,
        string reason,
        int userId,
        DateTime reversedAt)
    {
        if (!original.IsPosted || original.Status is "Pending" or "Rejected")
            throw new InvalidOperationException("Only an approved voucher can be reversed.");
        if (original.Status == "Cancelled")
            throw new InvalidOperationException("This voucher is already cancelled.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("A cancellation reason is required.");
        if (await db.AccountVouchers.AnyAsync(x => x.ReversalOfVoucherID == original.ID))
            throw new InvalidOperationException("A reversal voucher already exists.");

        var lines = await db.AccountVoucherLines.AsNoTracking()
            .Where(x => x.VoucherID == original.ID)
            .ToListAsync();
        if (lines.Count < 2)
            throw new InvalidOperationException("The original voucher has no balanced journal lines.");

        var reversal = new AccountVoucher
        {
            VoucherNo = $"R{DateTime.UtcNow:yyMMddHHmmssfff}{Guid.NewGuid():N}"[..20],
            VoucherType = "Reversal",
            VoucherDate = reversedAt.Date,
            Narration = $"Reversal of {original.VoucherNo}: {reason.Trim()}",
            Reference = $"REV:{original.ID}",
            IsPosted = true,
            Status = "Approved",
            EnteredBy = userId,
            TransDate = reversedAt,
            ApprovedBy = userId,
            ApprovedAt = reversedAt,
            ReversalOfVoucherID = original.ID,
            Lines = lines.Select(x => new AccountVoucherLine
            {
                AccountID = x.AccountID,
                Debit = x.Credit,
                Credit = x.Debit
            }).ToList()
        };

        original.Status = "Cancelled";
        original.CancelledBy = userId;
        original.CancelledAt = reversedAt;
        original.CancellationReason = reason.Trim();
        db.AccountVouchers.Add(reversal);
        return reversal;
    }
}
