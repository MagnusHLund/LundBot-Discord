using LundBot.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LundBot.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly LundBotDbContext _context;
        private readonly ILogger _logger = Log.ForContext<UnitOfWork>();

        public UnitOfWork(LundBotDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExecuteInTransactionAsync(Func<Task<bool>> action)
        {
            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    bool success = await action();

                    if (!success)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error executing transactional unit of work; rolling back.");
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}
