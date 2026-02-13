using Domain.Entities;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class FileRepository(SocialMediaDbContext db) : IFileRepository
{
    public async Task AddAsync<T>(T file, CancellationToken ct = default) where T : MediaFile
    {
        await db.Set<T>()
            .AddAsync(file, ct);
    }

    public async Task AddRangeAsync<T>(T[] files, CancellationToken ct = default) where T : MediaFile
    {
        await db.Set<T>()
            .AddRangeAsync(files, ct);
    }
}