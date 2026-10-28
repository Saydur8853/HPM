using System.Data;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

public static class ResilientTransaction
{
    public static async Task<TResult> ExecuteAsync<TResult>(
        IDbContextFactory<HpmDbContext> dbFactory,
        Func<HpmDbContext, Task<TResult>> operation,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        await using var strategyDb = await dbFactory.CreateDbContextAsync(cancellationToken);
        var strategy = strategyDb.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
            await using var transaction = await db.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
            try
            {
                var result = await operation(db);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public static async Task<TResult> ExecuteScopedAsync<TResult>(
        IServiceScopeFactory scopeFactory,
        Func<IServiceProvider, HpmDbContext, Task<TResult>> operation,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        await using var strategyScope = scopeFactory.CreateAsyncScope();
        var strategyDb = strategyScope.ServiceProvider.GetRequiredService<HpmDbContext>();
        var strategy = strategyDb.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var operationScope = scopeFactory.CreateAsyncScope();
            var services = operationScope.ServiceProvider;
            var db = services.GetRequiredService<HpmDbContext>();
            await using var transaction = await db.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
            try
            {
                var result = await operation(services, db);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}
