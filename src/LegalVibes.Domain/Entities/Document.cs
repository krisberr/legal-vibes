using LegalVibes.Domain.Common;
using LegalVibes.Domain.Enums;

namespace LegalVibes.Domain.Entities;

public class Document : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DocumentType Type { get; set; }
    public DocumentStatus Status { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public string? Version { get; set; }
    
    // For AI-generated documents
    public bool IsAIGenerated { get; set; }
    public string? GenerationPrompt { get; set; }
    public string? AIModel { get; set; }
    
    // Navigation properties
    public Guid ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;
} 