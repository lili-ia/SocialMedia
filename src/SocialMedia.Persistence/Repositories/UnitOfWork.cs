using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using SocialMedia.Application.Common.Events;
using SocialMedia.Application.Common.Exceptions;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class UnitOfWork(SocialMediaDbContext db, IDomainEventDispatcher domainEventDispatcher) : IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var domainEvents = db.ChangeTracker
                .Entries<BaseEntity>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();
            
            db.ChangeTracker
                .Entries<BaseEntity>()
                .ToList()
                .ForEach(e => e.Entity.ClearDomainEvents());
            
            var result = await db.SaveChangesAsync(cancellationToken);

            await domainEventDispatcher.DispatchEventsAsync(domainEvents, cancellationToken);
            
            return result;
        }
        catch (DbUpdateException ex) when (TryMapDuplicate(ex, out var duplicateEx))
        {
            throw duplicateEx!;
        }
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
    
    private static bool TryMapDuplicate(
        DbUpdateException ex,
        out DuplicateEntityException? duplicateException)
    {
        duplicateException = null;

        if (ex.InnerException is PostgresException { SqlState: "23505" } pgEx)
        {
            if (pgEx.ConstraintName == "IX_PostLikes_UserId_PostId")
            {
                duplicateException = new DuplicatePostLikeException(pgEx.ConstraintName);
                
                return true;
            }
        }

        return false;
    }
}