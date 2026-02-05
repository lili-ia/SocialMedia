namespace Domain.Entities;

public sealed class PostFile : MediaFile
{
    public Guid PostId { get; set; }

    public Post Post { get; set; } = null!;
}