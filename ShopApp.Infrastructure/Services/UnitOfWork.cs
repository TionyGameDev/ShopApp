using System.Transactions;
using Microsoft.EntityFrameworkCore.Storage;
using ShopApp.Infrastructure.Data;

namespace ShopApp.Application.Transations;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IDbContextTransaction _transaction;

    public async Task BeginTransactionAsync(IsolationLevel level)
        => _transaction = await context.Database.BeginTransactionAsync();

    public async Task CommitAsync()
        => await _transaction.CommitAsync();

    public async Task RollbackAsync()
        => await _transaction.RollbackAsync();
}