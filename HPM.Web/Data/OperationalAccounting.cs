using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

public static class OperationalAccounting
{
    public static async Task SyncSplitRevenueAsync(
        HpmDbContext db,
        string reference,
        long sourceId,
        decimal revenueAmount,
        decimal cashAmount,
        DateTime transactionDate,
        int userId,
        string narration,
        int? cashModuleFlag = null,
        int revenueAccountId = AccountingPosting.HospitalRevenue,
        int cashAccountId = AccountingPosting.Cash)
    {
        if (revenueAmount < 0) throw new InvalidOperationException("Revenue amount cannot be negative.");
        if (cashAmount < 0 || cashAmount > revenueAmount)
            throw new InvalidOperationException("Cash payment must be between zero and the net bill amount.");

        await FinancialControl.EnsurePostingAllowedAsync(db, transactionDate, userId);
        var receivableAmount = revenueAmount - cashAmount;
        var active = await db.AccountVouchers
            .Where(x => x.Reference == reference && x.IsPosted && x.Status != "Cancelled")
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync();

        var matches = false;
        if (active is not null)
        {
            var lines = await db.AccountVoucherLines.AsNoTracking()
                .Where(x => x.VoucherID == active.ID)
                .ToListAsync();
            var postedCash = lines.Where(x => x.AccountID == cashAccountId).Sum(x => x.Debit - x.Credit);
            var postedReceivable = lines.Where(x => x.AccountID == AccountingPosting.PatientReceivable).Sum(x => x.Debit - x.Credit);
            var postedRevenue = lines.Where(x => x.AccountID == revenueAccountId).Sum(x => x.Credit - x.Debit);
            matches = postedCash == cashAmount && postedReceivable == receivableAmount && postedRevenue == revenueAmount;
            if (!matches)
                await AccountingPosting.ReversePostedVoucherAsync(
                    db, active, "Operational source amount revised", userId, DateTime.Now);
        }

        if (!matches && revenueAmount > 0)
        {
            var lines = new List<(int AccountID, decimal Debit, decimal Credit)>();
            if (cashAmount > 0) lines.Add((cashAccountId, cashAmount, 0));
            if (receivableAmount > 0) lines.Add((AccountingPosting.PatientReceivable, receivableAmount, 0));
            lines.Add((revenueAccountId, 0, revenueAmount));
            AccountingPosting.AddPostedVoucher(
                db,
                cashAmount > 0 ? "Receipt" : "Journal",
                transactionDate,
                narration,
                reference,
                userId,
                lines.ToArray());
        }

        if (cashModuleFlag.HasValue)
        {
            var collected = await db.CentralCashCollections
                .Where(x => x.ModuleFlag == cashModuleFlag && x.ReferenceID == sourceId)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
            var delta = cashAmount - collected;
            if (delta != 0)
                db.CentralCashCollections.Add(new CentralCashCollection
                {
                    Amount = delta,
                    ModuleFlag = cashModuleFlag.Value,
                    HeadID = cashModuleFlag.Value,
                    EnteredBy = userId,
                    TransDate = DateTime.Now,
                    TransDateCL = transactionDate,
                    ReferenceID = sourceId
                });
        }
    }

    public static async Task SyncRevenueAsync(
        HpmDbContext db,
        string reference,
        long sourceId,
        decimal amount,
        DateTime transactionDate,
        int userId,
        string narration,
        int debitAccountId = AccountingPosting.Cash,
        int? cashModuleFlag = null,
        int revenueAccountId = AccountingPosting.HospitalRevenue,
        decimal doctorShareAmount = 0,
        int doctorPayableAccountId = 0)
    {
        if (amount < 0) throw new InvalidOperationException("Revenue amount cannot be negative.");
        doctorShareAmount = Math.Clamp(Math.Round(doctorShareAmount, 2), 0, amount);
        if (doctorShareAmount > 0 && doctorPayableAccountId <= 0)
            throw new InvalidOperationException("Doctor payable account is required for a doctor share.");
        await FinancialControl.EnsurePostingAllowedAsync(db, transactionDate, userId);

        var active = await db.AccountVouchers
            .Where(x => x.Reference == reference && x.IsPosted && x.Status != "Cancelled")
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync();
        decimal postedAmount = 0;
        if (active is not null)
        {
            var postedLines = await db.AccountVoucherLines.AsNoTracking().Where(x => x.VoucherID == active.ID).ToListAsync();
            postedAmount = postedLines.Where(x => x.AccountID == debitAccountId).Sum(x => x.Debit - x.Credit);
            var postedRevenue = postedLines.Where(x => x.AccountID == revenueAccountId).Sum(x => x.Credit - x.Debit);
            if (postedRevenue != amount - doctorShareAmount) postedAmount = -1; // head or doctor split changed: repost
        }

        if (active is not null && postedAmount != amount)
            await AccountingPosting.ReversePostedVoucherAsync(
                db, active, "Operational source amount revised", userId, DateTime.Now);

        if ((active is null || postedAmount != amount) && amount > 0)
        {
            var lines = new List<(int AccountID, decimal Debit, decimal Credit)> { (debitAccountId, amount, 0) };
            if (amount - doctorShareAmount > 0) lines.Add((revenueAccountId, 0, amount - doctorShareAmount));
            if (doctorShareAmount > 0) lines.Add((doctorPayableAccountId, 0, doctorShareAmount));
            AccountingPosting.AddPostedVoucher(
                db,
                debitAccountId == AccountingPosting.PatientReceivable ? "Journal" : "Receipt",
                transactionDate,
                narration,
                reference,
                userId,
                lines.ToArray());
        }

        if (cashModuleFlag.HasValue)
        {
            var collected = await db.CentralCashCollections
                .Where(x => x.ModuleFlag == cashModuleFlag && x.ReferenceID == sourceId)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
            var delta = amount - collected;
            if (delta != 0)
                db.CentralCashCollections.Add(new CentralCashCollection
                {
                    Amount = delta,
                    ModuleFlag = cashModuleFlag.Value,
                    HeadID = cashModuleFlag.Value,
                    EnteredBy = userId,
                    TransDate = DateTime.Now,
                    TransDateCL = transactionDate,
                    ReferenceID = sourceId
                });
        }
    }

