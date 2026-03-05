using System.Text.Json.Serialization;
using Domain.Enums;

namespace SocialMedia.Application.DTOs.Chat;

public class MessageAttachmentDto
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = null!;

    public ContentType ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    [JsonIgnore] 
    public string StorageKey { get; set; } = null!;

    public string Url { get; set; } = null!;
}