using LegalVibes.Domain.Enums;
using LegalVibes.Application.Contracts.Client;

namespace LegalVibes.Application.Contracts.Project;

public record ProjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ProjectStatus Status { get; init; }
    public ProjectType Type { get; init; }
    public DateTime? DueDate { get; init; }
    public string? ReferenceNumber { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    
    // Client information
    public Guid ClientId { get; init; }
    public ClientDto Client { get; init; } = null!;
    
    // Project type-specific fields
    public string? TrademarkName { get; init; }
    public string? TrademarkDescription { get; init; }
    public string? GoodsAndServices { get; init; }
    public string? SpecialConsiderations { get; init; }
}

public record CreateProjectRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ProjectType Type { get; init; }
    public DateTime? DueDate { get; init; }
    public string? ReferenceNumber { get; init; }
    public Guid ClientId { get; init; }
    
    // Project type-specific fields
    public string? TrademarkName { get; init; }
    public string? TrademarkDescription { get; init; }
    public string? GoodsAndServices { get; init; }
    public string? SpecialConsiderations { get; init; }
}

public record UpdateProjectRequest
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public ProjectStatus? Status { get; init; }
    public DateTime? DueDate { get; init; }
    public string? ReferenceNumber { get; init; }
    public Guid? ClientId { get; init; }
    
    // Project type-specific fields
    public string? TrademarkName { get; init; }
    public string? TrademarkDescription { get; init; }
    public string? GoodsAndServices { get; init; }
    public string? SpecialConsiderations { get; init; }
} 