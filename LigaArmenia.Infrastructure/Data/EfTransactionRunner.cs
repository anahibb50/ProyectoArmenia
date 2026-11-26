using LigaArmenia.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LigaArmenia.Infrastructure.Data;

public sealed class EfTransactionRunner : ITransactionRunner
{
    private readonly LigaArmeniaDbContext _dbContext;

    public EfTransactionRunner(LigaArmeniaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default)
    {
        if (_dbContext.Database.CurrentTransaction is not null)
        {
            return await action(cancellationToken);
        }

        var strategy = _dbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await action(cancellationToken);
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