    public static async Task ReverseRevenueAsync(
        HpmDbContext db,
        string entityType,
        string reference,
        long sourceId,
        decimal amount,
        DateTime transactionDate,
        int userId,
        string reason,
        int debitAccountId = AccountingPosting.Cash,
        int? cashModuleFlag = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("A reversal reason is required.");
        if (await db.FinancialReversals.AnyAsync(x => x.EntityType == entityType && x.EntityID == sourceId))
            throw new InvalidOperationException("This source transaction is already reversed.");

        var reversedAt = DateTime.Now;
        await FinancialControl.EnsurePostingAllowedAsync(db, reversedAt, userId);
        var active = await db.AccountVouchers
            .Where(x => x.Reference == reference && x.IsPosted && x.Status != "Cancelled")
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync();
        if (active is not null)
            await AccountingPosting.ReversePostedVoucherAsync(db, active, reason, userId, reversedAt);

        if (cashModuleFlag.HasValue)
        {
            var collected = await db.CentralCashCollections
                .Where(x => x.ModuleFlag == cashModuleFlag && x.ReferenceID == sourceId)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
            if (collected != 0)
                db.CentralCashCollections.Add(new CentralCashCollection
                {
                    Amount = -collected,
                    ModuleFlag = cashModuleFlag.Value,
                    HeadID = cashModuleFlag.Value,
                    EnteredBy = userId,
                    TransDate = reversedAt,
                    TransDateCL = reversedAt,
                    ReferenceID = sourceId
                });
        }

        db.FinancialReversals.Add(new FinancialReversal
        {
            EntityType = entityType,
            EntityID = sourceId,
            Amount = amount,
            Reason = reason.Trim(),
            ReversedBy = userId,
            ReversedAt = reversedAt
        });
    }

    /// <summary>
    /// Keeps exactly one posted voucher for <paramref name="reference"/> that matches <paramref name="lines"/>.
    /// Same lines: nothing happens. Different lines: the old voucher is reversed and a new one posted.
    /// No lines (or all zero): the old voucher, if any, is reversed.
    /// </summary>
    public static async Task SyncJournalAsync(
        HpmDbContext db,
        string reference,
        DateTime transactionDate,
        int userId,
        string narration,
        IReadOnlyList<(int AccountID, decimal Debit, decimal Credit)> lines,
        string voucherType = "Journal")
    {
        var wanted = Net(lines.Select(x => (x.AccountID, x.Debit - x.Credit)));
        var active = await db.AccountVouchers
            .Where(x => x.Reference == reference && x.IsPosted && x.Status != "Cancelled")
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync();
        if (active is not null)
        {
            var postedLines = await db.AccountVoucherLines.AsNoTracking().Where(x => x.VoucherID == active.ID).ToListAsync();
            var posted = Net(postedLines.Select(x => (x.AccountID, x.Debit - x.Credit)));
            if (posted.SequenceEqual(wanted)) return;
            await AccountingPosting.ReversePostedVoucherAsync(db, active, "Source document revised", userId, DateTime.Now);
        }
        if (wanted.Count == 0) return;
        await FinancialControl.EnsurePostingAllowedAsync(db, transactionDate, userId);
        AccountingPosting.AddPostedVoucher(db, voucherType, transactionDate, narration, reference, userId,
            wanted.Select(x => (x.AccountID, x.Net > 0 ? x.Net : 0m, x.Net < 0 ? -x.Net : 0m)).ToArray());

        static List<(int AccountID, decimal Net)> Net(IEnumerable<(int AccountID, decimal Amount)> rows) =>
            rows.GroupBy(x => x.AccountID)
                .Select(g => (AccountID: g.Key, Net: Math.Round(g.Sum(x => x.Amount), 2)))
                .Where(x => x.Net != 0)
                .OrderBy(x => x.AccountID)
                .ToList();
    }

    /// <summary>Reverses the active voucher for a reference, if there is one.</summary>
    public static async Task ReverseReferenceAsync(HpmDbContext db, string reference, int userId, string reason)
    {
        var active = await db.AccountVouchers
            .Where(x => x.Reference == reference && x.IsPosted && x.Status != "Cancelled")
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync();
        if (active is not null)
            await AccountingPosting.ReversePostedVoucherAsync(db, active, reason, userId, DateTime.Now);
    }
}
