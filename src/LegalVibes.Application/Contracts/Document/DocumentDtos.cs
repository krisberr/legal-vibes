using LegalVibes.Domain.Enums;

namespace LegalVibes.Application.Contracts.Document;

public record DocumentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DocumentType Type { get; init; }
    public DocumentStatus Status { get; init; }
    public string FilePath { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public Guid ProjectId { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public bool IsAIGenerated { get; init; }
}

public record CreateDocumentRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DocumentType Type { get; init; }
    public Guid ProjectId { get; init; }
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } = string.Empty;
}

public record UpdateDocumentRequest
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public byte[]? FileContent { get; init; }
    public string? ContentType { get; init; }
}

public record GenerateAIDocumentRequest
{
    public string DocumentType { get; init; } = string.Empty;
    public Guid ProjectId { get; init; }
    public Dictionary<string, string> Parameters { get; init; } = new();
} 