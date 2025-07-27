using LegalVibes.Domain.Common;
using LegalVibes.Domain.Enums;

namespace LegalVibes.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    public ProjectType Type { get; set; }
    public DateTime? DueDate { get; set; }
    public string? ReferenceNumber { get; set; }
    
    // Navigation properties
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public Guid ClientId { get; set; }
    public virtual Client Client { get; set; } = null!;
    
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    
    // Project type-specific information (can be expanded later)
    public string? TrademarkName { get; set; }
    public string? TrademarkDescription { get; set; }
    public string? GoodsAndServices { get; set; }
    public string? SpecialConsiderations { get; set; }
} 