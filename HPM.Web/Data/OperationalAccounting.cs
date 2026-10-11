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
        int? cashModuleFlag = null)
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
            var postedCash = lines.Where(x => x.AccountID == AccountingPosting.Cash).Sum(x => x.Debit - x.Credit);
            var postedReceivable = lines.Where(x => x.AccountID == AccountingPosting.PatientReceivable).Sum(x => x.Debit - x.Credit);
            var postedRevenue = lines.Where(x => x.AccountID == AccountingPosting.HospitalRevenue).Sum(x => x.Credit - x.Debit);
            matches = postedCash == cashAmount && postedReceivable == receivableAmount && postedRevenue == revenueAmount;
            if (!matches)
                await AccountingPosting.ReversePostedVoucherAsync(
                    db, active, "Operational source amount revised", userId, DateTime.Now);
        }

        if (!matches && revenueAmount > 0)
        {
            var lines = new List<(int AccountID, decimal Debit, decimal Credit)>();
            if (cashAmount > 0) lines.Add((AccountingPosting.Cash, cashAmount, 0));
            if (receivableAmount > 0) lines.Add((AccountingPosting.PatientReceivable, receivableAmount, 0));
            lines.Add((AccountingPosting.HospitalRevenue, 0, revenueAmount));
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
        int? cashModuleFlag = null)
    {
        if (amount < 0) throw new InvalidOperationException("Revenue amount cannot be negative.");
        await FinancialControl.EnsurePostingAllowedAsync(db, transactionDate, userId);

        var active = await db.AccountVouchers
            .Where(x => x.Reference == reference && x.IsPosted && x.Status != "Cancelled")
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync();
        decimal postedAmount = 0;
        if (active is not null)
            postedAmount = await db.AccountVoucherLines
                .Where(x => x.VoucherID == active.ID && x.AccountID == debitAccountId)
                .SumAsync(x => (decimal?)(x.Debit - x.Credit)) ?? 0;

        if (active is not null && postedAmount != amount)
            await AccountingPosting.ReversePostedVoucherAsync(
                db, active, "Operational source amount revised", userId, DateTime.Now);

        if ((active is null || postedAmount != amount) && amount > 0)
            AccountingPosting.AddPostedVoucher(
                db,
                debitAccountId == AccountingPosting.Cash ? "Receipt" : "Journal",
                transactionDate,
                narration,
                reference,
                userId,
                (debitAccountId, amount, 0),
                (AccountingPosting.HospitalRevenue, 0, amount));

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
}
