using LegalVibes.Domain.Enums;

namespace LegalVibes.Application.Contracts.Project;

public record ProjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ProjectStatus Status { get; init; }
    public ProjectType Type { get; init; }
    public DateTime? DueDate { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string ClientReference { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    
    // Trademark specific fields
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
    public string ClientName { get; init; } = string.Empty;
    public string ClientReference { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    
    // Trademark specific fields
    public string? TrademarkName { get; init; }
    public string? TrademarkDescription { get; init; }
    public string? GoodsAndServices { get; init; }
    public string? SpecialConsiderations { get; init; }
}

public record UpdateProjectRequest
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DateTime? DueDate { get; init; }
    public string? ClientName { get; init; }
    public string? ClientReference { get; init; }
    
    // Trademark specific fields
    public string? TrademarkName { get; init; }
    public string? TrademarkDescription { get; init; }
    public string? GoodsAndServices { get; init; }
    public string? SpecialConsiderations { get; init; }
} 