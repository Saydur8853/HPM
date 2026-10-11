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
            throw new InvalidOperationException($"Financial period for {day:dd MMM yyyy} is locked.");

        var cashierClosed = await db.CashierHandovers.AsNoTracking().AnyAsync(
            x => x.CashierID == userId && x.BusinessDate == day && x.IsLocked,
            cancellationToken);
        if (cashierClosed)
            throw new InvalidOperationException($"Your cashier day for {day:dd MMM yyyy} is already closed.");
    }
}
