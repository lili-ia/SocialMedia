using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IFileRepository
{
    Task AddAsync<T>(T file, CancellationToken ct = default) where T : MediaFile;

    Task AddRangeAsync<T>(T[] files, CancellationToken ct = default) where T : MediaFile;

    Task RemoveAsync<T>(T file, CancellationToken ct = default) where T : MediaFile;
}