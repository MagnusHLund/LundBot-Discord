namespace LundBot.Application.Common.Persistence
{
    /// <summary>
    /// Allows application services to group multiple repository writes into a single atomic
    /// database transaction, so a later failure can roll back earlier writes instead of leaving
    /// inconsistent state (for example, a recorded score source without its corresponding score
    /// increment).
    /// </summary>
    public interface IUnitOfWork
    {
        Task<bool> ExecuteInTransactionAsync(Func<Task<bool>> action);
    }
}
