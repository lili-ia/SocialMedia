using Domain.Enums;

namespace Domain.Entities;

public record AttachmentData(
    string FileName,
    ContentType ContentType,
    string StorageKey,
    long FileSizeBytes);