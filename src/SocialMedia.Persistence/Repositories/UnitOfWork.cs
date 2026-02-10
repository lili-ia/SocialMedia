using Microsoft.EntityFrameworkCore.Storage;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class UnitOfWork(SocialMediaDbContext db) : IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return db.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<IDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction = await db.Database.BeginTransactionAsync(cancellationToken);
        
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            
            if (_currentTransaction is not null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        finally
        {
            DisposeTransaction();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            DisposeTransaction();
        }
    }

    private void DisposeTransaction()
    {
        _currentTransaction?.Dispose();
        _currentTransaction = null;
    }
}