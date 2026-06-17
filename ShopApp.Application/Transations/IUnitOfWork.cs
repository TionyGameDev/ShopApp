using System.Transactions;

namespace ShopApp.Application.Transations;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(IsolationLevel level);
    Task CommitAsync();
    Task RollbackAsync();
}