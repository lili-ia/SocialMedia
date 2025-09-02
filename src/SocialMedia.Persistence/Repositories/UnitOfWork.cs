using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SocialMediaDbContext _db;

    public UnitOfWork(SocialMediaDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}