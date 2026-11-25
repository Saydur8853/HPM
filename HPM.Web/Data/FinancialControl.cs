using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

public static class FinancialControl
{
    public static async Task EnsurePostingAllowedAsync(
        HpmDbContext db,
        DateTime transactionDate,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var day = transactionDate.Date;
        var periodLocked = await db.FinancialPeriodLocks.AsNoTracking().AnyAsync(
            x => x.IsActive && x.FromDate <= day && x.ToDate >= day,
            cancellationToken);
        if (periodLocked)
            throw new InvalidOperationException(
                $"Financial period for {day:dd MMM yyyy} is locked. / {day:dd MMM yyyy} তারিখের আর্থিক সময়কাল লক করা আছে।");

        var cashierClosed = await db.CashierHandovers.AsNoTracking().AnyAsync(
            x => x.CashierID == userId && x.BusinessDate == day && x.IsLocked,
            cancellationToken);
        if (cashierClosed)
            throw new InvalidOperationException(
                $"Your cashier day for {day:dd MMM yyyy} is already closed. / {day:dd MMM yyyy} তারিখের আপনার ক্যাশিয়ার দিন ইতিমধ্যে বন্ধ করা হয়েছে।");
    }
}
