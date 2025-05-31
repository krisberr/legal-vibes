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
    public string ClientName { get; set; } = string.Empty;
    public string ClientReference { get; set; } = string.Empty;
    
    // Navigation properties
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    
    // Client-specific information for trademark applications
    public string? TrademarkName { get; set; }
    public string? TrademarkDescription { get; set; }
    public string? GoodsAndServices { get; set; }
    public string? SpecialConsiderations { get; set; }
} 