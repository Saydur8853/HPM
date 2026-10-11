using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

public static class SqlServerConcurrency
{
    public static async Task AcquireTransactionLockAsync(
        HpmDbContext db,
        string resource,
        CancellationToken cancellationToken = default)
    {
        if (db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("A database transaction is required before acquiring an application lock.");
        if (string.IsNullOrWhiteSpace(resource) || resource.Length > 255)
            throw new ArgumentException("The application-lock resource is invalid.", nameof(resource));

        await db.Database.ExecuteSqlInterpolatedAsync($$"""
            DECLARE @lockResult int;
            EXEC @lockResult = sys.sp_getapplock
                @Resource = {{resource}},
                @LockMode = 'Exclusive',
                @LockOwner = 'Transaction',
                @LockTimeout = 15000;
            IF @lockResult < 0
                THROW 51000, 'The requested record is busy. Please try again.', 1;
            """, cancellationToken);
    }
}
