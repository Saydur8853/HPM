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
    /// <summary>ID 4 was "Medicine Purchase"; with perpetual inventory it is the cost of medicine sold.</summary>
    public const int CostOfMedicineSold = 4;

    /// <summary>One journal line. CostCenterID is the department the amount belongs to.</summary>
    public readonly record struct PostingLine(int AccountID, decimal Debit, decimal Credit, int? CostCenterID = null);

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
        params (int AccountID, decimal Debit, decimal Credit)[] lines) =>
        AddPostedVoucher(db, voucherType, voucherDate, narration, reference, enteredBy,
            lines.Select(x => new PostingLine(x.AccountID, x.Debit, x.Credit)).ToArray());

    public static AccountVoucher AddPostedVoucher(
        HpmDbContext db,
        string voucherType,
        DateTime voucherDate,
        string narration,
        string reference,
        int enteredBy,
        PostingLine[] lines)
    {
        lines = lines.Where(x => x.Debit != 0 || x.Credit != 0).ToArray();
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
                Credit = line.Credit,
                CostCenterID = line.CostCenterID
            });

        db.AccountVouchers.Add(voucher);
        return voucher;
    }

    /// <summary>
    /// Keeps the automatic voucher for <paramref name="reference"/> equal to <paramref name="lines"/>:
    /// nothing happens when the posted voucher already matches; otherwise the old one is reversed
    /// (audit trail kept) and the new lines are posted.
    /// </summary>
    public static async Task SyncPostedVoucherAsync(
        HpmDbContext db,
        string reference,
        string voucherType,
        DateTime voucherDate,
        string narration,
        int userId,
        PostingLine[] lines)
    {
        lines = lines.Where(x => x.Debit != 0 || x.Credit != 0).ToArray();
        var active = await db.AccountVouchers
            .Where(x => x.Reference == reference && x.IsPosted && x.Status != "Cancelled")
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync();

        if (active is not null)
        {
            var posted = await db.AccountVoucherLines.AsNoTracking().Where(x => x.VoucherID == active.ID).ToListAsync();
            if (Net(posted.Select(x => new PostingLine(x.AccountID, x.Debit, x.Credit, x.CostCenterID))).SetEquals(Net(lines))
                && active.VoucherDate.Date == voucherDate.Date)
                return;
            await ReversePostedVoucherAsync(db, active, "Operational source amount revised", userId, DateTime.Now);
        }

        if (lines.Length > 0)
            AddPostedVoucher(db, voucherType, voucherDate, narration, reference, userId, lines);
    }

    private static HashSet<(int, int?, decimal)> Net(IEnumerable<PostingLine> lines) =>
        lines.GroupBy(x => (x.AccountID, x.CostCenterID))
            .Select(g => (g.Key.AccountID, g.Key.CostCenterID, g.Sum(x => x.Debit - x.Credit)))
            .Where(x => x.Item3 != 0)
            .ToHashSet();

    /// <summary>Splits an amount over weights, rounded to paisa, the rounding remainder on the largest share.</summary>
    public static List<(TKey Key, decimal Amount)> Split<TKey>(decimal amount, IEnumerable<(TKey Key, decimal Weight)> weights)
    {
        var list = weights.Where(x => x.Weight > 0).ToList();
        var total = list.Sum(x => x.Weight);
        if (amount == 0 || total <= 0) return [];
        var result = list.Select(x => (x.Key, Amount: Math.Round(amount * x.Weight / total, 2))).ToList();
        var diff = amount - result.Sum(x => x.Amount);
        if (diff != 0)
        {
            var i = result.FindIndex(x => x.Amount == result.Max(y => y.Amount));
            result[i] = (result[i].Key, result[i].Amount + diff);
        }
        return result;
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
                Credit = x.Debit,
                CostCenterID = x.CostCenterID
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
