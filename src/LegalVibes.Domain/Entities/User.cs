using LegalVibes.Domain.Common;

namespace LegalVibes.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    
    // Navigation properties
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    
    public string FullName => $"{FirstName} {LastName}";
} 