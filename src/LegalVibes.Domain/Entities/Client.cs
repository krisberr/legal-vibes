using LegalVibes.Domain.Common;

namespace LegalVibes.Domain.Entities;

public class Client : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
} 